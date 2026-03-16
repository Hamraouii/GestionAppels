namespace GestionAppels.Server.Dtos;

public record AdherentSearchResultDto(
    string Affiliation,
    string Nom,
    string Prenom,
    string? Immatriculation,
    string? Cin
);
