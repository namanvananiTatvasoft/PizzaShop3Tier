using BAL.Interfaces;
using DAL.Models;
using DAL.ViewModel;
using Microsoft.AspNetCore.Mvc;
namespace PizzaShop.Controllers;

public class RolePermissionController : BaseDashboardController
{
    protected IAuthServices _authServices;
    protected IRolePermissionServices _rolePermissionServices;
    public RolePermissionController(IJwtservices jwtservices, IAuthServices authServices, IRolePermissionServices rolePermissionServices) : base(jwtservices, authServices)
    {
        _authServices = authServices;
        _rolePermissionServices = rolePermissionServices;
    }

    [HttpGet]
    public IActionResult RolesList()
    {
        // ViewData["Username"] = GetUserName();
        List<Role> roles = _authServices.getRoles();
        // ViewBag.image = GetImgUrl();
        return View(roles);
    }

    [HttpGet]
    public IActionResult Permissions(int roleid)
    {
        PermissionModel model = _rolePermissionServices.GetPermissionModel(roleid);

        // ViewData["Username"] = GetUserName();
        // ViewBag.image = GetImgUrl();

        return View(model);
    }

    [HttpPost]
    public IActionResult Permissions(PermissionModel model)
    {
        _rolePermissionServices.UpdatePermissions(model);

        // ViewData["Username"] = GetUserName();
        // ViewBag.image = GetImgUrl();
        
        TempData["success"] = "Permissions Updated Succesfully !";
        return View(model);
    }
}
