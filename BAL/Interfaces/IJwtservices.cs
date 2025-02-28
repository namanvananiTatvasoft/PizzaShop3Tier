namespace BAL.Interfaces;

public interface IJwtservices
{
    public string GenerateJwtToken(string Email, string Role, int expireMinutes = 60);

    public string getEmailDetailsFromToken(string Token);
    
    public string getRoleDetailsFromToken(string Token);

    public bool isTokenExpired(string Token);
}
