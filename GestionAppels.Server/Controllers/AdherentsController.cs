using GestionAppels.Server.Dtos;
using GestionAppels.Server.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestionAppels.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AdherentsController : ControllerBase
{
    private readonly IAdherentService _adherentService;
    private readonly ILogger<AdherentsController> _logger;

    public AdherentsController(IAdherentService adherentService, ILogger<AdherentsController> logger)
    {
        _adherentService = adherentService;
        _logger = logger;
    }

    // GET: api/adherents/search?term=...&maxResults=...
    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<AdherentSearchResultDto>>> SearchAdherents(
        [FromQuery] string term,
        [FromQuery] int maxResults = 10)
    {
        if (string.IsNullOrWhiteSpace(term))
        {
            return BadRequest("Search term cannot be empty.");
        }
        try
        {
            var adherents = await _adherentService.SearchAdherentsAsync(term, maxResults);
            return Ok(adherents);
        }
        catch (System.Exception ex)
        {
            _logger.LogError(ex, "Error occurred while searching for adherents with term: {SearchTerm}", term);
            // Return a generic error response
            return StatusCode(500, "An unexpected error occurred while processing your request.");
        }
    }

}
