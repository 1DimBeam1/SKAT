using System;
using System.Collections.Generic;

namespace SKAT_Interface.Models.DbTaskModels;

public partial class Algorithm
{
    public int AlgoId { get; set; }

    public string SrcPath { get; set; } = null!;

    public string AlgoName { get; set; } = null!;

    public string PicPath { get; set; } = null!;

    public virtual ICollection<Algorithmstep> Algorithmsteps { get; set; } = new List<Algorithmstep>();

    public virtual ICollection<Test> Tests { get; set; } = new List<Test>();
}
