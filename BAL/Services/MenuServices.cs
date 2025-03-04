using System.Diagnostics;
using BAL.Interfaces;
using DAL.Database;
using DAL.Models;
using DAL.ViewModel;
using Microsoft.EntityFrameworkCore;

namespace BAL.Services;

public class MenuServices : IMenuServices
{
    private readonly PizzaShopDbContext _db;

    public MenuServices(PizzaShopDbContext db)
    {
        _db = db;
    }

    public async Task<List<CategoryMenuModel>> getCategories()
    {
        IQueryable<CategoryMenuModel> query = from category in _db.Categories
                                        where category.Isdeleted == false
                                        orderby category.Categoryid
                                        select new CategoryMenuModel
                                        {
                                            Categoryid = category.Categoryid,
                                            Categoryname = category.Categoryname,
                                            Description = category.Description ?? string.Empty
                                        };

        return await query.ToListAsync();
    }

    public async Task<ItemsViewMenuModel> getItems(int categoryId,int pageNumber,int pageSize,string searchKey)
    {
        ItemsViewMenuModel model = new ItemsViewMenuModel();


        
        var query = from itemstable in _db.Items
                    where itemstable.Categoryid == categoryId && (itemstable.Itemname.ToLower().Contains(searchKey.ToLower()) || itemstable.Description.ToLower().Contains(searchKey.ToLower()))
                    select new SingleItem
                    {
                        ItemId = itemstable.Itemid,
                        ItemName = itemstable.Itemname,
                        ItemType = (bool)itemstable.Itemtype,
                        Rate = itemstable.Rate,
                        Quantity = itemstable.Quantity,
                        Isavailable = itemstable.Isavailable,

                    };
        
        var count = await query.CountAsync();


        if (pageNumber < 1) pageNumber = 1;
        
        var totalPages = (int)Math.Ceiling((double)count / pageSize);

        if (pageNumber > totalPages) pageNumber = totalPages;
        if (pageNumber < 1) pageNumber = 1;


        
        model.Categoryid = categoryId;
        model.pageNumber = pageNumber;
        model.pageSize = pageSize;
        model.searchKey = searchKey;
        model.count = count;
        model.itemList = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

        return model;
    }

    public void addCategory(AddEditDeleteCategory model)
    {
        Category category = new Category();
        category.Categoryname = model.Categoryname;
        category.Description = model.Description;
        category.Createdby = model.CreatedBy;

        _db.Categories.Add(category);
        _db.SaveChanges();
        return;
    }

    public void editCategory(AddEditDeleteCategory model)
    {
        Category category = _db.Categories.Where(e => e.Categoryid.ToString() == model.Categoryid).FirstOrDefault();
        category.Categoryname = model.Categoryname;
        category.Description = model.Description;
        category.Modifiedby = model.ModifiedBy;
        category.Modifieddate = DateTime.Now;

        _db.SaveChanges();
        return;
    }

    public void deleteCategory(AddEditDeleteCategory model)
    {
        Category category = _db.Categories.Where(e => e.Categoryid.ToString() == model.Categoryid).FirstOrDefault();
        category.Modifiedby = model.ModifiedBy;
        category.Modifieddate = DateTime.Now;
        category.Isdeleted = true;

        _db.SaveChanges();
        return;
    }


}
