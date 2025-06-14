using System;
using System.Collections.Generic;

namespace SKAT_Interface.Models.DbUsersModels;

public partial class User
{
    public int UserId { get; set; }

    public int GroupId { get; set; }

    public string Role { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Login { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string IconPath { get; set; } = null!;

    public virtual ICollection<Grade> Grades { get; set; } = new List<Grade>();

    public virtual Group? Group { get; set; }

    public virtual ICollection<SolutionsByUser> SolutionsByUsers { get; set; } = new List<SolutionsByUser>();
}
