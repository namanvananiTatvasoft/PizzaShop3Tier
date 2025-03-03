using BAL.Interfaces;
using DAL.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace PizzaShop.Controllers;

public class MenuController : BaseDashboardController
{
    protected readonly IMenuServices _menuServices;
    protected readonly IAuthServices _auth;
    public MenuController(IJwtservices jwtservices, IAuthServices authServices, IMenuServices menuServices) : base(jwtservices, authServices)
    {
        _menuServices = menuServices;
        _auth = authServices;
    }

    [HttpGet]
    public async Task<IActionResult> MenuList()
    {
        MenuViewModel model = new MenuViewModel();
        model.categoryList = await _menuServices.getCategories();

        ViewData["Username"] = GetUserName();
        ViewBag.image = GetImgUrl();
        return View(model);
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult MenuListCategoryAdd(AddEditDeleteCategory categoryAdd)
    {
        categoryAdd.CreatedBy = _auth.getUser(GetUserName()).Userid;
        _menuServices.addCategory(categoryAdd);
        return RedirectToAction("MenuList");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult MenuListCategoryEdit(AddEditDeleteCategory categoryAdd)
    {
        categoryAdd.ModifiedBy = _auth.getUser(GetUserName()).Userid;
        _menuServices.editCategory(categoryAdd);
        
        return RedirectToAction("MenuList");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult MenuListCategoryDelete(AddEditDeleteCategory categoryAdd)
    {
        categoryAdd.ModifiedBy = _auth.getUser(GetUserName()).Userid;
        _menuServices.deleteCategory(categoryAdd);
        return RedirectToAction("MenuList");
    }

}
