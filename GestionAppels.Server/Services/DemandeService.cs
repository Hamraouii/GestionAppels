using GestionAppels.Server.Data;
using GestionAppels.Server.Dtos;
using GestionAppels.Server.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GestionAppels.Server.Services
{
    public class DemandeService : IDemandeService
    {
        private readonly ApplicationDbContext _context;

        public DemandeService(ApplicationDbContext context)
        {
            _context = context;
        }

        // TypeDemande methods
        public async Task<TypeDemandeDto> CreateTypeDemandeAsync(CreateTypeDemandeDto createTypeDemandeDto, Guid userId)
        {
            var typeDemande = new TypeDemande
            {
                IntituleDemande = createTypeDemandeDto.IntituleDemande,
                DescriptionDemande = createTypeDemandeDto.DescriptionDemande,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = userId
            };

            _context.TypeDemandes.Add(typeDemande);
            await _context.SaveChangesAsync();

            return new TypeDemandeDto(typeDemande.Id, typeDemande.IntituleDemande, typeDemande.DescriptionDemande, new List<SousTypeDemandeDto>());
        }

        public async Task DeleteTypeDemandeAsync(Guid id)
        {
            var typeDemande = await _context.TypeDemandes.FindAsync(id);
            if (typeDemande != null)
            {
                _context.TypeDemandes.Remove(typeDemande);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<TypeDemandeDto>> GetAllTypeDemandesAsync()
        {
            return await _context.TypeDemandes
                .Include(td => td.SousTypeDemandes)
                .Select(td => new TypeDemandeDto(td.Id, td.IntituleDemande, td.DescriptionDemande, 
                    td.SousTypeDemandes.Select(std => new SousTypeDemandeDto(std.Id, std.Intitule, std.Description)).ToList())).ToListAsync();
        }

        public async Task<TypeDemandeDto?> GetTypeDemandeByIdAsync(Guid id)
        {
            var typeDemande = await _context.TypeDemandes
                .Include(td => td.SousTypeDemandes)
                .FirstOrDefaultAsync(td => td.Id == id);

            if (typeDemande == null)
            {
                return null;
            }

            return new TypeDemandeDto(typeDemande.Id, typeDemande.IntituleDemande, typeDemande.DescriptionDemande, 
                typeDemande.SousTypeDemandes.Select(std => new SousTypeDemandeDto(std.Id, std.Intitule, std.Description)).ToList());
        }

        public async Task UpdateTypeDemandeAsync(Guid id, UpdateTypeDemandeDto updateTypeDemandeDto, Guid userId)
        {
            var typeDemande = await _context.TypeDemandes.FindAsync(id);
            if (typeDemande != null)
            {
                typeDemande.IntituleDemande = updateTypeDemandeDto.IntituleDemande;
                typeDemande.DescriptionDemande = updateTypeDemandeDto.DescriptionDemande;
                typeDemande.UpdatedAt = DateTime.UtcNow;
                typeDemande.UpdatedBy = userId;

                _context.TypeDemandes.Update(typeDemande);
                await _context.SaveChangesAsync();
            }
        }

        // SousTypeDemande methods
        public async Task<SousTypeDemandeDto> CreateSousTypeDemandeAsync(CreateSousTypeDemandeDto createSousTypeDemandeDto, Guid userId)
        {
            var sousTypeDemande = new SousTypeDemande
            {
                Intitule = createSousTypeDemandeDto.Intitule,
                Description = createSousTypeDemandeDto.Description,
                TypeDemandeId = createSousTypeDemandeDto.TypeDemandeId,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = userId
            };

            _context.SousTypeDemandes.Add(sousTypeDemande);
            await _context.SaveChangesAsync();

            return new SousTypeDemandeDto(sousTypeDemande.Id, sousTypeDemande.Intitule, sousTypeDemande.Description);
        }

        public async Task DeleteSousTypeDemandeAsync(Guid id)
        {
            var sousTypeDemande = await _context.SousTypeDemandes.FindAsync(id);
            if (sousTypeDemande != null)
            {
                _context.SousTypeDemandes.Remove(sousTypeDemande);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<SousTypeDemandeDto?> GetSousTypeDemandeByIdAsync(Guid id)
        {
            var sousTypeDemande = await _context.SousTypeDemandes.FindAsync(id);
            if (sousTypeDemande == null)
            {
                return null;
            }

            return new SousTypeDemandeDto(sousTypeDemande.Id, sousTypeDemande.Intitule, sousTypeDemande.Description);
        }

        public async Task<IEnumerable<SousTypeDemandeDto>> GetSousTypeDemandesByTypeIdAsync(Guid typeDemandeId)
        {
            return await _context.SousTypeDemandes
                .Where(std => std.TypeDemandeId == typeDemandeId)
                .Select(std => new SousTypeDemandeDto(std.Id, std.Intitule, std.Description)).ToListAsync();
        }

        public async Task UpdateSousTypeDemandeAsync(Guid id, UpdateSousTypeDemandeDto updateSousTypeDemandeDto, Guid userId)
        {
            var sousTypeDemande = await _context.SousTypeDemandes.FindAsync(id);
            if (sousTypeDemande != null)
            {
                sousTypeDemande.Intitule = updateSousTypeDemandeDto.Intitule;
                sousTypeDemande.Description = updateSousTypeDemandeDto.Description;
                sousTypeDemande.UpdatedAt = DateTime.UtcNow;
                sousTypeDemande.UpdatedBy = userId;

                _context.SousTypeDemandes.Update(sousTypeDemande);
                await _context.SaveChangesAsync();
            }
        }
    }
}
