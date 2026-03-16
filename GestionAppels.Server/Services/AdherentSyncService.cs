using Dapper;
using GestionAppels.Server.Data;
using GestionAppels.Server.Dtos;
using GestionAppels.Server.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GestionAppels.Server.Services
{
    public interface IAdherentSyncService
    {
        Task SyncAdherentsAsync();
    }

    public class AdherentSyncService : IAdherentSyncService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AdherentSyncService> _logger;
        private const string OracleConnectionStringName = "OracleConnection";
        private const string SyncProcessNameKey = "AdherentSync";
        private const int BatchSize = 10000;

        public AdherentSyncService(ApplicationDbContext dbContext, IConfiguration configuration, ILogger<AdherentSyncService> logger)
        {
            _dbContext = dbContext;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SyncAdherentsAsync()
        {
            _logger.LogInformation("Starting Adherent synchronization process at {StartTime}", DateTime.UtcNow);
            var oracleConnectionString = _configuration.GetConnectionString(OracleConnectionStringName);

            if (string.IsNullOrEmpty(oracleConnectionString))
            {
                _logger.LogError("Oracle connection string '{ConnectionStringName}' not found in configuration.", OracleConnectionStringName);
                return;
            }

            DateTime lastSyncTimestamp = await GetLastSyncTimestampAsync();
            DateTime currentSyncStartTime = DateTime.UtcNow;
            
            int totalNewCount = 0;
            int totalUpdatedCount = 0;
            int pageOffset = 0; // Represents the starting row for the next batch
            bool moreDataToFetch = true;

            // Base SQL query without ordering or paging. Ordering is applied inside the ROWNUM subquery.
            string baseSql = @"SELECT
                    adherent.NOM_ADHE   AS Nom,
                    adherent.PRENADHE   AS Prenom,
                    ville.libentad      AS Ville,
                    adherent.SEXEPERS   AS Sexe,
                    adherent.ADREADHE   AS Adresse,
                    adherent.IMMATRIC   AS Immatriculation,
                    adherent.NUME_CIN   AS Cin,
                    adherent.DATENAIS   AS DateNaissance,
                    adherent.MATRADHE   AS Affiliation,
                    adherent.STATADHE   AS StatutAdherent,
                    adherent.datesais   AS DateSaisieOracle,
                    adherent.date_maj   AS DateMajOracle
                FROM
                    taadod.adhe adherent
                JOIN
                    taadod.enad ville ON adherent.codentad = ville.codentad
                WHERE
                    ((adherent.datesais > :P_LAST_SYNC_TIMESTAMP OR adherent.date_maj > :P_LAST_SYNC_TIMESTAMP)
                    OR :P_LAST_SYNC_TIMESTAMP_IS_NULL = 1)";

            try
            {
                using (var connection = new OracleConnection(oracleConnectionString))
                {
                    await connection.OpenAsync();

                    while (moreDataToFetch)
                    {
                        int lowerBound = pageOffset;
                        int upperBound = pageOffset + BatchSize;

                        // Using ROWNUM for pagination, compatible with older Oracle versions (pre-12c)
                        // Explicitly list columns to avoid mapping the temporary 'rnum' column to the DTO.
                        string pagedSql = $@"SELECT Nom, Prenom, Ville, Sexe, Adresse, Immatriculation, Cin, DateNaissance, Affiliation, StatutAdherent, DateSaisieOracle, DateMajOracle FROM (
                                SELECT t.*, ROWNUM rnum FROM (
                                    {baseSql} ORDER BY COALESCE(adherent.date_maj, adherent.datesais), adherent.MATRADHE
                                ) t WHERE ROWNUM <= :P_UPPER_BOUND
                            ) WHERE rnum > :P_LOWER_BOUND";
                        
                        _logger.LogInformation("Fetching batch from Oracle. LowerBound: {LowerBound}, UpperBound: {UpperBound}, LastSyncTimestamp: {LastSyncTimestamp}", 
                                               lowerBound, upperBound, lastSyncTimestamp == DateTime.MinValue ? (object)"NULL (full sync)" : lastSyncTimestamp);

                        var oracleAdherentsBatch = await connection.QueryAsync<AdherentOracleDto>(pagedSql, new
                        {
                            P_LAST_SYNC_TIMESTAMP = lastSyncTimestamp == DateTime.MinValue ? (DateTime?)null : lastSyncTimestamp,
                            P_LAST_SYNC_TIMESTAMP_IS_NULL = lastSyncTimestamp == DateTime.MinValue ? 1 : 0,
                            P_UPPER_BOUND = upperBound,
                            P_LOWER_BOUND = lowerBound
                        });

                        var batchAsList = oracleAdherentsBatch.AsList();

                        if (batchAsList.Count == 0)
                        {
                            moreDataToFetch = false;
                            _logger.LogInformation("No more adherents found in Oracle for the current criteria. Batch processing complete.");
                            break;
                        }

                        _logger.LogInformation("Fetched {Count} adherents in current batch from Oracle.", batchAsList.Count);

                        // Prepare for batch lookup in PostgreSQL using Affiliation
                        var affiliationsInBatch = batchAsList
                            .Where(dto => dto.Affiliation.HasValue)
                            .Select(dto => dto.Affiliation.ToString())
                            .Distinct()
                            .ToList();

                        if (affiliationsInBatch.Count == 0)
                        {
                            _logger.LogWarning("Current batch from Oracle contains no DTOs with valid Affiliation values. Skipping PG lookup for this batch.");
                            pageOffset += batchAsList.Count;
                             if (batchAsList.Count < BatchSize) {
                                moreDataToFetch = false;
                                _logger.LogInformation("Last batch processed (fetched less than page size, no valid affiliations).");
                            }
                            continue;
                        }
                        
                        // Batch fetch existing adherents from PostgreSQL based on Affiliation
                        var existingAdherentsInPg = await _dbContext.Adherents
                            .Where(a => a.Affiliation != null && affiliationsInBatch.Contains(a.Affiliation))
                            .ToListAsync();

                        var existingAdherentsLookup = existingAdherentsInPg
                            .ToDictionary(a => a.Affiliation!); 

                        // This set will track affiliations we've already processed in this batch
                        // to prevent duplicate key errors if the source data itself has duplicates.
                        var processedAffiliationsInBatch = new HashSet<string>();
                        
                        int currentBatchNewCount = 0;
                        int currentBatchUpdatedCount = 0;

                        foreach (var dto in batchAsList)
                        {
                            if (!dto.Affiliation.HasValue || string.IsNullOrWhiteSpace(dto.Nom) || string.IsNullOrWhiteSpace(dto.Prenom))
                            {
                                _logger.LogWarning("Skipping adherent due to missing required fields (Affiliation, Nom, or Prenom). Affiliation: {Affiliation}, Nom: {Nom}, Prenom: {Prenom}", dto.Affiliation, dto.Nom, dto.Prenom);
                                continue;
                            }
                            
                            string dtoAffiliationString = dto.Affiliation.ToString()!;

                            // If we've already handled this affiliation in this batch, skip to the next one.
                            // The HashSet.Add method returns false if the item is already in the set.
                            if (!processedAffiliationsInBatch.Add(dtoAffiliationString))
                            {
                                _logger.LogWarning("Duplicate Affiliation '{Affiliation}' found within the same source batch. Processing the first record and skipping subsequent ones.", dtoAffiliationString);
                                continue;
                            }

                            if (existingAdherentsLookup.TryGetValue(dtoAffiliationString, out var existingAdherent))
                            {
                                // Update existing
                                var sexeFromDto = !string.IsNullOrEmpty(dto.Sexe) ? dto.Sexe[0] : (char?)null;
                                var dateNaissanceFromDto = dto.DateNaissance.HasValue ? DateOnly.FromDateTime(dto.DateNaissance.Value) : (DateOnly?)null;

                                bool needsUpdate = existingAdherent.Nom != dto.Nom ||
                                                   existingAdherent.Prenom != dto.Prenom ||
                                                   existingAdherent.Ville != dto.Ville ||
                                                   existingAdherent.Sexe != sexeFromDto ||
                                                   existingAdherent.Adresse != dto.Adresse ||
                                                   existingAdherent.Immatriculation != dto.Immatriculation ||
                                                   existingAdherent.Cin != dto.Cin ||
                                                   existingAdherent.DateNaissance != dateNaissanceFromDto ||
                                                   existingAdherent.StatutAdherent != dto.StatutAdherent;

                                if (needsUpdate)
                                {
                                    existingAdherent.Nom = dto.Nom;
                                    existingAdherent.Prenom = dto.Prenom;
                                    existingAdherent.Ville = dto.Ville;
                                    existingAdherent.Sexe = sexeFromDto;
                                    existingAdherent.Adresse = dto.Adresse;
                                    existingAdherent.Immatriculation = dto.Immatriculation;
                                    existingAdherent.Cin = dto.Cin;
                                    existingAdherent.DateNaissance = dateNaissanceFromDto;
                                    existingAdherent.StatutAdherent = dto.StatutAdherent;
                                    existingAdherent.UpdatedAt = currentSyncStartTime;
                                    _dbContext.Adherents.Update(existingAdherent);
                                    currentBatchUpdatedCount++;
                                }
                            }
                            else
                            {
                                // Add new

                                // Npgsql requires DateTime Kind to be UTC for 'timestamp with time zone' columns.
                                // Dapper reads Oracle DATE as 'Unspecified', so we must explicitly set the Kind.
                                var createdAt = dto.DateSaisieOracle ?? currentSyncStartTime;
                                if (createdAt.Kind == DateTimeKind.Unspecified)
                                {
                                    createdAt = DateTime.SpecifyKind(createdAt, DateTimeKind.Utc);
                                }

                                var updatedAt = dto.DateMajOracle ?? dto.DateSaisieOracle ?? currentSyncStartTime;
                                if (updatedAt.Kind == DateTimeKind.Unspecified)
                                {
                                    updatedAt = DateTime.SpecifyKind(updatedAt, DateTimeKind.Utc);
                                }
                                
                                var newAdherent = new Adherent
                                {
                                    Nom = dto.Nom,
                                    Prenom = dto.Prenom,
                                    Ville = dto.Ville,
                                    Sexe = !string.IsNullOrEmpty(dto.Sexe) ? dto.Sexe[0] : null,
                                    Adresse = dto.Adresse,
                                    Immatriculation = dto.Immatriculation,
                                    Cin = dto.Cin,
                                    DateNaissance = dto.DateNaissance.HasValue ? DateOnly.FromDateTime(dto.DateNaissance.Value) : null,
                                    Affiliation = dtoAffiliationString,
                                    StatutAdherent = dto.StatutAdherent,
                                    CreatedAt = createdAt,
                                    UpdatedAt = updatedAt
                                };
                                _dbContext.Adherents.Add(newAdherent);
                                currentBatchNewCount++;
                            }
                        }

                        if (currentBatchNewCount > 0 || currentBatchUpdatedCount > 0)
                        {
                            await _dbContext.SaveChangesAsync();
                            _logger.LogInformation("Batch saved to PostgreSQL: {NewCount} new, {UpdatedCount} updated.", currentBatchNewCount, currentBatchUpdatedCount);
                        }
                        else
                        {
                            _logger.LogInformation("No changes in the current batch to save to PostgreSQL.");
                        }
                        
                        totalNewCount += currentBatchNewCount;
                        totalUpdatedCount += currentBatchUpdatedCount;
                        pageOffset += batchAsList.Count;

                        if (batchAsList.Count < BatchSize)
                        {
                            moreDataToFetch = false;
                             _logger.LogInformation("Last batch processed (fetched less than page size).");
                        }
                    } // End while(moreDataToFetch)
                } // End using OracleConnection

                await UpdateLastSyncTimestampAsync(currentSyncStartTime);
                _logger.LogInformation("Adherent synchronization process finished successfully. Total new: {TotalNew}, Total updated: {TotalUpdated}", totalNewCount, totalUpdatedCount);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during adherent synchronization process.");
            }
            finally
            {
                 _logger.LogInformation("Adherent synchronization process ended at {EndTime}", DateTime.UtcNow);
            }
        }

        private async Task<DateTime> GetLastSyncTimestampAsync()
        {
            var syncState = await _dbContext.SyncStates.FirstOrDefaultAsync(s => s.SyncProcessName == SyncProcessNameKey);
            if (syncState == null)
            {
                _logger.LogInformation("No previous sync timestamp found for '{SyncProcessName}'. Will perform a full sync.", SyncProcessNameKey);
                return DateTime.MinValue; 
            }
            _logger.LogInformation("Last sync timestamp for '{SyncProcessName}' is {LastSyncTimestamp}", SyncProcessNameKey, syncState.LastSyncTimestamp);
            return syncState.LastSyncTimestamp;
        }

        private async Task UpdateLastSyncTimestampAsync(DateTime timestamp)
        {
            var syncState = await _dbContext.SyncStates.FirstOrDefaultAsync(s => s.SyncProcessName == SyncProcessNameKey);
            if (syncState == null)
            {
                syncState = new SyncState { SyncProcessName = SyncProcessNameKey, LastSyncTimestamp = timestamp };
                _dbContext.SyncStates.Add(syncState);
            }
            else
            {
                syncState.LastSyncTimestamp = timestamp;
                _dbContext.SyncStates.Update(syncState);
            }
            await _dbContext.SaveChangesAsync(); 
            _logger.LogInformation("Updated last sync timestamp for '{SyncProcessName}' to {NewTimestamp}", SyncProcessNameKey, timestamp);
        }
    }
}
