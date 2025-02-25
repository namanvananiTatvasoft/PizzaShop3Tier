using BAL.Interfaces;
using DAL.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace PizzaShop.Controllers;

public class DashboardController : BaseDashboardController
{
    protected readonly IDashServices _dash;
    protected readonly IAuthServices _auth;

    public DashboardController(IJwtservices jwtservices, IDashServices dash, IAuthServices auth) : base(jwtservices)
    {
        _dash = dash;
        _auth = auth;
    }

    // Dashboard **************************************** get Method ******************
    public IActionResult Dash()
    {
        ViewData["UserName"] = GetUserName();
        ViewBag.Active = "Dashboard";
        return View();
    }

    // My Profile **************************************** get Method ******************
    [HttpGet]
    public IActionResult MyProfile()
    {
        var ojbPass = _dash.MapObject(GetUserName());
        ViewData["UserName"] = GetUserName();
        ViewData["Role"] = GetRole();
        ViewBag.Active = "Dashboard";

        return View(ojbPass);
    }

    // My Profile
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult MyProfile(UpdateUserDetails objPass)
    {
        _dash.UpdateDB(objPass);
        ViewBag.Active = "Dashboard";
        return RedirectToAction("MyProfile");
    }

        // Change Password **************************************** get Method ******************
    [HttpGet]
    public IActionResult ChangePassword()
    {
        ViewBag.Active = "Dashboard";
        ViewData["UserName"] = GetUserName();

        return View();
    }
    // Change Password **************************************** Post Method ******************
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(UpdatePassword model)
    {
        if(ModelState.IsValid){
            var email = GetUserName();
            var obj = _auth.getUser(email);

            if(_auth.checkPassword(model.OldPassword, obj.Hashpassword)){
                await _auth.UpdatePassword(email, model.NewPassword);
                ModelState.AddModelError("Wrong Password", "Successfully Change Password");
                return View();
            }else{
                ModelState.AddModelError("Wrong Password", "Invalid Old Password");
            }
        }
        ViewBag.Active = "Dashboard";
        ViewData["UserName"] = GetUserName();
        return View();
    }


    // Logout **********************************************************
    public IActionResult LogOut(){
        Response.Cookies.Delete("AuthToken");
        Response.Cookies.Delete("UserName");
        ViewBag.Active = "Dashboard";
        return RedirectToAction("Login", "Home");
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
