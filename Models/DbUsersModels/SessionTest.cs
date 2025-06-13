using System;
using System.Collections.Generic;

namespace SKAT_Interface.Models.DbUsersModels;

public partial class SessionTest
{
    public int SessionId { get; set; }

    public int TestId { get; set; }

    public virtual Session Session { get; set; } = null!;
}
