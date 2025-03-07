using System.Diagnostics;
using BAL.Interfaces;
using DAL.Database;
using DAL.Models;
using DAL.ViewModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;


namespace BAL.Services;

public class MenuServices : IMenuServices
{
    private readonly PizzaShopDbContext _db;
    private readonly IWebHostEnvironment _webHostEnvironment;


    public MenuServices(PizzaShopDbContext db, IWebHostEnvironment webHostEnvironment)
    {
        _db = db;
        _webHostEnvironment = webHostEnvironment;
    }

    public Item? getItem(int itemId)
    {
        return _db.Items.Where(e => e.Itemid == itemId).FirstOrDefault();
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
                    where itemstable.Categoryid == categoryId && !itemstable.Isdeleted &&(itemstable.Itemname.ToLower().Contains(searchKey.ToLower()) || itemstable.Description.ToLower().Contains(searchKey.ToLower()))
                    select new SingleItem
                    {
                        ItemId = itemstable.Itemid,
                        ItemName = itemstable.Itemname,
                        ItemType = (bool)itemstable.Itemtype,
                        Rate = itemstable.Rate,
                        Quantity = itemstable.Quantity,
                        Isavailable = itemstable.Isavailable,
                        ImageUrl = itemstable.Photourl

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

    public (string, bool) addCategory(AddEditDeleteCategory model)
    {
        Category category = new Category();

        if(_db.Categories.Where(e=>e.Categoryname == model.Categoryname).FirstOrDefault() != null)
        {
            return("Category Already Exists", false);
        }
        category.Categoryname = model.Categoryname;
        category.Description = model.Description;
        category.Createdby = model.CreatedBy;

        _db.Categories.Add(category);
        _db.SaveChanges();

        return("New Category is Added", true);
    }

    public (string, bool) editCategory(AddEditDeleteCategory model)
    {

        if(_db.Categories.Where(e=>e.Categoryid.ToString()!=model.Categoryid && e.Categoryname == model.Categoryname ).FirstOrDefault() != null)
        {
            return("Category Name Already Exists", false);
        }

        Category category = _db.Categories.Where(e => e.Categoryid.ToString() == model.Categoryid).FirstOrDefault();

        category.Categoryname = model.Categoryname;
        category.Description = model.Description;
        category.Modifiedby = model.ModifiedBy;
        category.Modifieddate = DateTime.Now;

        _db.SaveChanges();
        return ("Category Updated Succesfully", true);
    }

    public void deleteCategory(AddEditDeleteCategory model)
    {
        Category category = _db.Categories.Where(e => e.Categoryid.ToString() == model.Categoryid).FirstOrDefault();
        category.Modifiedby = model.ModifiedBy;
        category.Modifieddate = DateTime.Now;
        category.Isdeleted = true;

        List<Item> items = _db.Items.Where(e=>e.Categoryid.ToString() == model.Categoryid).ToList();
        foreach(var item in items)
        {
            item.Isdeleted = true;
        }

        _db.SaveChanges();
        return;
    }

    public (string, bool) addItem(AddItemModel model)
    {
        Item item = new Item();

        if(_db.Items.Where(e=>e.Itemname == model.ItemName).FirstOrDefault() != null)
        {
            return ("Item name already Exists", false);
        }

        item.Itemname = model.ItemName;
        item.Categoryid = model.ItemCategory;
        item.Itemtype = model.ItemType;
        item.Description = model.ItemDescription;
        item.Rate = model.ItemRate;
        item.Quantity = model.ItemQuantity;
        item.Unit = model.ItemUnit;
        item.Isavailable = model.ItemAvailable;
        item.Defaulttax = model.DefaultTax;
        item.Taxpercentage = model.ItemTaxPercentage;
        item.Shortcode = model.ItemShortCode;

        item.Createdby = model.CreatedBy;
        item.Createddate = DateTime.Now;

        _db.Items.Add(item);
        _db.SaveChanges();

        string imageURL = "";

        

        if(model.ItemImage != null)
        {
            // Save the image
            // var Itemname = model.ItemName;
            var ItemID = _db.Items.Where(e=>e.Itemname == model.ItemName).FirstOrDefault();
            var fileExtension = Path.GetExtension(model.ItemImage.FileName);

            var fileName = $"{ItemID.Itemid}{fileExtension}";
            var uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "images", "itemsPic");

            var filePath = Path.Combine(uploadPath, fileName);

            Console.WriteLine(filePath);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                model.ItemImage.CopyTo(fileStream);
            }

            imageURL = "/images/itemsPic/" + fileName;

            ItemID.Photourl = imageURL;
            _db.SaveChanges();

            Console.WriteLine(imageURL);

        }

        Item itemToBeAdded = _db.Items.Where(e=>e.Itemname == model.ItemName).FirstOrDefault();

        for (int i = 0; i < model.modGroupList.Count; i++)
        {
            _db.Itemmodifiergroupmaps.Add(
                new Itemmodifiergroupmap{
                    Itemid = itemToBeAdded.Itemid,
                    Modifiergroupid = model.modGroupList[i]
                }
            );
        }

        _db.SaveChanges();

        return ("Item Added Succesfully!", true);

    }

