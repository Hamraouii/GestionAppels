namespace GestionAppels.Server.Models
{
    public class Adherent : BaseEntity
    {
        public string? Nom {  get; set; }
        public string? Prenom { get; set; }

        public string? Ville { get; set; }
        public char? Sexe { get; set; }

        public string? Adresse { get; set; }

        public string? Immatriculation { get; set; }

        public string? Cin { get; set; }

        public DateOnly? DateNaissance { get; set; }
        public required string Affiliation { get; set; }
        public int? StatutAdherent { get; set; }

        public virtual ICollection<Fiche> Fiches { get; set; } = new HashSet<Fiche>();
    }
}
