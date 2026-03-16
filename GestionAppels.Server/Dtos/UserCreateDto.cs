using System.ComponentModel.DataAnnotations;
using GestionAppels.Server.Models;

namespace GestionAppels.Server.Dtos;

public record UserCreateDto(
    [Required, StringLength(50)] string FirstName,
    [Required, StringLength(50)] string LastName,
    [Required, EmailAddress] string EmailAddress,
    [Required, MinLength(8)] string Password,
    [Required] Role Role
);
