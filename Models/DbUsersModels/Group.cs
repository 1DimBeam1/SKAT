using System;
using System.Collections.Generic;

namespace SKAT_Interface.Models.DbUsersModels;

public partial class Group
{
    public int GroupId { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<User> Users { get; set; } = new List<User>();

    public virtual ICollection<Session> Sessions { get; set; } = new List<Session>();
}
