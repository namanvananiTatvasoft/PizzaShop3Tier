using BAL.Interfaces;
using DAL.Database;
using DAL.Models;
using DAL.ViewModel;

namespace BAL.Services;

public class AuthServices: IAuthServices
{
    private readonly PizzaShopDbContext _db;
    private readonly IHashServices _hashServices;

    private readonly IEmailServices _emailServices;

    public AuthServices(PizzaShopDbContext db, IHashServices hashServices, IEmailServices emailServices)
    {
        _db = db;
        _hashServices = hashServices;
        _emailServices = emailServices;
    }

    public User getUser(string email)
    {
        return _db.Users.Where(a => a.Email == email).FirstOrDefault() ?? new User();
    }

    public Userdetail getUserDetails(string email)
    {
        return _db.Userdetails.Where(a => a.Email == email).FirstOrDefault() ?? new Userdetail();
    }

    public async Task AddUserToDB(AddUserModel model, string createdByEmail)
    {
        var user = new User
        {
            Email = model.Email,
            Password = model.Password,
            Hashpassword = _hashServices.HashPassword(model.Password),
            Roleid = model.Roleid,
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        var createdByUser = getUser(createdByEmail);
        var ourUser = getUser(model.Email);

        var Userdetails = new Userdetail
        {
            Userid = ourUser.Userid,
            Firstname = model.Firstname,
            Lastname = model.Lastname,
            Email = model.Email,
            Username = model.Username,
            Phone = model.Phone,
            Countryid = (short?)model.Countryid,
            Stateid = (short?)model.Stateid,
            Cityid = model.Cityid,
            Zipcode = model.Zipcode,
            Address1 = model.Address1,
            Createdby = createdByUser.Userid,
            Roleid = (short)model.Roleid,
        };

        _db.Userdetails.Add(Userdetails);
        await _db.SaveChangesAsync();

        return;
    }

    public async Task UpdateUserToDB(EditUserModel model, string updatedByEmail)
    {
        var user = getUserDetails(model.Email);
        var updatedByUser = getUser(updatedByEmail);

        user.Firstname = model.Firstname;
        user.Lastname = model.Lastname;
        user.Email = model.Email;
        user.Username = model.Username;
        user.Phone = model.Phone;
        user.Countryid = (short?)model.Countryid;
        user.Stateid = (short?)model.Stateid;
        user.Cityid = model.Cityid;
        user.Zipcode = model.Zipcode;
        user.Address1 = model.Address1;
        user.Status = (bool)model.Status;
        user.Roleid = (short)model.Roleid;
        user.Modifiedby = updatedByUser.Userid;
        user.Modifieddate = DateTime.Now;

        _db.Userdetails.Update(user);
        await _db.SaveChangesAsync();

        return;
    }

    public async Task DeleteUser(string email)
    {
        var user = getUserDetails(email);
        user.Isdeleted = true;
        _db.Userdetails.Update(user);

        await _db.SaveChangesAsync();

        return;
    }



    public List<Country> getCountries()
    {
        return _db.Countries.ToList();
    }

    public List<State> getStates(int countryId = -1)
    {
        if(countryId == -1)
        {
            return _db.States.ToList();
        }
        return _db.States.Where(a => a.CountryId == countryId).ToList();
    }

    public List<City> getCities(int stateId = -1)
    {   
        if(stateId == -1)
        {
            return _db.Cities.ToList();
        }
        return _db.Cities.Where(a => a.StateId == stateId).ToList();
    }

    public Role getRole(int roleid)
    {
        return _db.Roles.Where(a => a.Roleid == roleid).FirstOrDefault() ?? new Role();
    }

    public async Task UpdatePassword(string email, string newPassword)
    {
        var user = getUser(email);
        user.Password = newPassword;
        user.Hashpassword = _hashServices.HashPassword(newPassword);
        await _db.SaveChangesAsync();
    }

    public bool checkPassword(string password, string hashPassword)
    {
        if(string.IsNullOrEmpty(password))
        {
            return false;
        }
        return hashPassword == _hashServices.HashPassword(password);
    }

    public async Task SendResetPasswordEmail(string Email, string uriii)
    {
        string sendString = @$"
            <div style='height: 80px; display: flex; justify-content: center; align-items: center; font-size: 30px; background-color: rgb(45, 134, 134); color: white;'>
                    PIZZASHOP
            </div>

            <div style='margin-top: 20px; font-size: 20px;'>
                Please Click <a href={uriii} style='text-decoration:underline'>here</a> for reset your Account Password
            </div>
            <div style='margin-top: 20px; font-size: 20px;'>
                if you encounter any issue or have any question, please do not hesitate to contact our support team.
            </div>
            <div style='margin-top: 20px; font-size: 20px;'>
                <span style='color: yellowgreen;'>Importatnt Note</span> For Security reasons, the link will expire in 24 hours. if you did not request a password reset, please ignoer this email or contact our support team immediately.
            </div>
        ";
        string subject = "Reset Password";
        await _emailServices.SendEmailAsync(Email, subject, sendString);

        return;
    }

}
