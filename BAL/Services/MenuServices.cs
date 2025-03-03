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
