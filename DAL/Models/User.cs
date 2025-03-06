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

    public virtual ICollection<Category> CategoryCreatedbyNavigations { get; } = new List<Category>();

    public virtual ICollection<Category> CategoryModifiedbyNavigations { get; } = new List<Category>();

    public virtual ICollection<Item> ItemCreatedbyNavigations { get; } = new List<Item>();

    public virtual ICollection<Item> ItemModifiedbyNavigations { get; } = new List<Item>();

    public virtual ICollection<Modgroup> ModgroupCreatedbyNavigations { get; } = new List<Modgroup>();

    public virtual ICollection<Modgroup> ModgroupModifiedbyNavigations { get; } = new List<Modgroup>();

    public virtual ICollection<Permission> Permissions { get; } = new List<Permission>();

    public virtual ICollection<Userdetail> UserdetailCreatedbyNavigations { get; } = new List<Userdetail>();

    public virtual ICollection<Userdetail> UserdetailModifiedbyNavigations { get; } = new List<Userdetail>();

    public virtual ICollection<Userdetail> UserdetailUsers { get; } = new List<Userdetail>();
}
