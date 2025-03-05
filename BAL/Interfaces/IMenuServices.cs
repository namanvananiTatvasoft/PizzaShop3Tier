using DAL.Models;
using DAL.ViewModel;

namespace BAL.Interfaces;

public interface IMenuServices
{
    public Item getItem(int itemId);
    public Task<List<CategoryMenuModel>> getCategories();

    public Task<ItemsViewMenuModel> getItems(int categoryId,int pageNumber,int pageSize,string searchKey);

    public void addCategory(AddEditDeleteCategory model);

    public void editCategory(AddEditDeleteCategory model);

    public void deleteCategory(AddEditDeleteCategory model);

    public void addItem(AddItemModel model);

    public void editItem(AddItemModel model);

    public void deleteItem(AddItemModel model);
}
