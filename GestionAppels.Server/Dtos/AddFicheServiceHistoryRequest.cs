namespace GestionAppels.Server.Dtos
{
    public record AddFicheServiceHistoryRequest(
        Guid ServiceId,
        string? Notes
    );
} 