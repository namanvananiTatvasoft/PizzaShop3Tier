using DAL.Models;
using DAL.ViewModel;

namespace BAL.Interfaces;

public interface IMenuServices
{
    public Item getItem(int itemId);
    public Task<List<CategoryMenuModel>> getCategories();

    public Task<ItemsViewMenuModel> getItems(int categoryId,int pageNumber,int pageSize,string searchKey);

    public (string, bool) addCategory(AddEditDeleteCategory model);

    public (string, bool) editCategory(AddEditDeleteCategory model);

    public void deleteCategory(AddEditDeleteCategory model);

    public (string, bool) addItem(AddItemModel model);

    public (string, bool) editItem(AddItemModel model);

    public void deleteItem(AddItemModel model);

    public void deleteItemCombine(List<int> itemList);

    public List<Modgroup> getModGroups();
}
