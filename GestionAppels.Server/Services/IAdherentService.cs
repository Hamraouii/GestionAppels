using GestionAppels.Server.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestionAppels.Server.Services;

public interface IAdherentService
{
    Task<IEnumerable<AdherentSearchResultDto>> SearchAdherentsAsync(string searchTerm, int maxResults = 10);
    // We can add other Adherent-specific methods here later if needed,
    // e.g., GetAdherentDetailsByAffiliationAsync, etc.
}
