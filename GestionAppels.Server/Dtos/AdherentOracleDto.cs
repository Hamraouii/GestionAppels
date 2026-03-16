using System;

namespace GestionAppels.Server.Dtos
{
    public record AdherentOracleDto
    (
        string Nom,
        string Prenom,
        string Ville,
        string Sexe, 
        string Adresse,
        string Immatriculation,
        string Cin,
        DateTime? DateNaissance, 
        long? Affiliation,      // Changed from string? to long? to match Oracle NUMBER type via Dapper
        short? StatutAdherent,   // Changed from int? to short? to match Oracle NUMBER type via Dapper
        DateTime? DateSaisieOracle, // From datesais
        DateTime? DateMajOracle      // From date_maj
    );
}
