using System;
using System.Collections.Generic;

namespace SKAT_Interface.Models.DbUsersModels;

public partial class Grade
{
    public int UserId { get; set; }

    public int SessionId { get; set; }

    public float Mark { get; set; }

    public DateTime Datetime { get; set; }

    public virtual Session Session { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
