using System.Net.Http.Headers;
using BAL.Interfaces;
using DAL.Database;
using DAL.Models;
using DAL.ViewModel;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;

namespace BAL.Services;

public class AuthServices: IAuthServices
{
    private readonly PizzaShopDbContext _db;
    private readonly IHashServices _hashServices;
    private readonly IEmailServices _emailServices;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public AuthServices(PizzaShopDbContext db, IHashServices hashServices, IEmailServices emailServices, IWebHostEnvironment webHostEnvironment)
    {
        _db = db;
        _hashServices = hashServices;
        _emailServices = emailServices;
        _webHostEnvironment = webHostEnvironment;
    }

    #region get table values
    public User getUser(string email)
    {
        return _db.Users.Where(a => a.Email == email).FirstOrDefault() ?? new User();
    }

    public Userdetail getUserDetails(string email)
    {
        return _db.Userdetails.Where(a => a.Email == email).FirstOrDefault() ?? new Userdetail();
    }

    public string getImageUrl(string email)
    {
        return _db.Userdetails.Where(a => a.Email == email).Select(a => a.Photourl).FirstOrDefault() ?? "";
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

    public List<Role> getRoles()
    {
        return _db.Roles.ToList();
    }

    public Role getRole(int roleid)
    {
        return _db.Roles.Where(a => a.Roleid == roleid).FirstOrDefault() ?? new Role();
    }
    #endregion



    #region check values
    public bool checkPassword(string password, string hashPassword)
    {
        if(string.IsNullOrEmpty(password))
        {
            return false;
        }
        return hashPassword == _hashServices.HashPassword(password);
    }

    public async Task<(string, bool)> checkUsernameEmailPhone(string email, string phone, string username, bool isEdit = false)
    {
        Userdetail user;
        
        if(isEdit) user = await _db.Userdetails.Where(a =>  a.Email != email && (a.Phone == phone || a.Username == username)).FirstOrDefaultAsync();
        else user = await _db.Userdetails.Where(a => a.Email == email || a.Phone == phone || a.Username == username).FirstOrDefaultAsync();
        

        if(user != null)
        {
            if(!isEdit && user.Email == email) return ("Email already Exist", true);
            
            if(user.Phone == phone) return ("Phone already Exist", true);
            
            if(user.Username == username) return ("Username already Exist", true);
            
        }
        return ("All are Unique", false);
    }


    #endregion


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

        string imageURL = "";

        if(model.imageFile != null)
        {
            // Save the image
            var customerId = user.Userid;
            var fileExtension = Path.GetExtension(model.imageFile.FileName);

            var fileName = $"{customerId}{fileExtension}";
            var uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "images", "profilePic");

            var filePath = Path.Combine(uploadPath, fileName);

            Console.WriteLine(filePath);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                model.imageFile.CopyTo(fileStream);
            }

            imageURL = "/images/profilePic/" + fileName;

        }

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
            Photourl = imageURL
        };

        _db.Userdetails.Add(Userdetails);
        await _db.SaveChangesAsync();

        return;
    }

    public async Task UpdateUserToDB(EditUserModel model, string updatedByEmail)
    {
        var user = getUserDetails(model.Email);
        var updatedByUser = getUser(updatedByEmail);

        string imageURL = "";

        if(model.imageFile != null)
        {
            // Save the image
            var customerId = user.Userid;
            var fileExtension = Path.GetExtension(model.imageFile.FileName);

            var fileName = $"{customerId}{fileExtension}";
            var uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "images", "profilePic");

            var filePath = Path.Combine(uploadPath, fileName);

            Console.WriteLine(filePath);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                model.imageFile.CopyTo(fileStream);
            }

            imageURL = "/images/profilePic/" + fileName;

        }



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

        if(imageURL != "")
            user.Photourl = imageURL;

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
    
    public async Task UpdatePassword(string email, string newPassword)
    {
        var user = getUser(email);
        user.Password = newPassword;
        user.Hashpassword = _hashServices.HashPassword(newPassword);
        await _db.SaveChangesAsync();
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
