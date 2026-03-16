using GestionAppels.Server.Dtos;
using GestionAppels.Server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GestionAppels.Server.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class DemandesController : ControllerBase
    {
        private readonly IDemandeService _demandeService;

        public DemandesController(IDemandeService demandeService)
        {
            _demandeService = demandeService;
        }

        private Guid GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (Guid.TryParse(userIdClaim, out var userId))
            {
                return userId;
            }
            throw new InvalidOperationException("User ID not found in token");
        }

        // TypeDemande endpoints
        [HttpGet("types")]
        public async Task<IActionResult> GetAllTypeDemandes()
        {
            var typeDemandes = await _demandeService.GetAllTypeDemandesAsync();
            return Ok(typeDemandes);
        }

        [HttpGet("types/{id}")]
        public async Task<IActionResult> GetTypeDemandeById(Guid id)
        {
            var typeDemande = await _demandeService.GetTypeDemandeByIdAsync(id);
            if (typeDemande == null)
            {
                return NotFound();
            }
            return Ok(typeDemande);
        }

        [HttpPost("types")]
        public async Task<IActionResult> CreateTypeDemande([FromBody] CreateTypeDemandeDto createTypeDemandeDto)
        {
            var userId = GetUserId();
            var newTypeDemande = await _demandeService.CreateTypeDemandeAsync(createTypeDemandeDto, userId);
            return CreatedAtAction(nameof(GetTypeDemandeById), new { id = newTypeDemande.Id }, newTypeDemande);
        }

        [HttpPut("types/{id}")]
        public async Task<IActionResult> UpdateTypeDemande(Guid id, [FromBody] UpdateTypeDemandeDto updateTypeDemandeDto)
        {
            var userId = GetUserId();
            await _demandeService.UpdateTypeDemandeAsync(id, updateTypeDemandeDto, userId);
            return NoContent();
        }

        [HttpDelete("types/{id}")]
        public async Task<IActionResult> DeleteTypeDemande(Guid id)
        {
            await _demandeService.DeleteTypeDemandeAsync(id);
            return NoContent();
        }

        // SousTypeDemande endpoints
        [HttpGet("types/{typeDemandeId}/soustypes")]
        public async Task<IActionResult> GetSousTypeDemandesByTypeId(Guid typeDemandeId)
        {
            var sousTypeDemandes = await _demandeService.GetSousTypeDemandesByTypeIdAsync(typeDemandeId);
            return Ok(sousTypeDemandes);
        }

        [HttpGet("soustypes/{id}")]
        public async Task<IActionResult> GetSousTypeDemandeById(Guid id)
        {
            var sousTypeDemande = await _demandeService.GetSousTypeDemandeByIdAsync(id);
            if (sousTypeDemande == null)
            {
                return NotFound();
            }
            return Ok(sousTypeDemande);
        }

        [HttpPost("soustypes")]
        public async Task<IActionResult> CreateSousTypeDemande([FromBody] CreateSousTypeDemandeDto createSousTypeDemandeDto)
        {
            var userId = GetUserId();
            var newSousTypeDemande = await _demandeService.CreateSousTypeDemandeAsync(createSousTypeDemandeDto, userId);
            return CreatedAtAction(nameof(GetSousTypeDemandeById), new { id = newSousTypeDemande.Id }, newSousTypeDemande);
        }

        [HttpPut("soustypes/{id}")]
        public async Task<IActionResult> UpdateSousTypeDemande(Guid id, [FromBody] UpdateSousTypeDemandeDto updateSousTypeDemandeDto)
        {
            var userId = GetUserId();
            await _demandeService.UpdateSousTypeDemandeAsync(id, updateSousTypeDemandeDto, userId);
            return NoContent();
        }

        [HttpDelete("soustypes/{id}")]
        public async Task<IActionResult> DeleteSousTypeDemande(Guid id)
        {
            await _demandeService.DeleteSousTypeDemandeAsync(id);
            return NoContent();
        }
    }
}
