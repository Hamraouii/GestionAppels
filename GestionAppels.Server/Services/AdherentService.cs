using GestionAppels.Server.Data;
using GestionAppels.Server.Dtos;
using GestionAppels.Server.Models; // Required for Adherent entity
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GestionAppels.Server.Services;

public class AdherentService : IAdherentService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<AdherentService> _logger;

    public AdherentService(ApplicationDbContext context, ILogger<AdherentService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<AdherentSearchResultDto>> SearchAdherentsAsync(string searchTerm, int maxResults = 10)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return Enumerable.Empty<AdherentSearchResultDto>();
        }

        var searchTermLower = searchTerm.ToLower(); // Case-insensitive search

        // It's generally better to use EF.Functions.Like for "starts with" if the database provider supports it well
        // and it's configured for case-insensitivity, or ensure the database collation is case-insensitive.
        // For simplicity here, we'll use StartsWith after converting both sides to lower case if needed,
        // or rely on database's default case sensitivity for StartsWith.
        // PostgreSQL's StartsWith is case-sensitive by default. For case-insensitive, use ILIKE or ToLower.

        var query = _context.Adherents
            .Where(a => (!string.IsNullOrEmpty(a.Affiliation) && a.Affiliation.ToLower().StartsWith(searchTermLower)) ||
                        (!string.IsNullOrEmpty(a.Immatriculation) && a.Immatriculation.ToLower().StartsWith(searchTermLower)))
            .OrderBy(a => a.Nom) // Optional: order by name
            .ThenBy(a => a.Prenom);

        return await query
            .Take(maxResults)
            .Select(a => new AdherentSearchResultDto(
                a.Affiliation,
                a.Nom,
                a.Prenom,
                a.Immatriculation,
                a.Cin
            ))
            .ToListAsync();
    }
}
