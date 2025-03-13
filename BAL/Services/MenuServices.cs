using System.Diagnostics;
using BAL.Interfaces;
using DAL.Database;
using DAL.Models;
using DAL.ViewModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Org.BouncyCastle.Asn1.Misc;
using Microsoft.Extensions.Options;


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

    public AddItemModel? getItem(int itemId)
    {
        Item item = _db.Items.Where(e => e.Itemid == itemId).FirstOrDefault();
        AddItemModel model = new AddItemModel();
        model.ItemId = item.Itemid;
        model.ItemName = item.Itemname;
        model.ItemCategory = item.Categoryid;
        model.ItemType = (bool)item.Itemtype;
        model.DefaultTax = item.Defaulttax;
        model.ItemAvailable = item.Isavailable;
        model.ItemQuantity = (short)item.Quantity;
        model.ItemRate = item.Rate;
        model.ItemTaxPercentage = item.Taxpercentage;
        model.ItemShortCode = item.Shortcode;
        model.ItemDescription = item.Description;


    
        return model;
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
                    Modifiergroupid = model.modGroupList[i].modGroupId,
                    Min = model.modGroupList[i].Min,
                    Max = model.modGroupList[i].Max
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
        return _db.Modgroups.OrderBy(m=>m.Modgroupid).ToList();
    }

    public List<ModifierGroupModel> getModGroupsForList()
    {
        return _db.Modgroups
            .Where(mg => mg.Isdeleted == false)  // Filter where isDeleted is false
            .OrderBy(mg => mg.Modgroupid)
            .Select(mg => new ModifierGroupModel
            {
                Modifiergroupid = mg.Modgroupid,
                Modifiergroupname = mg.Modgroupname,
                Description = mg.Description
            })
            .ToList();
    }

    public ModifiersViewMenuModel getModifiersList(int categoryId,int pageNumber,int pageSize,string searchKey)
    {

        ModifiersViewMenuModel model = new ModifiersViewMenuModel();

        var query = default(IQueryable<SingleModifier>);

        if(categoryId == -1)
        {
            query = from moditems in _db.Moditems
            where !moditems.Isdeleted && (moditems.Modifiername.ToLower().Contains(searchKey.ToLower()) || moditems.Description.ToLower().Contains(searchKey.ToLower()))
            select new SingleModifier
            {
                ModifierId = moditems.Modifierid,
                Modifiername = moditems.Modifiername,
                Unit = moditems.Unit,
                Rate = moditems.Rate,
                Quantity = moditems.Quantity,
                ImageUrl = moditems.Photourl
            };

        }else
        {
            query = from moditems in _db.Moditems
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
        }


            
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


    public void addModGroup(AddEditDeleteModGroup model)
    {
        Modgroup group = _db.Modgroups.Where(e=>e.Modgroupname == model.Modgroupname).FirstOrDefault();

        if(group != null)
        {
            Console.WriteLine("Mod Group Already Exist");
        }

        _db.Modgroups.Add(new Modgroup
        {
            Modgroupname = model.Modgroupname,
            Description = model.Description,
            Createdby = model.CreatedBy,
            Createddate = DateTime.Now
        });

        _db.SaveChanges();

        group = _db.Modgroups.Where(e=>e.Modgroupname == model.Modgroupname).FirstOrDefault();

        foreach(var modifierid in model.Modifiersidlist)
        {
            _db.Moditemgroupmaps.Add(new Moditemgroupmap
            {
                Modgroupid = group.Modgroupid,
                Modifierid = modifierid
            });
        }

        _db.SaveChanges();


    }

    public AddEditDeleteModGroup getModGroupValuesForEdit(int modifierGroupId)
    {
        AddEditDeleteModGroup model = new AddEditDeleteModGroup();

        Modgroup modgroup = _db.Modgroups.Where(e=>e.Modgroupid == modifierGroupId).FirstOrDefault();
        if(modgroup == null) return model;

        model.Modgroupid = modgroup?.Modgroupid.ToString() ?? string.Empty;
        model.Modgroupname = modgroup.Modgroupname;
        model.Description = modgroup.Description;


        // model.Modifiersidlist = (from maptable in _db.Moditemgroupmaps
        //                         where maptable.Modgroupid == modifierGroupId
        //                         select maptable.Modifierid).ToList();

        // model.Modifierslist = (from moditems in _db.Moditems
        //                        join maptable in _db.Moditemgroupmaps on moditems.Modifierid equals maptable.Modifierid
        //                        where maptable.Modgroupid == modifierGroupId
        //                        select moditems.Modifiername).ToList();

        var result = (from maptable in _db.Moditemgroupmaps
                      join moditems in _db.Moditems on maptable.Modifierid equals moditems.Modifierid
                      where maptable.Modgroupid == modifierGroupId
                      select new 
                      {
                          Modifierid = maptable.Modifierid,
                          Modifiername = moditems.Modifiername
                      }).ToList();

        model.Modifiersidlist = result.Select(r => r.Modifierid).ToList();
        model.Modifierslist = result.Select(r => r.Modifiername).ToList();
    

        return model;
    }

    public void editModGroup(AddEditDeleteModGroup model)
    {

        if (!int.TryParse(model.Modgroupid, out int modGroupId))
        {
            // Optionally, log the error or return early
            return;
        }

        Modgroup group = _db.Modgroups.Where(e=>e.Modgroupid == int.Parse(model.Modgroupid)).FirstOrDefault();

        if(group == null)
        {
            return;
        }

        group.Modgroupname = model.Modgroupname;
        group.Description = model.Description;
        group.Modifiedby = model.CreatedBy;
        group.Modifieddate = DateTime.Now;

        var itemsToRemove = _db.Moditemgroupmaps.Where(e=>e.Modgroupid == int.Parse(model.Modgroupid)).ToList();

        _db.Moditemgroupmaps.RemoveRange(itemsToRemove);


        if (model.Modifiersidlist != null && model.Modifiersidlist.Any())
        {
            var newMappings = model.Modifiersidlist.Select(modifierid => new Moditemgroupmap
            {
                Modgroupid = group.Modgroupid,
                Modifierid = modifierid
            }).ToList();

            _db.Moditemgroupmaps.AddRange(newMappings);
        }

        _db.SaveChanges();


    }


    public void deleteModGroup(int deleteModGroup, int modifiedBy)
    {
        Modgroup group = _db.Modgroups.Where(e=>e.Modgroupid == deleteModGroup).FirstOrDefault();
        group.Isdeleted = true;
        group.Modifiedby = modifiedBy;
        group.Modifieddate = DateTime.Now;

        var itemsToRemove = _db.Moditemgroupmaps.Where(e=>e.Modgroupid == deleteModGroup).ToList();
        _db.Moditemgroupmaps.RemoveRange(itemsToRemove);

        _db.SaveChanges();
    }

    public (string, bool) addModifier(AddEditDeleteModifiers model)
    {

        Moditem moditem = _db.Moditems.Where(e=>e.Modifiername == model.Modifiername).FirstOrDefault();
        if(moditem != null)
        {
            return ("Modifier Name Already Exists", false);
        }

        _db.Moditems.Add(new Moditem {
            Modifiername = model.Modifiername,
            Description = model.Description,
            Unit = model.Unit,
            Rate = model.Rate,
            Quantity = model.Quantity,
            Createdby = model.ModifiedBy,
            Createddate = DateTime.Now
        });

        _db.SaveChanges();

        var itemId = _db.Moditems.Where(e=>e.Modifiername == model.Modifiername).FirstOrDefault().Modifierid;

        foreach(var modGroupid in model.ModifiersGroupList)
        {
            _db.Moditemgroupmaps.Add(new Moditemgroupmap{
                Modifierid = itemId,
                Modgroupid = int.Parse(modGroupid)
            });
        }

        _db.SaveChanges();

        return ("Added Modifier Succesfully", true);
    }

    public (string, bool) editModifier(AddEditDeleteModifiers model)
    {

        Moditem moditem = _db.Moditems.Where(e=>e.Modifiername == model.Modifiername && e.Modifierid != model.ModifierId).FirstOrDefault();
        if(moditem != null)
        {
            return ("Modifier Name Already Exists", false);
        }

        Moditem item = _db.Moditems.Where(e=>e.Modifierid == model.ModifierId).FirstOrDefault();

        item.Modifiername = model.Modifiername;
        item.Description = model.Description;
        item.Unit = model.Unit;
        item.Rate = model.Rate;
        item.Quantity = model.Quantity;
        item.Modifiedby = model.ModifiedBy;
        item.Modifieddate = DateTime.Now;

        
        var itemsToRemove = _db.Moditemgroupmaps.Where(e=>e.Modifierid == model.ModifierId).ToList();
        _db.Moditemgroupmaps.RemoveRange(itemsToRemove);


        if (model.ModifiersGroupList != null && model.ModifiersGroupList.Any())
        {
            var newMappings = model.ModifiersGroupList.Select(modifierGroupid => new Moditemgroupmap
            {
                Modgroupid = int.Parse(modifierGroupid),
                Modifierid = model.ModifierId
            }).ToList();

            _db.Moditemgroupmaps.AddRange(newMappings);
        }


        // _db.SaveChanges();

        // foreach(var modGroupid in model.ModifiersGroupList)
        // {
        //     _db.Moditemgroupmaps.Add(new Moditemgroupmap{
        //         Modifierid = model.ModifierId,
        //         Modgroupid = int.Parse(modGroupid)
        //     });
        // }

        _db.SaveChanges();

        return ("Updated Modifier Succesfully", true);
    }

    public AddEditDeleteModifiers getModifierValuesForEdit(int modifierid)
    {
        AddEditDeleteModifiers model = new AddEditDeleteModifiers();
        
        Moditem item = _db.Moditems.Where(e=>e.Modifierid == modifierid).FirstOrDefault();
        model.ModifierId = item.Modifierid;
        model.Unit = item.Unit;
        model.Quantity = item.Quantity;
        model.Description = item.Description;
        model.Modifiername = item.Modifiername;
        model.Rate = item.Rate;

        model.ModifiersGroupListIds = _db.Moditemgroupmaps.Where(e=>e.Modifierid == modifierid).Select(e=>e.Modgroupid).ToList();

        return model;
    }


    public void deleteModItemGroupMap(int modifierItemId,int modifierGroupId)
    {
        Moditemgroupmap? item = _db.Moditemgroupmaps.FirstOrDefault(e=>e.Modifierid == modifierItemId && e.Modgroupid == modifierGroupId);
        if (item != null)
        {
            _db.RemoveRange(new List<Moditemgroupmap> { item });
        }
        _db.SaveChanges();
    }


    public ModGroupDetails getModGroupDetails(int modGroupId)
    {
        Modgroup? group = _db.Modgroups.Where(e=>e.Modgroupid == modGroupId).FirstOrDefault();
        ModGroupDetails model = new ModGroupDetails();
        if (group != null)
        {
            model.Id = group.Modgroupid;
            model.GroupName = group.Modgroupname;
            model.Items = (from m in _db.Moditems
                         join map in _db.Moditemgroupmaps on m.Modifierid equals map.Modifierid
                         where map.Modgroupid == modGroupId
                         select new itemForList{
                            ItemName = m.Modifiername,
                            Rate = m.Rate
                         }).ToList();
        }
    
        return model;
    }

}
