using System;
using System.Collections.Generic;

namespace SKAT_Interface.Models.DbTaskModels;

public partial class Trackedvariable
{
    public int Sequence { get; set; }

    public int LineNumber { get; set; }

    public string VarType { get; set; } = null!;

    public string VarName { get; set; } = null!;

    public int AlgoStep { get; set; }

    public int AlgoId { get; set; }

    public virtual Algorithmstep Algorithmstep { get; set; } = null!;
}
