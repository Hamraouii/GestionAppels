using System.ComponentModel.DataAnnotations;

namespace GestionAppels.Server.Models;

public class User : BaseEntity
{
    [Required]
    [StringLength(50)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string EmailAddress { get; set; } = string.Empty;

    public byte[] PasswordHash { get; set; } = [];
    public byte[] PasswordSalt { get; set; } = [];

    public Role Role { get; set; }

    //public Guid ServiceId { get; set; }
    //public virtual Service Service { get; set; } = null!;
}
