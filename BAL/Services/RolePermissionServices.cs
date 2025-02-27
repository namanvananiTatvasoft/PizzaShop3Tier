using DAL.ViewModel;
using BAL.Interfaces;
using DAL.Models;
using DAL.Database;
namespace BAL.Services;


public class RolePermissionServices : IRolePermissionServices
{
    protected PizzaShopDbContext _db;
    public RolePermissionServices(PizzaShopDbContext db)
    {
        _db = db;
    }

    public PermissionModel GetPermissionModel(int roleid)
    {
        Role role = _db.Roles.Where(a => a.Roleid == roleid).FirstOrDefault();

        PermissionModel ans = new PermissionModel(); // Initialize 'ans' variable
        ans.RoleId = role.Roleid;
        ans.RoleName = role.Rolename;

        ans.Permissions = (from permission in _db.Permissions
                           join permissionList in _db.Permissionlists on permission.Permissionid equals permissionList.Permissionid
                           where permission.Roleid == roleid
                           select new PermissionToggles
                           {
                                PermissionId = permission.Permissionid,
                                PermissionName = permissionList.Permissionname,
                                IsEnable = permission.Isenable,
                                CanView = permission.Canview,
                                CanAddEdit = permission.Canaddedit,
                                CanDelete = permission.Candelete
                           }).ToList();
        
        return ans;
    }


    public void UpdatePermissions(PermissionModel model)
    {
        var permissionsToChange = _db.Permissions.Where(a => a.Roleid == model.RoleId).ToList();

        foreach (PermissionToggles permission in model.Permissions)
        {
            Permission permissionEntity = permissionsToChange.Where(a => a.Permissionid == permission.PermissionId).FirstOrDefault();

            permissionEntity.Isenable = permission.IsEnable;
            permissionEntity.Canview = permission.CanView;
            permissionEntity.Canaddedit = permission.CanAddEdit;
            permissionEntity.Candelete = permission.CanDelete;

        }

        _db.SaveChanges();

    }
}
