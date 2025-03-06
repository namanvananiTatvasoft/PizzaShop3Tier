using System.Runtime.InteropServices.JavaScript;
using BAL.Interfaces;
using DAL.Models;
using DAL.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;

namespace PizzaShop.Controllers;

[Route("Menu")]
public class MenuController : BaseDashboardController
{
    protected readonly IMenuServices _menuServices;
    protected readonly IAuthServices _auth;
    public MenuController(IJwtservices jwtservices, IAuthServices authServices, IMenuServices menuServices) : base(jwtservices, authServices)
    {
        _menuServices = menuServices;
        _auth = authServices;
    }

    [HttpGet("MenuList")]
    public async Task<IActionResult> MenuList(int categoryId = 1, int pageNumber = 1, int pageSize = 5, string searchKey = "")
    {
        MenuViewModel model = new MenuViewModel();
        model.categoryList = await _menuServices.getCategories();
        
        model.items = await _menuServices.getItems(categoryId, pageNumber, pageSize, searchKey);


        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
        {
            return PartialView("_MenuListItems", model); // Return partial view for AJAX
        }
        return View(model);
    }

    [HttpGet("MenuListTable")]
    public async Task<IActionResult> MenuListTable(int categoryId = 1, int pageNumber = 1, int pageSize = 5, string searchKey = "")
    {
        MenuViewModel model = new MenuViewModel();
        model.categoryList = await _menuServices.getCategories();
        model.items = await _menuServices.getItems(categoryId, pageNumber, pageSize, searchKey);

        // ViewData["Username"] = GetUserName();
        // ViewBag.image = GetImgUrl();

        return PartialView("_MenuListItemsTable", model); // Return partial view for AJAX
    }


    [HttpPost("MenuListCategoryAdd")]
    public IActionResult MenuListCategoryAdd(AddEditDeleteCategory categoryAdd)
    {
        string msg;
        bool response;
        categoryAdd.CreatedBy = _auth.getUser(GetUserName()).Userid;
        (msg, response) = _menuServices.addCategory(categoryAdd);

        // if(!response) TempData["error"] = msg;
        // else TempData["success"] = msg;


        return Json( new { success = response, message = msg});
        
        // return RedirectToAction("MenuList");
    }


    [HttpPost("MenuListCategoryEdit")]
    public IActionResult MenuListCategoryEdit(AddEditDeleteCategory categoryAdd)
    {
        string msg;
        bool response;
        categoryAdd.ModifiedBy = _auth.getUser(GetUserName()).Userid;
        (msg, response) = _menuServices.editCategory(categoryAdd);

        // if(!response) TempData["error"] = msg;
        // else TempData["success"] = msg;

        return Json( new { success = response, message = msg});
        
        // return RedirectToAction("MenuList");
    }


    [HttpPost("MenuListCategoryDelete")]
    [ValidateAntiForgeryToken]
    public IActionResult MenuListCategoryDelete(AddEditDeleteCategory categoryAdd)
    {
        categoryAdd.ModifiedBy = _auth.getUser(GetUserName()).Userid;
        _menuServices.deleteCategory(categoryAdd);

        TempData["success"] = "Category Deleted Succesfully";
        return RedirectToAction("MenuList");
    }


    [HttpPost("MenuListItemAdd")]
    public IActionResult MenuListItemAdd(AddItemModel model)
    {
        string msg;
        bool response;

        model.CreatedBy = _auth.getUser(GetUserName()).Userid;
        (msg, response) = _menuServices.addItem(model);

        if(!response) TempData["error"] = msg;
        else TempData["success"] = msg;

        // return RedirectToAction("MenuList");
        // return Json( new { success = response, message = msg, urlToRedirect: $"/Menu/MenuList?categoryId={model.ItemCategory}" });
        return Json(new { success = response, message = msg, urlToRedirect = $"/Menu/MenuList?categoryId={model.ItemCategory}" });


    }


    [HttpGet("ItemDetailsForEdit")]
    public IActionResult ItemDetailsForEdit(int Itemid)
    {
        Item item = _menuServices.getItem(Itemid);
        return Json(item);
    }


    [HttpPost("MenuListItemEdit")]
    public IActionResult MenuListItemEdit(AddItemModel model)
    {

        string msg;
        bool response;

        model.CreatedBy = _auth.getUser(GetUserName()).Userid;
        (msg, response) = _menuServices.editItem(model);

        if(!response) TempData["error"] = msg;
        else TempData["success"] = msg;

        return RedirectToAction("MenuList", new { categoryId = model.ItemCategory });
    }


    [HttpPost("MenuListItemDelete")]
    public IActionResult MenuListItemDelete(AddItemModel model)
    {
        model.CreatedBy = _auth.getUser(GetUserName()).Userid;
        _menuServices.deleteItem(model);
        TempData["success"] = "Item is Deleted";
        return RedirectToAction("MenuList");
    }


    [HttpPost("MenuItemDeleteCombine")]
    public IActionResult MenuItemDeleteCombine(List<int> itemList)
    {
        for (int i = 0; i < itemList.Count; i++)
        {
            Console.WriteLine(itemList[i]);
        }

        _menuServices.deleteItemCombine(itemList);
        return Json( new { success = true, message = "hi"});
    }

    [HttpGet("getModifierNames")]
    public IActionResult getModifierNames()
    {
        List<Modgroup> modList = _menuServices.getModGroups();
        return Json(new {modGroupList = modList});
    }
}
