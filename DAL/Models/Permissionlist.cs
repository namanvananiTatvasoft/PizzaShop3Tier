using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class Permissionlist
{
    public int Permissionid { get; set; }

    public string Permissionname { get; set; } = null!;

    public virtual ICollection<Permission> Permissions { get; } = new List<Permission>();
}
