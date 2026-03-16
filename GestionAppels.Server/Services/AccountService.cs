using GestionAppels.Server.Data;
using GestionAppels.Server.Dtos;
using GestionAppels.Server.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace GestionAppels.Server.Services;

public class AccountService : IAccountService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<AccountService> _logger;
    private readonly ITokenService _tokenService;

    public AccountService(ApplicationDbContext context, ILogger<AccountService> logger, ITokenService tokenService)
    {
        _context = context;
        _logger = logger;
        _tokenService = tokenService;
    }

    public async Task<UserViewDto?> CreateUserAsync(UserCreateDto userCreateDto, ClaimsPrincipal createdByUser)
    {
        if (await _context.Users.AnyAsync(u => u.EmailAddress == userCreateDto.EmailAddress))
        {
            _logger.LogWarning("Attempted to create a user with an existing email: {Email}", userCreateDto.EmailAddress);
            return null;
        }

        Guid creatorIdToSet;
        var newUserId = Guid.NewGuid(); // Generate ID for the new user upfront

        // Get the ID of the user performing the creation, if authenticated
        string? authenticatedUserIdString = createdByUser.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!string.IsNullOrEmpty(authenticatedUserIdString) && Guid.TryParse(authenticatedUserIdString, out Guid parsedAuthenticatedUserId))
        {
            creatorIdToSet = parsedAuthenticatedUserId;
            _logger.LogInformation("User creation initiated by authenticated user: {CreatorId}", creatorIdToSet);
        }
        else
        {
            // This handles anonymous creation (e.g., first admin user) or if the claim is unexpectedly missing.
            // The new user will be marked as self-created.
            _logger.LogWarning("CreateUserAsync called by an unauthenticated user or with a missing/invalid NameIdentifier claim. The new user will be set as its own creator.");
            creatorIdToSet = newUserId; // New user is its own creator
        }

        CreatePasswordHash(userCreateDto.Password, out byte[] passwordHash, out byte[] passwordSalt);

        var user = new User
        {
            Id = newUserId, // Assign the pre-generated Id
            FirstName = userCreateDto.FirstName,
            LastName = userCreateDto.LastName,
            EmailAddress = userCreateDto.EmailAddress,
            Role = userCreateDto.Role,
            PasswordHash = passwordHash,
            PasswordSalt = passwordSalt,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = creatorIdToSet // Use the determined creatorId
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        _logger.LogInformation("User {Email} created successfully by {CreatorId}", user.EmailAddress, creatorIdToSet);

        return new UserViewDto(user.Id, user.FirstName, user.LastName, user.EmailAddress, user.Role, user.CreatedAt);
    }

    public async Task<UserLoginResponseDto?> LoginAsync(LoginDto loginDto)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.EmailAddress == loginDto.EmailAddress);

        if (user == null || !VerifyPasswordHash(loginDto.Password, user.PasswordHash, user.PasswordSalt))
        {
            _logger.LogWarning("Login failed for {EmailAddress}", loginDto.EmailAddress);
            return null; // Invalid credentials
        }

        var token = _tokenService.CreateToken(user);

        var userView = new UserViewDto(user.Id, user.FirstName, user.LastName, user.EmailAddress, user.Role, user.CreatedAt);

        _logger.LogInformation("User {EmailAddress} logged in successfully.", user.EmailAddress);

        return new UserLoginResponseDto(userView, token);
    }

    private static bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
    {
        ArgumentNullException.ThrowIfNull(password);
        if (storedHash.Length != 64) throw new ArgumentException("Invalid length of password hash (64 bytes expected).", nameof(storedHash));
        if (storedSalt.Length != 128) throw new ArgumentException("Invalid length of password salt (128 bytes expected).", nameof(storedSalt));

        using var hmac = new HMACSHA512(storedSalt);
        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        for (int i = 0; i < computedHash.Length; i++)
        {
            if (computedHash[i] != storedHash[i]) return false;
        }

        return true;
    }

    private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
    {
        using var hmac = new HMACSHA512();
        passwordSalt = hmac.Key;
        passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
    }
}
