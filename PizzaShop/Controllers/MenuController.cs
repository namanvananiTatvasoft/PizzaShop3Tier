using BAL.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace PizzaShop.Controllers;

public class MenuController : BaseDashboardController
{
    public MenuController(IJwtservices jwtservices, IAuthServices authServices) : base(jwtservices, authServices)
    {

    }

    public IActionResult MenuList()
    {
        ViewData["Username"] = GetUserName();
        ViewBag.image = GetImgUrl();
        return View();
    }


}
