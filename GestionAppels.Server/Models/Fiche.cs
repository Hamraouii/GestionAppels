using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionAppels.Server.Models;

public class Fiche : BaseEntity
{
    public string? Telephone1 { get; set; }
    public string? Telephone2 { get; set; }
    public string? Telephone3 { get; set; }

    [MaxLength(15)]
    public string Affiliation { get; set; } = null!; 

    [ForeignKey("Affiliation")]
    public virtual Adherent Adherent { get; set; } = null!;

    public string? Details { get; set; }

    public Guid SousTypeDemandeId { get; set; }

    [ForeignKey("SousTypeDemandeId")]
    public virtual SousTypeDemande SousTypeDemande { get; set; } = null!;

    public Guid UserId { get; set; }

    [ForeignKey("UserId")]
    public virtual User User { get; set; } = null!;
    public StatutDemande Statut { get; set; }

    public virtual ICollection<FicheServiceHistory> ServiceHistory { get; set; } = new List<FicheServiceHistory>();
}