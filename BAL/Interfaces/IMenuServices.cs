using DAL.ViewModel;

namespace BAL.Interfaces;

public interface IMenuServices
{
    public Task<List<CategoryMenuModel>> getCategories();

    public Task<ItemsViewMenuModel> getItems(int categoryId,int pageNumber,int pageSize,string searchKey);

    public void addCategory(AddEditDeleteCategory model);

    public void editCategory(AddEditDeleteCategory model);

    public void deleteCategory(AddEditDeleteCategory model);
}
