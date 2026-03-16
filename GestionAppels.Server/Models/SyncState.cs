using System.ComponentModel.DataAnnotations;

namespace GestionAppels.Server.Models
{
    public class SyncState
    {
        [Key]
        public required string SyncProcessName { get; set; }
        public DateTime LastSyncTimestamp { get; set; }
    }
}
