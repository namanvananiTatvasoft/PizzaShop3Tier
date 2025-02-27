using BAL.Interfaces;
using DAL.Models;
using DAL.ViewModel;
using Microsoft.AspNetCore.Mvc;
namespace PizzaShop.Controllers;

public class RolePermissionController : BaseDashboardController
{
    protected IAuthServices _authServices;
    protected IRolePermissionServices _rolePermissionServices;
    public RolePermissionController(IJwtservices jwtservices, IAuthServices authServices, IRolePermissionServices rolePermissionServices) : base(jwtservices)
    {
        _authServices = authServices;
        _rolePermissionServices = rolePermissionServices;
    }

    [HttpGet]
    public IActionResult RolesList()
    {
        ViewData["Username"] = GetUserName();
        List<Role> roles = _authServices.getRoles();
        return View(roles);
    }

    [HttpGet]
    public IActionResult Permissions(int roleid)
    {
        PermissionModel model = _rolePermissionServices.GetPermissionModel(roleid);

        ViewData["Username"] = GetUserName();
        return View(model);
    }

    [HttpPost]
    public IActionResult Permissions(PermissionModel model)
    {
        _rolePermissionServices.UpdatePermissions(model);

        ViewData["Username"] = GetUserName();
        return View(model);
    }
}
