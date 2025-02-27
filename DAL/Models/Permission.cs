using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class Permission
{
    public int Id { get; set; }

    public int Roleid { get; set; }

    public int Permissionid { get; set; }

    public bool Isenable { get; set; }

    public bool Canview { get; set; }

    public bool Canaddedit { get; set; }

    public bool Candelete { get; set; }

    public int? Modifiedby { get; set; }

    public DateTime? Modifieddate { get; set; }

    public virtual User? ModifiedbyNavigation { get; set; }

    public virtual Permissionlist PermissionNavigation { get; set; } = null!;

    public virtual Role Role { get; set; } = null!;
}
