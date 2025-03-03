using DAL.ViewModel;

namespace BAL.Interfaces;

public interface IMenuServices
{
    public Task<List<CategoryMenuModel>> getCategories();

    public void addCategory(AddEditDeleteCategory model);

    public void editCategory(AddEditDeleteCategory model);

    public void deleteCategory(AddEditDeleteCategory model);
}
