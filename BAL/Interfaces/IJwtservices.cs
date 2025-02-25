namespace BAL.Interfaces;

public interface IJwtservices
{
    public string GenerateJwtToken(string Email, string Role);

    public string getEmailDetailsFromToken(string Token);
    
    public string getRoleDetailsFromToken(string Token);
}
