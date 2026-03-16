using System.Collections.Generic;

namespace GestionAppels.Server.Models
{
    public class Division : BaseEntity
    {
        public string IntituleDivision { get; set; } = string.Empty;

        public string AbreviationDivision {  get; set; } = string.Empty;

        public virtual ICollection<Service> Services { get; set; } = new List<Service>();
    }
}
