namespace GestionAppels.Server.Dtos;

public record UserLoginResponseDto(UserViewDto User, string Token);
