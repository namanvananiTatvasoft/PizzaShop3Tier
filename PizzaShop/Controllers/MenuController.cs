using BAL.Interfaces;
using DAL.Models;
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
    public async Task<IActionResult> MenuList(int categoryId = 1, int pageNumber = 1, int pageSize = 5, string searchKey = "")
    {
        MenuViewModel model = new MenuViewModel();
        model.categoryList = await _menuServices.getCategories();
        model.items = await _menuServices.getItems(categoryId, pageNumber, pageSize, searchKey);

        // ViewData["Username"] = GetUserName();
        // ViewBag.image = GetImgUrl();

        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
        {
            return PartialView("_MenuListItems", model); // Return partial view for AJAX
        }
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> MenuListTable(int categoryId = 1, int pageNumber = 1, int pageSize = 5, string searchKey = "")
    {
        MenuViewModel model = new MenuViewModel();
        model.categoryList = await _menuServices.getCategories();
        model.items = await _menuServices.getItems(categoryId, pageNumber, pageSize, searchKey);

        // ViewData["Username"] = GetUserName();
        // ViewBag.image = GetImgUrl();

        return PartialView("_MenuListItemsTable", model); // Return partial view for AJAX
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

    [HttpPost]
    public IActionResult MenuListItemAdd(AddItemModel model)
    {
        model.CreatedBy = _auth.getUser(GetUserName()).Userid;
        _menuServices.addItem(model);
        return RedirectToAction("MenuList");
    }

    [HttpGet]
    public IActionResult ItemDetailsForEdit(int Itemid)
    {
        Item item = _menuServices.getItem(Itemid);
        return Json(item);
    }

    [HttpPost]
    public IActionResult MenuListItemEdit(AddItemModel model)
    {
        model.CreatedBy = _auth.getUser(GetUserName()).Userid;
        _menuServices.editItem(model);
        Console.WriteLine(model.ItemId);
        return RedirectToAction("MenuList");
    }

    [HttpPost]
    public IActionResult MenuListItemDelete(AddItemModel model)
    {
        model.CreatedBy = _auth.getUser(GetUserName()).Userid;
        _menuServices.deleteItem(model);
        TempData["success"] = "Item is Deleted";
        return RedirectToAction("MenuList");
    }

}