    public (string, bool) editItem(AddItemModel model)
    {

        if(_db.Items.Where(e=>e.Itemid!=model.ItemId && e.Itemname == model.ItemName).FirstOrDefault() != null)
        {
            return ("Item name already Exists", false);
        }
        Item item = _db.Items.Where(e=>e.Itemid == model.ItemId).FirstOrDefault();

        item.Itemname = model.ItemName;
        item.Categoryid = model.ItemCategory;
        item.Itemtype = model.ItemType;
        item.Description = model.ItemDescription;
        item.Rate = model.ItemRate;
        item.Quantity = model.ItemQuantity;
        item.Unit = model.ItemUnit;
        item.Isavailable = model.ItemAvailable;
        item.Defaulttax = model.DefaultTax;
        item.Taxpercentage = model.ItemTaxPercentage;
        item.Shortcode = model.ItemShortCode;

        item.Modifiedby = model.CreatedBy;
        item.Modifieddate = DateTime.Now;

        if(model.ItemImage != null)
        {
            var ItemID = model.ItemId;
            var fileExtension = Path.GetExtension(model.ItemImage.FileName);

            var fileName = $"{ItemID}{fileExtension}";
            var uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "images", "itemsPic");

            var filePath = Path.Combine(uploadPath, fileName);

            Console.WriteLine(filePath);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                model.ItemImage.CopyTo(fileStream);
            }

            var imageURL = "/images/itemsPic/" + fileName;
            item.Photourl = imageURL;

        }
        _db.SaveChanges();

        return ("Item Updated Succesfully!", true);

    }

    public void deleteItem(AddItemModel model)
    {
        Item item = _db.Items.Where(e=>e.Itemid == model.ItemId).FirstOrDefault();
        item.Modifiedby = model.CreatedBy;
        item.Modifieddate = DateTime.Now;
        item.Isdeleted = true;

        _db.SaveChanges();
        return;

    }

    public void deleteItemCombine(List<int> itemList)
    {
        for (int i = 0; i < itemList.Count; i++)
        {
            Item item = _db.Items.Where(e=>e.Itemid == itemList[i]).FirstOrDefault();
            item.Isdeleted = true;

        }

        _db.SaveChanges();
    }

    public List<Modgroup> getModGroups()
    {
        return _db.Modgroups.ToList();
    }

    public List<ModifierGroupModel> getModGroupsForList()
    {
        return _db.Modgroups.Select(mg => new ModifierGroupModel{Modifiergroupid = mg.Modgroupid, Modifiergroupname = mg.Modgroupname, Description = mg.Description}).ToList();
    }

    public ModifiersViewMenuModel getModifiersList(int categoryId,int pageNumber,int pageSize,string searchKey)
    {
        ModifiersViewMenuModel model = new ModifiersViewMenuModel();

        var query = from moditems in _db.Moditems
                    join map in _db.Moditemgroupmaps on moditems.Modifierid equals map.Modifierid
                    where map.Modgroupid == categoryId && !moditems.Isdeleted && (moditems.Modifiername.ToLower().Contains(searchKey.ToLower()) || moditems.Description.ToLower().Contains(searchKey.ToLower()))
                    select new SingleModifier
                    {
                        ModifierId = moditems.Modifierid,
                        Modifiername = moditems.Modifiername,
                        Unit = moditems.Unit,
                        Rate = moditems.Rate,
                        Quantity = moditems.Quantity,
                        ImageUrl = moditems.Photourl
                    };
            
        var count = query.Count();

        if (pageNumber < 1) pageNumber = 1;
        var totalPages = (int)Math.Ceiling((double)count / pageSize);
        if (pageNumber > totalPages) pageNumber = totalPages;
        if (pageNumber < 1) pageNumber = 1;

        model.Modgroupid = categoryId;
        model.Pagenumber = pageNumber;
        model.Pagesize = pageSize;
        model.Searchkey = searchKey;
        model.Count = count;
        model.modifiersList = query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();


        return model;
    }

}
