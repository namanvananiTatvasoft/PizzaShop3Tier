using DAL.ViewModel;

namespace BAL.Interfaces;

public interface IDashServices
{
    public UpdateUserDetails MapObject(string Email);
    public void UpdateDB(UpdateUserDetails objPass);
}
