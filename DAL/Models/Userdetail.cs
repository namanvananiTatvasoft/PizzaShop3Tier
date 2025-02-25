using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class Userdetail
{
    public int Id { get; set; }

    public int Userid { get; set; }

    public string Username { get; set; } = null!;

    public short Roleid { get; set; }

    public string Email { get; set; } = null!;

    public string? Phone { get; set; }

    public string? Photourl { get; set; }

    public string Firstname { get; set; } = null!;

    public string? Lastname { get; set; }

    public string? Address1 { get; set; }

    public short? Countryid { get; set; }

    public short? Stateid { get; set; }

    public int? Cityid { get; set; }

    public int Createdby { get; set; }

    public DateTime Createddate { get; set; }

    public int? Modifiedby { get; set; }

    public DateTime? Modifieddate { get; set; }

    public bool? Status { get; set; }

    public bool Isdeleted { get; set; }

    public int? Zipcode { get; set; }

    public virtual User CreatedbyNavigation { get; set; } = null!;

    public virtual User? ModifiedbyNavigation { get; set; }

    public virtual User User { get; set; } = null!;
}
