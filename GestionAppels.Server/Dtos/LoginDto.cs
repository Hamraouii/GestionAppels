using System.ComponentModel.DataAnnotations;

namespace GestionAppels.Server.Dtos;

public record LoginDto([Required, EmailAddress] string EmailAddress, [Required] string Password);
