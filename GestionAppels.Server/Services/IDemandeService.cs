using GestionAppels.Server.Dtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestionAppels.Server.Services
{
    public interface IDemandeService
    {
        // TypeDemande methods
        Task<IEnumerable<TypeDemandeDto>> GetAllTypeDemandesAsync();
                Task<TypeDemandeDto?> GetTypeDemandeByIdAsync(Guid id);
        Task<TypeDemandeDto> CreateTypeDemandeAsync(CreateTypeDemandeDto createTypeDemandeDto, Guid userId);
        Task UpdateTypeDemandeAsync(Guid id, UpdateTypeDemandeDto updateTypeDemandeDto, Guid userId);
        Task DeleteTypeDemandeAsync(Guid id);

        // SousTypeDemande methods
        Task<IEnumerable<SousTypeDemandeDto>> GetSousTypeDemandesByTypeIdAsync(Guid typeDemandeId);
                Task<SousTypeDemandeDto?> GetSousTypeDemandeByIdAsync(Guid id);
        Task<SousTypeDemandeDto> CreateSousTypeDemandeAsync(CreateSousTypeDemandeDto createSousTypeDemandeDto, Guid userId);
        Task UpdateSousTypeDemandeAsync(Guid id, UpdateSousTypeDemandeDto updateSousTypeDemandeDto, Guid userId);
        Task DeleteSousTypeDemandeAsync(Guid id);
    }
}
