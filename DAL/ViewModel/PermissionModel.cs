namespace DAL.ViewModel;

public class PermissionToggles
{
    public int PermissionId { get; set; }
    public string PermissionName { get; set; }

    // Enable/Disable for permission
    public bool IsEnable { get; set; }
    public bool CanView { get; set; }
    public bool CanAddEdit { get; set; }
    public bool CanDelete { get; set; }
}

public class PermissionModel
{
    public int RoleId { get; set; }
    public string RoleName { get; set; }

    // permissions for specific role
    public List<PermissionToggles> Permissions { get; set; }
}

