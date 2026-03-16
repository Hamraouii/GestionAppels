namespace GestionAppels.Server.Models
{
    public class Service : BaseEntity
    {

        public string IntituleService { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty ;

        public Guid DivisionID { get; set; }
        public virtual Division Division { get; set; } = null!;

    }
}
