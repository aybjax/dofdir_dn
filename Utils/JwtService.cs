using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using dofdir_komek.Extensions;
using dofdir_komek.Models;
using dofdir_komek.Repository;
using Microsoft.IdentityModel.Tokens;

namespace dofdir_komek.Utils;

public class JwtService
{
    private JwtData? _jwtDataCache = default;
    
    public const int EXPIRATION_MINUTE = 30;
    private readonly IConfiguration _configuration;
    private readonly IUserRespository _userRepository;

    public JwtService(IConfiguration configuration, IUserRespository userRepository)
    {
        _configuration = configuration;
        _userRepository = userRepository;
    }

    public JwtData? GetJwtData(string jwt)
    {
        if (_jwtDataCache is not null)
        {
            return _jwtDataCache;
        }
        
        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(jwt);
        
        var dataJson = token.Claims.FirstOrDefault(el => el.Type == "data")?.Value;

        if (string.IsNullOrEmpty(dataJson)) return null;

        var data = JwtData.FromJson(dataJson);

        if (data is not null)
        {
            _jwtDataCache = data;
        }

        return data;
    }

    public async Task<string?> JwtToken(string email, string password)
    {
        User? user = await _userRepository.GetUserByCredentialsAsync(email, password);

        if (user is null) return null;
        
        var issuer = _configuration["Jwt:Issuer"];
        var audience = _configuration["Jwt:Audience"];
        var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);
        var signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(key),
            SecurityAlgorithms.HmacSha512Signature
        );

        var data = new JwtData(
            user.Id,
            user.Email,
            user.Name,
            user.Role
        );

        var claims = new List<Claim>()
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Name),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim("roles", user.Role),
            new Claim("data", data.ToJson())
        };

        var subject = new ClaimsIdentity(claims);

        var expires = DateTime.UtcNow.AddMinutes(EXPIRATION_MINUTE);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = subject,
            Expires = expires,
            Issuer = issuer,
            Audience = audience,
            SigningCredentials = signingCredentials,
        };
        
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var jwtToken = tokenHandler.WriteToken(token);
        
        return jwtToken;
    }


    public record class JwtData(
        int Id,
        string Email,
        string Name,
        string Role
    )
    {
        public string ToJson()
        {
            return JsonSerializer.Serialize(this, JwtDataContext.Default.JwtData);
        }

        public static JwtData? FromJson(string json)
        {
            return JsonSerializer.Deserialize<JwtData>(json, JwtDataContext.Default.JwtData);
        }
    }
}
