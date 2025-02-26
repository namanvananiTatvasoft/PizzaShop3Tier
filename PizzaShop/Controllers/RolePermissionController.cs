using BAL.Interfaces;
using DAL.Models;
using Microsoft.AspNetCore.Mvc;
namespace PizzaShop.Controllers;

public class RolePermissionController : BaseDashboardController
{
    protected IAuthServices _authServices;
    public RolePermissionController(IJwtservices jwtservices, IAuthServices authServices) : base(jwtservices)
    {
        _authServices = authServices;
    }

    [HttpGet]
    public IActionResult RolesList()
    {
        ViewData["Username"] = GetUserName();
        List<Role> roles = _authServices.getRoles();
        return View(roles);
    }

    [HttpGet]
    public IActionResult Permissions()
    {
        ViewData["Username"] = GetUserName();
        return View();
    }
}
