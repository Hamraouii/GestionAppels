using System;

namespace GestionAppels.Server.Models
{
    public class FicheServiceHistory : BaseEntity
    {
        public Guid FicheId { get; set; }
        public virtual Fiche Fiche { get; set; } = null!;

        public Guid ServiceId { get; set; }
        public virtual Service Service { get; set; } = null!;

        public DateTime EnteredAt { get; set; }
        public DateTime? ExitedAt { get; set; }

        public string? Notes { get; set; }
    }
} 