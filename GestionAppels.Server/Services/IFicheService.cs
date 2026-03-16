using GestionAppels.Server.Dtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestionAppels.Server.Services;

public interface IFicheService
{
    Task<IEnumerable<FicheDto>> GetAllFichesAsync();
    Task<FicheDto?> GetFicheByIdAsync(Guid id);
    Task<IEnumerable<FicheDto>> GetFichesByAdherentAffiliationAsync(string affiliation);
    Task<FicheDto> CreateFicheAsync(CreateFicheDto createFicheDto, Guid userId);
    Task<bool> UpdateFicheAsync(Guid id, UpdateFicheDto updateFicheDto, Guid userId);
    Task<bool> DeleteFicheAsync(Guid id);
    Task<IEnumerable<FicheServiceHistoryDto>> GetFicheServiceHistoryAsync(Guid ficheId);
    Task<FicheServiceHistoryDto> AddFicheServiceHistoryAsync(Guid ficheId, Guid serviceId, string? notes = null);
}
