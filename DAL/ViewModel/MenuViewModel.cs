using System.ComponentModel.DataAnnotations;
using System.Numerics;
using Microsoft.AspNetCore.Http;

namespace DAL.ViewModel;

public class MenuViewModel
{
    public List<CategoryMenuModel> categoryList {get; set;}

    public ItemsViewMenuModel items {get; set;}

    public AddEditDeleteCategory categoryAdd {get; set;}

    // modifiers ----------------------------
    public List<ModifierGroupModel> modifierGroupList {get; set;}

    public ModifiersViewMenuModel modifiers {get; set;}
    
}

// all about category & item --------------------------------------
public class CategoryMenuModel
{
    public int Categoryid { get; set; }

    public string Categoryname { get; set; }

    public string Description { get; set; }
    
    // List of Items In Future

}

public class ItemsViewMenuModel
{
    public int Categoryid { get; set; }

    public int pageNumber {get; set;}

    public int pageSize {get; set;}

    public string searchKey {get; set;}

    public int count{get; set;}

    public List<SingleItem> itemList {get; set;}
}

public class SingleItem
{
    public int ItemId {get; set;}

    public string ItemName {get; set;}

    public bool ItemType {get; set;}

    public short Rate { get; set; }

    public int Quantity { get; set; }

    public bool Isavailable { get; set; }

    public string ImageUrl {get; set;}
}

public class AddEditDeleteCategory
{
    public string Categoryid {get; set;}

    public string Categoryname { get; set; }

    public string Description { get; set; }

    public int CreatedBy {get; set;}

    public int ModifiedBy {get; set;}

    public DateTime ModifiedDate{get; set;}

}

public class AddItemModel
{
    public int ItemId {get; set;}
    
    public string ItemName {get; set;}

    public int ItemCategory {get; set;}

    public bool ItemType {get; set;}

    public short ItemRate {get; set;}

    public short ItemQuantity {get; set;}

    public string ItemUnit {get; set;}

    public bool ItemAvailable {get; set;}

    public bool DefaultTax {get; set;}

    [Range(0, 999.99, ErrorMessage = "ItemRate must be between 0 and 999.99.")]
    public decimal ItemTaxPercentage {get; set;}

    public string ItemShortCode {get; set;}

    public string ItemDescription {get; set;}

    public IFormFile ItemImage {get; set;}

    public int CreatedBy {get; set;}

    public List<int> modGroupList {get; set;}
    
}


// all about Modifier Group & item --------------------------------------
public class ModifierGroupModel
{
    public int Modifiergroupid { get; set; }

    public string Modifiergroupname { get; set; }

    public string Description { get; set; }
}

public class ModifiersViewMenuModel
{
    public int Modgroupid { get; set; }

    public int Pagenumber {get; set;}

    public int Pagesize {get; set;}

    public string Searchkey {get; set;}

    public int Count{get; set;}

    public List<SingleModifier> modifiersList {get; set;}
}

public class SingleModifier
{
    public int ModifierId {get; set;}

    public string Modifiername {get; set;}

    public string Unit {get; set;}

    public short Rate { get; set; }

    public int Quantity { get; set; }

    public string ImageUrl {get; set;}
}
