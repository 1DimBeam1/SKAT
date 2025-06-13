using System;
using System.Collections.Generic;

namespace SKAT_Interface.Models.DbTaskModels;

public partial class Algorithmstep
{
    public int AlgoId { get; set; }

    public int AlgoStep { get; set; }

    public string Description { get; set; } = null!;

    public float Difficult { get; set; }

    public virtual Algorithm Algo { get; set; } = null!;

    public virtual ICollection<Trackedvariable> Trackedvariables { get; set; } = new List<Trackedvariable>();
}
