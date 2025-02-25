using DAL.Models;
using DAL.ViewModel;

namespace BAL.Interfaces;

public interface IAuthServices
{
    public User getUser(string email);
    public Userdetail getUserDetails(string email);
    public Task AddUserToDB(AddUserModel model, string createdByEmail);
    public Task UpdateUserToDB(EditUserModel model, string updatedByEmail);
    public Task DeleteUser(string email);
    public List<Country> getCountries();
    public List<State> getStates(int countryId);
    public List<City> getCities(int stateId);

    public Role getRole(int roleid);
    public Task UpdatePassword(string email, string password);
    public bool checkPassword(string password, string hashPassword);
    public Task SendResetPasswordEmail(string Email, string uriii);
}
