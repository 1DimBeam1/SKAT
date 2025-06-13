using System;
using System.Collections.Generic;

namespace SKAT_Interface.Models.DbUsersModels;

public partial class VariablesSolutionsByProgram
{
    public int ProgramStep { get; set; }

    public int ProgramLineNumber { get; set; }

    public int OrderNumber { get; set; }

    public int TestId { get; set; }

    public string VarName { get; set; } = null!;

    public string VarValue { get; set; } = null!;
}
