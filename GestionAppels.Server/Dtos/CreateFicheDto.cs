using GestionAppels.Server.Models; // For StatutDemande
using System;
using System.ComponentModel.DataAnnotations;

namespace GestionAppels.Server.Dtos;

public record CreateFicheDto(
    [MaxLength(20)]
    string? Telephone1,
    [MaxLength(20)]
    string? Telephone2,
    [MaxLength(20)]
    string? Telephone3,
    [Required]
    [MaxLength(15)]
    string Affiliation, // Required to link to an Adherent
    string? Details,
    [Required]
    Guid SousTypeDemandeId,
    [Required]
    string Statut // Changed from StatutDemande to string. Parsing will be needed in the service.
);
