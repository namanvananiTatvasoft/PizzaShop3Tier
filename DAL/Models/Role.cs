using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class Role
{
    public int Roleid { get; set; }

    public string Rolename { get; set; } = null!;
}
