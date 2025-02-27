using DAL.ViewModel;

namespace BAL.Interfaces;
public interface IRolePermissionServices
{
    public PermissionModel GetPermissionModel(int roleid);

    public void UpdatePermissions(PermissionModel model);
}
