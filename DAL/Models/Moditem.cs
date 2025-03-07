using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class Moditem
{
    public int Modifierid { get; set; }

    public string Modifiername { get; set; } = null!;

    public string? Description { get; set; }

    public string Unit { get; set; } = null!;

    public short Rate { get; set; }

    public int Quantity { get; set; }

    public int Createdby { get; set; }

    public DateTime Createddate { get; set; }

    public int? Modifiedby { get; set; }

    public DateTime? Modifieddate { get; set; }

    public string? Photourl { get; set; }

    public bool Isdeleted { get; set; }

    public virtual ICollection<Moditemgroupmap> Moditemgroupmaps { get; } = new List<Moditemgroupmap>();
}
