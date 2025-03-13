using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class Itemmodifiergroupmap
{
    public int Id { get; set; }

    public int Itemid { get; set; }

    public int Modifiergroupid { get; set; }

    public int Min { get; set; }

    public int Max { get; set; }

    public virtual Item Item { get; set; } = null!;

    public virtual Modgroup Modifiergroup { get; set; } = null!;
}
