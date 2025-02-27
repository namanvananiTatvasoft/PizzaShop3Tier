using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class User
{
    public int Userid { get; set; }

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? Hashpassword { get; set; }

    public int Roleid { get; set; }

    public virtual ICollection<Permission> Permissions { get; } = new List<Permission>();

    public virtual ICollection<Userdetail> UserdetailCreatedbyNavigations { get; } = new List<Userdetail>();

    public virtual ICollection<Userdetail> UserdetailModifiedbyNavigations { get; } = new List<Userdetail>();

    public virtual ICollection<Userdetail> UserdetailUsers { get; } = new List<Userdetail>();
}
