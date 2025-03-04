using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class Item
{
    public int Itemid { get; set; }

    public string Itemname { get; set; } = null!;

    public int Categoryid { get; set; }

    public bool? Itemtype { get; set; }

    public string? Description { get; set; }

    public short Rate { get; set; }

    public int Quantity { get; set; }

    public string Unit { get; set; } = null!;

    public bool Isavailable { get; set; }

    public bool Defaulttax { get; set; }

    public decimal Taxpercentage { get; set; }

    public string? Shortcode { get; set; }

    public int Createdby { get; set; }

    public DateTime Createddate { get; set; }

    public int? Modifiedby { get; set; }

    public DateTime? Modifieddate { get; set; }

    public string? Photourl { get; set; }

    public bool Isdeleted { get; set; }

    public virtual Category Category { get; set; } = null!;

    public virtual User CreatedbyNavigation { get; set; } = null!;

    public virtual User? ModifiedbyNavigation { get; set; }
}
