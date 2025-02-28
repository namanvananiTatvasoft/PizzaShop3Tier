using BAL.Interfaces;
using DAL.Database;
using DAL.ViewModel;
using Microsoft.AspNetCore.Hosting;

namespace BAL.Services;

public class DashServices : IDashServices
{
    private readonly PizzaShopDbContext _db;
    private readonly IHashServices _hashServices;
    private readonly IAuthServices _auth;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public DashServices(PizzaShopDbContext db, IHashServices hashServices, IAuthServices auth, IWebHostEnvironment webHostEnvironment)
    {
        _db = db;
        _hashServices = hashServices;
        _auth = auth;
        _webHostEnvironment = webHostEnvironment;
    }
    public UpdateUserDetails MapObject(string Email)
    {
        var obj = _auth.getUserDetails(Email);

        var ojbPass = new UpdateUserDetails{
            Email = obj.Email,
            Username = obj.Username,
            Phone = obj.Phone,
            Firstname = obj.Firstname,
            Lastname = obj.Lastname,
            Address1 = obj.Address1,
            Zipcode = obj.Zipcode,
            CountryId = (int)obj.Countryid,
            StateId = (int)obj.Stateid,
            CityId = (int)obj.Cityid,   
            CountryList = _db.Countries.ToList(),
            StateList = _db.States.ToList(),
            CityList = _db.Cities.ToList(),
            photoUrl = obj.Photourl,
        };

        return ojbPass;
    }
    

    public void UpdateDB(UpdateUserDetails objPass)
    {
        var obj = _auth.getUserDetails(objPass.Email);

        if(objPass.profileImg != null)
        {
            // Save the image
            var customerId = obj.Userid;
            var fileExtension = Path.GetExtension(objPass.profileImg.FileName);

            var fileName = $"{customerId}{fileExtension}";
            var uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "images", "profilePic");

            var filePath = Path.Combine(uploadPath, fileName);

            Console.WriteLine(filePath);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                objPass.profileImg.CopyTo(fileStream);
            }

            obj.Photourl = "/images/profilePic/" + fileName;

        }

        obj.Firstname = objPass.Firstname;
        obj.Lastname = objPass.Lastname;    
        obj.Username = objPass.Username;
        obj.Phone = objPass.Phone;  
        obj.Address1 = objPass.Address1;
        obj.Zipcode = objPass.Zipcode;
        obj.Countryid = (short)objPass.CountryId;
        obj.Stateid = (short)objPass.StateId;
        obj.Cityid = objPass.CityId;

        _db.SaveChanges();

        return;
    }
}
