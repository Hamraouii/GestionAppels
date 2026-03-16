namespace GestionAppels.Server.Models
{
    public class SousTypeDemande : BaseEntity
    {
        public string Intitule { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public Guid TypeDemandeId { get; set; }
        public virtual TypeDemande TypeDemande { get; set; } = null!;
    }
}
