using GestionAppels.Server.Models; // For StatutDemande
using System;
using System.ComponentModel.DataAnnotations;

namespace GestionAppels.Server.Dtos;

public record UpdateFicheDto(
    [MaxLength(20)]
    string? Telephone1,
    [MaxLength(20)]
    string? Telephone2,
    [MaxLength(20)]
    string? Telephone3,
    string? Details,
    Guid? SousTypeDemandeId, 
    Guid? UserId, 
    StatutDemande? Statut 
);
