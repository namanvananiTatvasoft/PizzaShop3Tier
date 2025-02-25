namespace BAL.Interfaces;

public interface IEmailServices
{
    public Task SendEmailAsync(string email, string subject, string message);
}
