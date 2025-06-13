using System;
using System.Collections.Generic;

namespace SKAT_Interface.Models.DbTaskModels;

public partial class Test
{
    public int TestId { get; set; }

    public int AlgoId { get; set; }

    public string Description { get; set; } = null!;

    public string TestName { get; set; } = null!;

    public float Difficult { get; set; }

    public int SolvedCount { get; set; }

    public int UnsolvedCount { get; set; }

    public virtual Algorithm Algo { get; set; } = null!;
}
