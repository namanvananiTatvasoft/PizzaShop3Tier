using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

using Microsoft.Extensions.Configuration;
using BAL.Interfaces;

public class JwtServices : IJwtservices
{
    private readonly string _secretKey = "6f2a1d2f9e56b0a7c92d7a3b1f054f7f7bdbf0f9c9d86bfeeb4973171f8fc4f6"; // Ideally, store in appsettings.json
    private readonly string _issuer = "myIssuer";
    private readonly string _audience = "myAudience";
    
    public string GenerateJwtToken(string Email, string Role, int expireMinutes = 60){
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, Email),
            new Claim(ClaimTypes.Role, Role),
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey?? string.Empty));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: DateTime.Now.AddMinutes(expireMinutes),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string getEmailDetailsFromToken(string Token)
    {
        var handler = new JwtSecurityTokenHandler();
        if(Token==null){
            return "nothing";
        }
        var jwtToken = handler.ReadJwtToken(Token);

        var email = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
        return email;
    }

    public string getRoleDetailsFromToken(string Token)
    {
        var handler = new JwtSecurityTokenHandler();
        if(Token==null){
            return "nothing";
        }
        var jwtToken = handler.ReadJwtToken(Token);

        var role = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
        return role;
    }

    public bool isTokenExpired(string Token)
    {
        var handler = new JwtSecurityTokenHandler();
        if(Token==null){
            return true;
        }
        var jwtToken = handler.ReadJwtToken(Token);

        if(jwtToken.ValidTo < DateTime.UtcNow){
            return true;
        }
        return false;
    }
}

