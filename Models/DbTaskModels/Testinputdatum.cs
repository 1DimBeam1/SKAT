using System;
using System.Collections.Generic;

namespace SKAT_Interface.Models.DbTaskModels;

public partial class Testinputdatum
{
    public int TestId { get; set; }

    public string VarName { get; set; } = null!;

    public string VarValue { get; set; } = null!;

    public string VarType { get; set; } = null!;

    public int LineNumber { get; set; }
}
