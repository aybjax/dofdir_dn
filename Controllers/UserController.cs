using dofdir_komek.Models;
using dofdir_komek.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using dofdir_komek.Utils;

namespace dofdir_komek.Controllers;

[ApiController]
[Route("api")]
[Authorize]
public class UserController : ControllerBase
{
    private readonly JwtService _jwtService;
    private readonly IUserRespository _userRespository;
    
    public UserController(JwtService jwtService, IUserRespository userRespository)
    {
        this._jwtService = jwtService;
        this._userRespository = userRespository;
    }

    [HttpGet("me")]
    public async Task<IActionResult> Test()
    {
        if (!Request.Headers.TryGetValue("Authorization", out var accessToken))
        {
            return Unauthorized();
        }

        var l = accessToken.ToString().Split(' ');
        if (l.Length < 2) return Unauthorized();

        var dataStr = l[1];
        var data = _jwtService.GetJwtData(dataStr);

        if (data is null) return Unauthorized();

        return Ok(data);
    }

    [AllowAnonymous]
    [HttpPost("login", Name = "login")]
    public async Task<IActionResult> Login([FromBody] LoginDto data)
    {
        string email = data.Email ?? throw new ArgumentNullException(nameof(data.Email));
        string passwordUnhashed = data.Password ?? throw new ArgumentNullException(nameof(data.Password));
        string password = ArgoHasher.Hash(passwordUnhashed);
        
        string? token = await _jwtService.JwtToken(email, password);

        if (token is null)
        {
            return BadRequest(new
            {
                Error = "User not found"
            });
        }

        return Ok(new
        {
            Message = "ok",
            Token = token,
        });
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto data)
    {
        var user = new User
        {
            Name = data.Name ?? throw new ArgumentNullException(nameof(data.Name)),
            Email = data.Email ?? throw new ArgumentNullException(nameof(data.Email)),
            Password = data.Hashed(),
            Role = "SUPER_ADMIN",
        };

        var isSuccessful = await _userRespository.SaveUserAsync(user);

        if (!isSuccessful) return BadRequest(new
        {
            Error = "User already exists"
        });

        return Ok(new
        {
            Message = "ok",
        });
    }
}


public record LoginDto
{
    [Required] public string? Email { get; set; }
    [Required] public string? Password { get; set; }
}

public record RegisterDto
{
    [Required]
    public string? Name { get; set; }
    
    [Required]
    [EmailAddress]
    public string? Email { get; set; }

    [Required]
    [MinLength(8)]
    [Compare("Repeated")]
    public string? Password { get; set; }

    [MinLength(8)]
    [Compare("Password")]
    [Required] public string? Repeated { get; set; }

    public string Hashed()
    {
        return Password is null ? throw new ArgumentException(nameof(Password)) : ArgoHasher.Hash(Password);
    }
}
