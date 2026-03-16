using GestionAppels.Server.Models;

namespace GestionAppels.Server.Dtos;

public record UserViewDto(Guid Id, string FirstName, string LastName, string EmailAddress, Role Role, DateTime CreatedAt);
