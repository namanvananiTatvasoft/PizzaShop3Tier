using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class Modgroup
{
    public int Modgroupid { get; set; }

    public string Modgroupname { get; set; } = null!;

    public string? Description { get; set; }

    public int Createdby { get; set; }

    public DateTime Createddate { get; set; }

    public int? Modifiedby { get; set; }

    public DateTime? Modifieddate { get; set; }

    public bool Isdeleted { get; set; }

    public virtual User CreatedbyNavigation { get; set; } = null!;

    public virtual ICollection<Itemmodifiergroupmap> Itemmodifiergroupmaps { get; } = new List<Itemmodifiergroupmap>();

    public virtual User? ModifiedbyNavigation { get; set; }

    public virtual ICollection<Moditemgroupmap> Moditemgroupmaps { get; } = new List<Moditemgroupmap>();
}
