using System;

namespace GestionAppels.Server.Dtos
{
    public record CreateSousTypeDemandeDto(string Intitule, string Description, Guid TypeDemandeId);
}
