using GestionAppels.Server.Dtos;
using System.Security.Claims;

namespace GestionAppels.Server.Services;

public interface IAccountService
{
    Task<UserViewDto?> CreateUserAsync(UserCreateDto userCreateDto, ClaimsPrincipal createdByUser);
    Task<UserLoginResponseDto?> LoginAsync(LoginDto loginDto);
}
