using GestionAppels.Server.Models;

namespace GestionAppels.Server.Services;

public interface ITokenService
{
    string CreateToken(User user);
}
