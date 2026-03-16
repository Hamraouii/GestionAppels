using System.Collections.Generic;

namespace GestionAppels.Server.Models
{
    public class TypeDemande : BaseEntity
    {
        public string IntituleDemande { get; set; } = string.Empty;
        public string DescriptionDemande { get; set; } = string.Empty;

        public virtual ICollection<SousTypeDemande> SousTypeDemandes { get; set; } = new List<SousTypeDemande>();
    }
}
