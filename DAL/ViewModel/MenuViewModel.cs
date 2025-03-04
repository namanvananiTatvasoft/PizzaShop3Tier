namespace DAL.ViewModel;

public class MenuViewModel
{
    public List<CategoryMenuModel> categoryList {get; set;}

    public ItemsViewMenuModel items {get; set;}

    public AddEditDeleteCategory categoryAdd {get; set;}
    
}


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

