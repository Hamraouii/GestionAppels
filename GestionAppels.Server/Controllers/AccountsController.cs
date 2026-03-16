using GestionAppels.Server.Dtos;
using GestionAppels.Server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestionAppels.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountsController : ControllerBase
{
    private readonly IAccountService _accountService;

    public AccountsController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    [HttpPost]
    [AllowAnonymous]
    // [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateUser(UserCreateDto userCreateDto)
    {
        var newUser = await _accountService.CreateUserAsync(userCreateDto, User);

        if (newUser == null)
        {
            return BadRequest("A user with this email address already exists.");
        }

        return CreatedAtAction(nameof(CreateUser), new { id = newUser.Id }, newUser);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginDto loginDto)
    {
        var loginResponse = await _accountService.LoginAsync(loginDto);

        if (loginResponse == null)
        {
            return Unauthorized("Invalid email or password.");
        }

        return Ok(loginResponse);
    }
}
