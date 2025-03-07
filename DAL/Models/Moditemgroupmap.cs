using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class Moditemgroupmap
{
    public int Id { get; set; }

    public int Modifierid { get; set; }

    public int Modgroupid { get; set; }

    public virtual Modgroup Modgroup { get; set; } = null!;

    public virtual Moditem Modifier { get; set; } = null!;
}
