using System;

namespace GestionAppels.Server.Dtos
{
    public record FicheServiceHistoryDto(
        Guid Id,
        Guid FicheId,
        Guid ServiceId,
        string ServiceName,
        DateTime EnteredAt,
        DateTime? ExitedAt,
        string? Notes
    );
} 