namespace DAL.ViewModel;

public class MenuViewModel
{
    public List<CategoryMenuModel> categoryList {get; set;}
    public AddEditDeleteCategory categoryAdd {get; set;}
    
}


public class CategoryMenuModel
{
    public int Categoryid { get; set; }

    public string Categoryname { get; set; }

    public string Description { get; set; }
    
    // List of Items In Future

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
