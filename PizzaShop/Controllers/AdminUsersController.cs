using BAL.Interfaces;
using DAL.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace PizzaShop.Controllers;

public class AdminUsersController : BaseDashboardController
{
    protected readonly IAdminUsersServices _adminUsersServices;
    protected readonly IAuthServices _auth;
    public AdminUsersController(IJwtservices jwtservices, IAdminUsersServices adminUsersServices, IAuthServices auth) : base(jwtservices)
    {
        _adminUsersServices = adminUsersServices;
        _auth = auth;
    }


        // Dashboard **************************************** get Method ******************
    public async Task<IActionResult> UserListAll(int PageSize = 5, int PageNumber = 1, string SortColumn = "Firstname", string SortDirection = "asc", string SearchKey = "")
    {
        var (users,count,pageSize, pageNumber, sortColumn, sortDirection, searchKey) = await _adminUsersServices.getDynamicUserList(PageSize, PageNumber, SortColumn, SortDirection, SearchKey);
        
        ViewBag.PageSize = pageSize;
        ViewBag.PageNumber = pageNumber;
        ViewBag.Count = count;
        ViewBag.SortColumn = sortColumn;
        ViewBag.SortDirection = sortDirection;
        ViewBag.SearchKey = searchKey;
        ViewBag.Active = "Users";
        ViewData["Username"] = GetUserName();

        return View(users);
    }

    // Add User **************************************** get Method ******************
    public async Task<IActionResult> AddUser()
    {
        ViewData["Username"] = GetUserName();
        return View();
    }

    // Add User ****** post Method ******
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddUser(AddUserModel user)
    {
        if (ModelState.IsValid)
        {
            await _auth.AddUserToDB(user, GetUserName());

            var subject = "Welcome to Pizza Shop";
            // var message = "Welcome to Pizza Shop. Your account has been created successfully. Your username is " + user.Username + " and password is " + user.Password + ". Please login to your account and change your password.";


            await _adminUsersServices.SendEmailToNewUser(user.Email, subject, user.Username, user.Password);
            
            
            return RedirectToAction("UserListAll");
        }

        return View();
    }


    // Edit Page **************************************** get Method ******************
    [HttpGet]
    public async Task<IActionResult> EditUser(string email)
    {
        var user = _auth.getUserDetails(email);
        var model = new EditUserModel
        {
            Firstname = user.Firstname,
            Lastname = user.Lastname,
            Email = user.Email,
            Username = user.Username,
            Roleid = (int)user.Roleid,
            Phone = user.Phone,
            Countryid = (int)user.Countryid,
            Stateid = (int)user.Stateid,
            Cityid = (int)user.Cityid,
            Zipcode = (int)user.Zipcode,
            Address1 = user.Address1,
            Status = (bool)user.Status,
            CountryList = _auth.getCountries(),
            StateList = _auth.getStates(-1),
            CityList = _auth.getCities(-1),
        };

        ViewData["Username"] = GetUserName();
        return View(model);
    }

    // Edit Page ********** Post Method *********
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditUser(EditUserModel model)
    {
        await _auth.UpdateUserToDB(model, GetUserName());

        ViewData["Username"] = GetUserName();
        return RedirectToAction("EditUser", new {email = model.Email});
    }

    
    public async Task<IActionResult> DeleteUser(string email)
    {
        await _auth.DeleteUser(email);

        return RedirectToAction("UserListAll");
    }
















    [HttpGet]
    public IActionResult GetCountries()
    {
        var countries = _auth.getCountries();
        return Json(countries.Select(c => new { countryId = c.CountryId, countryName = c.CountryName }));
    }

    [HttpGet]
    public IActionResult GetStates(int countryId)
    {
        var states = _auth.getStates(countryId);
        return Json(states.Select(s => new { stateId = s.StateId, stateName = s.StateName }));
    }
    // Get cities based on the selected State
    [HttpGet]
    public IActionResult GetCities(int stateId)
    {
        var cities = _auth.getCities(stateId);
        return Json(cities.Select(c => new { cityId = c.CityId, cityName = c.CityName }));
    }

}
