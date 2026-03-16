using GestionAppels.Server.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace GestionAppels.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SyncController : ControllerBase
    {
        private readonly IAdherentSyncService _adherentSyncService;
        private readonly ILogger<SyncController> _logger;

        public SyncController(IAdherentSyncService adherentSyncService, ILogger<SyncController> logger)
        {
            _adherentSyncService = adherentSyncService;
            _logger = logger;
        }

        [HttpPost("adherents")] // POST /api/sync/adherents
        public async Task<IActionResult> TriggerAdherentSync()
        {
            _logger.LogInformation("Manual Adherent sync triggered via API.");
            try
            {
                // Don't await here if you want the API to return immediately 
                // and let the sync run in the background.
                // For initial testing, awaiting is fine to see completion or errors directly.
                await _adherentSyncService.SyncAdherentsAsync();
                _logger.LogInformation("Manual Adherent sync process completed (or is running in background).");
                return Ok("Adherent synchronization process initiated successfully.");
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error during manually triggered Adherent sync.");
                return StatusCode(500, "An error occurred while starting the adherent synchronization process.");
            }
        }
    }
}
