using GestionAppels.Server.Models; // For StatutDemande
using System;

namespace GestionAppels.Server.Dtos;

public record FicheDto(
    Guid Id,
    string? Telephone1,
    string? Telephone2,
    string? Telephone3,
    string Affiliation, 
    string AdherentNom, 
    string? Details,
    Guid SousTypeDemandeId,
    string SousTypeDemandeNom, 
    Guid UserId,
    string UserNom, 
    StatutDemande Statut,
    DateTime CreatedAt,
    DateTime? UpdatedAt 
);
