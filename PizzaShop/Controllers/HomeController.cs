using System.Diagnostics;
using BAL.Interfaces;
using BAL.Services;
using DAL.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using PizzaShop.Models;

namespace PizzaShop.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IAuthServices _auth;  
    private readonly IJwtservices _jwtServices; 
    private readonly IEmailServices _emailServices; 

    public HomeController(ILogger<HomeController> logger, IAuthServices auth, IJwtservices jwtServices, IEmailServices emailServices)
    {
        _logger = logger;
        _auth = auth;
        _jwtServices = jwtServices;
        _emailServices = emailServices;
    }

    public IActionResult Index()
    {    
        if(string.IsNullOrEmpty(Request.Cookies["AuthToken"])){
            return RedirectToAction("Home", "Login");
        }else{
            return RedirectToAction("Dash", "Dashboard");
        }
        return View();
    }


    //***************************** Get  Login *******************
    [HttpGet]
    public IActionResult Login()
    {
        if(!string.IsNullOrEmpty(Request.Cookies["AuthToken"])){
            return RedirectToAction("Dash", "Dashboard");
        }
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Login(LoginViewModel objUser)
    {
        Console.WriteLine(objUser.RememberMe);

        if (ModelState.IsValid)
        {
            var obj = _auth.getUser(objUser.Email);
            if(obj.Email == null)
            {
                ModelState.AddModelError("Email", "Email is Incorrect");
            }
            else if(_auth.checkPassword(objUser.Password, obj.Hashpassword))
            {

                var roleObj =  _auth.getRole(obj.Roleid);
                var token = _jwtServices.GenerateJwtToken(obj.Email, roleObj.Rolename);

                if(objUser.RememberMe)
                {
                    Response.Cookies.Append("AuthToken", token, new CookieOptions
                    {
                        Expires = DateTime.UtcNow.AddDays(7),
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.Strict
                    });
                }else{
                    Response.Cookies.Append("AuthToken", token, new CookieOptions
                    {
                        Expires = DateTime.UtcNow.AddHours(1),
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.Strict
                    });
                }
                
                Response.Cookies.Append("UserName", objUser.Email, new CookieOptions
                {
                    Expires = DateTime.UtcNow.AddDays(7),
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict

                });
                
                return RedirectToAction("Index", "Home");
            }
            else
                ModelState.AddModelError("Password", "Password is Incorrect");
        }
            
        return View();
    }



        //************************************************************** ForgotPassword Page ************************************************************
    [HttpGet]
    public IActionResult ForgotPassword(string Email)
    {
        if (!string.IsNullOrEmpty(Email))
        {
            ViewData["Email"] = Email;
        }else{
            ViewData["Email"] = "";
        }
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordEmail obj)
    {

        if(!ModelState.IsValid)
        {
            ModelState.AddModelError(string.Empty, "Email is Invalid");
            return View();
        }

        var user = _auth.getUser(obj.Email);

        if(user.Email != null){

            string uriii = Url.Action("ResetPassword", "Home", new { Email = obj.Email }, Request.Scheme);
            await _auth.SendResetPasswordEmail(obj.Email, uriii);

            ViewData["Message"] = "Reset Password Link is send to your Email";
        }else{
            ViewData["Message"] = "Email does not exist";
        }

        return View(obj);
    }

    //************************************************************** Reset Page ************************************************************
    [HttpGet]
    public IActionResult ResetPassword(string Email)
    {
        ViewData["Email"] = Email;
        ViewData["Message"] = null;
        Console.WriteLine(Email);
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(ResetPasswordModel obj)
    {

        if(ModelState.IsValid)
        {
            await _auth.UpdatePassword(obj.Email, obj.NewPassword);
            ViewData["Message"] = "Password Updated Succesfully";
            ViewData["Email"] = obj.Email;
            return View();
        }

        
        ViewData["Email"] = obj.Email;
        return View("ResetPassword", obj);
    }





    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
