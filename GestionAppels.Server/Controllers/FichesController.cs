using GestionAppels.Server.Dtos;
using GestionAppels.Server.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization; 

namespace GestionAppels.Server.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class FichesController : ControllerBase
{
    private readonly IFicheService _ficheService;
    private readonly ILogger<FichesController> _logger;

    public FichesController(IFicheService ficheService, ILogger<FichesController> logger)
    {
        _ficheService = ficheService;
        _logger = logger;
    }

    // GET: api/fiches
    [HttpGet]
    public async Task<ActionResult<IEnumerable<FicheDto>>> GetAllFiches()
    {
        try
        {
            var fiches = await _ficheService.GetAllFichesAsync();
            return Ok(fiches);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all fiches.");
            return StatusCode(500, "An unexpected error occurred.");
        }
    }

    // GET: api/fiches/{id}
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<FicheDto>> GetFicheById(Guid id)
    {
        try
        {
            var fiche = await _ficheService.GetFicheByIdAsync(id);
            if (fiche == null)
            {
                return NotFound($"Fiche with ID {id} not found.");
            }
            return Ok(fiche);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving fiche with ID {FicheId}.", id);
            return StatusCode(500, "An unexpected error occurred.");
        }
    }

    // GET: api/fiches/adherent/{affiliation}
    [HttpGet("adherent/{affiliation}")]
    public async Task<ActionResult<IEnumerable<FicheDto>>> GetFichesByAdherentAffiliation(string affiliation)
    {
        if (string.IsNullOrWhiteSpace(affiliation))
        {
            return BadRequest("Affiliation cannot be empty.");
        }
        try
        {
            var fiches = await _ficheService.GetFichesByAdherentAffiliationAsync(affiliation);
            return Ok(fiches);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving fiches for affiliation {Affiliation}.", affiliation);
            return StatusCode(500, "An unexpected error occurred.");
        }
    }

    // POST: api/fiches
    [HttpPost]
    public async Task<ActionResult<FicheDto>> CreateFiche([FromBody] CreateFicheDto createFicheDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        Guid userId;
        try
        {
            userId = GetUserId(); 
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "User ID could not be determined while creating fiche.");
            return Unauthorized("User ID could not be determined."); // Or Forbid()
        }

        try
        {
            var createdFiche = await _ficheService.CreateFicheAsync(createFicheDto, userId);
            return CreatedAtAction(nameof(GetFicheById), new { id = createdFiche.Id }, createdFiche);
        }
        catch (ArgumentException ex) // Catch specific exceptions from service for bad data
        {
            _logger.LogWarning(ex, "Argument error while creating fiche.");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating new fiche.");
            return StatusCode(500, "An unexpected error occurred while creating the fiche.");
        }
    }


    // PUT: api/fiches/{id}
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateFiche(Guid id, [FromBody] UpdateFicheDto updateFicheDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        Guid userId;
        try
        {
            userId = GetUserId(); 
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "User ID could not be determined while updating fiche {FicheId}.", id);
            return Unauthorized("User ID could not be determined."); // Or Forbid()
        }

        try
        {
            var success = await _ficheService.UpdateFicheAsync(id, updateFicheDto, userId);
            if (!success)
            {
                return NotFound($"Fiche with ID {id} not found or update failed.");
            }
            return NoContent(); 
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Argument error while updating fiche {FicheId}.", id);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating fiche with ID {FicheId}.", id);
            return StatusCode(500, "An unexpected error occurred while updating the fiche.");
        }
    }

    // DELETE: api/fiches/{id}
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteFiche(Guid id)
    {
        try
        {
            var success = await _ficheService.DeleteFicheAsync(id);
            if (!success)
            {
                return NotFound($"Fiche with ID {id} not found.");
            }
            return NoContent(); 
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting fiche with ID {FicheId}.", id);
            return StatusCode(500, "An unexpected error occurred while deleting the fiche.");
        }
    }

    // GET: api/fiches/{ficheId}/service-history
    [HttpGet("{ficheId:guid}/service-history")]
    public async Task<ActionResult<IEnumerable<FicheServiceHistoryDto>>> GetFicheServiceHistory(Guid ficheId)
    {
        try
        {
            var history = await _ficheService.GetFicheServiceHistoryAsync(ficheId);
            return Ok(history);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving service history for fiche {FicheId}.", ficheId);
            return StatusCode(500, "An unexpected error occurred.");
        }
    }

    // POST: api/fiches/{ficheId}/service-history
    [HttpPost("{ficheId:guid}/service-history")]
    public async Task<ActionResult<FicheServiceHistoryDto>> AddFicheServiceHistory(Guid ficheId, [FromBody] AddFicheServiceHistoryRequest request)
    {
        try
        {
            var history = await _ficheService.AddFicheServiceHistoryAsync(ficheId, request.ServiceId, request.Notes);
            return Ok(history);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding service history for fiche {FicheId}.", ficheId);
            return StatusCode(500, "An unexpected error occurred.");
        }
    }

    private Guid GetUserId()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            throw new InvalidOperationException("User ID not found or invalid in token claims.");
        }
        return userId;
    }
}
