using GestionAppels.Server.Data;
using GestionAppels.Server.Dtos;
using GestionAppels.Server.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GestionAppels.Server.Services;

public class FicheService : IFicheService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<FicheService> _logger;

    public FicheService(ApplicationDbContext context, ILogger<FicheService> logger)
    {
        _context = context;
        _logger = logger;
    }

    private static FicheDto MapFicheToDto(Fiche fiche)
    {
        return new FicheDto(
            fiche.Id,
            fiche.Telephone1,
            fiche.Telephone2,
            fiche.Telephone3,
            fiche.Affiliation,
            (fiche.Adherent != null) ? $"{fiche.Adherent.Nom} {fiche.Adherent.Prenom}" : "N/A",
            fiche.Details,
            fiche.SousTypeDemandeId,
            fiche.SousTypeDemande?.Intitule ?? "N/A", // Handle potential null if not included
            fiche.UserId,
            (fiche.User != null) ? $"{fiche.User.FirstName} {fiche.User.LastName}" : "N/A", // Handle potential null
            fiche.Statut,
            fiche.CreatedAt,
            fiche.UpdatedAt
        );
    }

    public async Task<IEnumerable<FicheDto>> GetAllFichesAsync()
    {
        return await _context.Fiches
            .Include(f => f.Adherent)
            .Include(f => f.SousTypeDemande)
            .Include(f => f.User)
            .Select(f => MapFicheToDto(f))
            .ToListAsync();
    }

    public async Task<FicheDto?> GetFicheByIdAsync(Guid id)
    {
        var fiche = await _context.Fiches
            .Include(f => f.Adherent)
            .Include(f => f.SousTypeDemande)
            .Include(f => f.User)
            .FirstOrDefaultAsync(f => f.Id == id);

        return fiche != null ? MapFicheToDto(fiche) : null;
    }

    public async Task<IEnumerable<FicheDto>> GetFichesByAdherentAffiliationAsync(string affiliation)
    {
        return await _context.Fiches
            .Where(f => f.Affiliation == affiliation)
            .Include(f => f.Adherent)
            .Include(f => f.SousTypeDemande)
            .Include(f => f.User)
            .Select(f => MapFicheToDto(f))
            .ToListAsync();
    }

    public async Task<FicheDto> CreateFicheAsync(CreateFicheDto createFicheDto, Guid userId)
    {
        // Verify Adherent exists
        var adherentExists = await _context.Adherents.AnyAsync(a => a.Affiliation == createFicheDto.Affiliation);
        if (!adherentExists)
        {
            // Or throw a custom exception, or return a result object indicating failure
            _logger.LogWarning("Attempted to create Fiche for non-existent Adherent with Affiliation {Affiliation}", createFicheDto.Affiliation);
            throw new ArgumentException($"Adherent with Affiliation '{createFicheDto.Affiliation}' not found.", nameof(createFicheDto.Affiliation));
        }
        
        // Verify SousTypeDemande exists
        var sousTypeDemandeExists = await _context.SousTypeDemandes.AnyAsync(s => s.Id == createFicheDto.SousTypeDemandeId);
        if (!sousTypeDemandeExists)
        {
            _logger.LogWarning("Attempted to create Fiche with non-existent SousTypeDemandeId {SousTypeDemandeId}", createFicheDto.SousTypeDemandeId);
            throw new ArgumentException($"SousTypeDemande with Id '{createFicheDto.SousTypeDemandeId}' not found.", nameof(createFicheDto.SousTypeDemandeId));
        }

        // Verify User exists
        //var userExists = await _context.Users.AnyAsync(u => u.Id == createFicheDto.UserId);
        //if (!userExists)
        //{
        //    _logger.LogWarning("Attempted to create Fiche with non-existent UserId {UserId}", createFicheDto.UserId);
        //    throw new ArgumentException($"User with Id '{createFicheDto.UserId}' not found.", nameof(createFicheDto.UserId));
        //}

        var fiche = new Fiche
        {
            Telephone1 = createFicheDto.Telephone1,
            Telephone2 = createFicheDto.Telephone2,
            Telephone3 = createFicheDto.Telephone3,
            Affiliation = createFicheDto.Affiliation,
            Details = createFicheDto.Details,
            SousTypeDemandeId = createFicheDto.SousTypeDemandeId,
            UserId = userId, // Set from authenticated user
            // Statut = createFicheDto.Statut, // Original line, replaced by parsing logic below
            Statut = Enum.TryParse<StatutDemande>(createFicheDto.Statut, true, out var parsedStatut) ? parsedStatut : throw new ArgumentException($"Invalid Statut value: {createFicheDto.Statut}"),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow, // Initially same as CreatedAt
            CreatedBy = userId
        };

        _context.Fiches.Add(fiche);
        await _context.SaveChangesAsync();
        
        // Add initial service history if service is provided (assume User's service)
        var user = await _context.Users.FindAsync(userId);
        if (user != null)
        {
            var initialHistory = new FicheServiceHistory
            {
                FicheId = fiche.Id,
                //ServiceId = user.ServiceId,
                EnteredAt = DateTime.UtcNow,
                Notes = "Fiche created in user's service."
            };
            _context.FicheServiceHistories.Add(initialHistory);
            await _context.SaveChangesAsync();
        }

        // Re-fetch with includes to populate navigation properties for the DTO
        var createdFicheWithIncludes = await _context.Fiches
            .Include(f => f.Adherent)
            .Include(f => f.SousTypeDemande)
            .Include(f => f.User)
            .FirstAsync(f => f.Id == fiche.Id);

        return MapFicheToDto(createdFicheWithIncludes);
    }

    public async Task<bool> UpdateFicheAsync(Guid id, UpdateFicheDto updateFicheDto, Guid userId)
    {
        var fiche = await _context.Fiches.FindAsync(id);

        if (fiche == null)
        {
            return false;
        }

        // Update properties if they are provided in the DTO
        fiche.Telephone1 = updateFicheDto.Telephone1 ?? fiche.Telephone1;
        fiche.Telephone2 = updateFicheDto.Telephone2 ?? fiche.Telephone2;
        fiche.Telephone3 = updateFicheDto.Telephone3 ?? fiche.Telephone3;
        fiche.Details = updateFicheDto.Details ?? fiche.Details;

        if (updateFicheDto.SousTypeDemandeId.HasValue)
        {
            var sousTypeDemandeExists = await _context.SousTypeDemandes.AnyAsync(s => s.Id == updateFicheDto.SousTypeDemandeId.Value);
            if (!sousTypeDemandeExists) {
                _logger.LogWarning("Attempted to update Fiche {FicheId} with non-existent SousTypeDemandeId {SousTypeDemandeId}", id, updateFicheDto.SousTypeDemandeId.Value);
                throw new ArgumentException($"SousTypeDemande with Id '{updateFicheDto.SousTypeDemandeId.Value}' not found.", nameof(updateFicheDto.SousTypeDemandeId));
            }
            fiche.SousTypeDemandeId = updateFicheDto.SousTypeDemandeId.Value;
        }

        if (updateFicheDto.UserId.HasValue)
        {
             var userExists = await _context.Users.AnyAsync(u => u.Id == updateFicheDto.UserId.Value);
            if (!userExists) {
                _logger.LogWarning("Attempted to update Fiche {FicheId} with non-existent UserId {UserId}", id, updateFicheDto.UserId.Value);
                throw new ArgumentException($"User with Id '{updateFicheDto.UserId.Value}' not found.", nameof(updateFicheDto.UserId));
            }
            fiche.UserId = updateFicheDto.UserId.Value;
        }

        if (updateFicheDto.Statut.HasValue)
        {
            fiche.Statut = updateFicheDto.Statut.Value;
        }

        fiche.UpdatedAt = DateTime.UtcNow;
        fiche.UpdatedBy = userId;

        try
        {
            await _context.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateConcurrencyException ex)
        {
            _logger.LogError(ex, "Concurrency error while updating Fiche {FicheId}", id);
            return false; // Or rethrow/handle as appropriate
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating Fiche {FicheId}", id);
            throw; // Rethrow for global error handling to catch it
        }
    }

    public async Task<bool> DeleteFicheAsync(Guid id)
    {
        var fiche = await _context.Fiches.FindAsync(id);

        if (fiche == null)
        {
            return false;
        }

        _context.Fiches.Remove(fiche);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<FicheServiceHistoryDto>> GetFicheServiceHistoryAsync(Guid ficheId)
    {
        return await _context.FicheServiceHistories
            .Where(h => h.FicheId == ficheId)
            .Include(h => h.Service)
            .OrderBy(h => h.EnteredAt)
            .Select(h => new FicheServiceHistoryDto(
                h.Id,
                h.FicheId,
                h.ServiceId,
                h.Service.IntituleService,
                h.EnteredAt,
                h.ExitedAt,
                h.Notes
            ))
            .ToListAsync();
    }

    public async Task<FicheServiceHistoryDto> AddFicheServiceHistoryAsync(Guid ficheId, Guid serviceId, string? notes = null)
    {
        var fiche = await _context.Fiches.FindAsync(ficheId);
        if (fiche == null)
            throw new ArgumentException($"Fiche with Id '{ficheId}' not found.");
        var service = await _context.Services.FindAsync(serviceId);
        if (service == null)
            throw new ArgumentException($"Service with Id '{serviceId}' not found.");

        // Optionally close previous history entry
        var lastHistory = await _context.FicheServiceHistories
            .Where(h => h.FicheId == ficheId && h.ExitedAt == null)
            .OrderByDescending(h => h.EnteredAt)
            .FirstOrDefaultAsync();
        if (lastHistory != null)
        {
            lastHistory.ExitedAt = DateTime.UtcNow;
        }

        var history = new FicheServiceHistory
        {
            FicheId = ficheId,
            ServiceId = serviceId,
            EnteredAt = DateTime.UtcNow,
            Notes = notes
        };
        _context.FicheServiceHistories.Add(history);
        await _context.SaveChangesAsync();

        return new FicheServiceHistoryDto(
            history.Id,
            history.FicheId,
            history.ServiceId,
            service.IntituleService,
            history.EnteredAt,
            history.ExitedAt,
            history.Notes
        );
    }
}
