using System;
using System.Collections.Generic;

namespace GestionAppels.Server.Dtos
{
    public record TypeDemandeDto(Guid Id, string IntituleDemande, string DescriptionDemande, List<SousTypeDemandeDto> SousTypeDemandes);
}
