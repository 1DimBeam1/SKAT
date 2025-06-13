using System;
using System.Collections.Generic;

namespace SKAT_Interface.Models.DbUsersModels;

public partial class Session
{
    public int SessionId { get; set; }

    public float Difficult { get; set; }

    public DateTime DateStart { get; set; }

    public DateTime DateFinish { get; set; }

    public TimeSpan Time { get; set; }

    public string? SessionType { get; set; }

    public virtual ICollection<Grade> Grades { get; set; } = new List<Grade>();

    public virtual ICollection<SessionTest> SessionTests { get; set; } = new List<SessionTest>();

    public virtual ICollection<SolutionsByProgram> SolutionsByPrograms { get; set; } = new List<SolutionsByProgram>();

    public virtual ICollection<SolutionsByUser> SolutionsByUsers { get; set; } = new List<SolutionsByUser>();

    public virtual ICollection<Group> Groups { get; set; } = new List<Group>();
}
