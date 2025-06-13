using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using SKAT_Interface.Models.DbUsersModels;

namespace SKAT_Interface.Data;

public partial class DbUsersContext : DbContext
{
    public DbUsersContext(DbContextOptions<DbUsersContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Grade> Grades { get; set; }

    public virtual DbSet<Group> Groups { get; set; }

    public virtual DbSet<Session> Sessions { get; set; }

    public virtual DbSet<SessionTest> SessionTests { get; set; }

    public virtual DbSet<SolutionsByProgram> SolutionsByPrograms { get; set; }

    public virtual DbSet<SolutionsByUser> SolutionsByUsers { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<VariablesSolutionsByProgram> VariablesSolutionsByPrograms { get; set; }

    public virtual DbSet<VariablesSolutionsByUser> VariablesSolutionsByUsers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Grade>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.SessionId });

            entity.HasIndex(e => e.SessionId, "IX_Grades_session_id");

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.SessionId).HasColumnName("session_id");
            entity.Property(e => e.Datetime).HasColumnName("datetime");
            entity.Property(e => e.Mark).HasColumnName("mark");

            entity.HasOne(d => d.Session).WithMany(p => p.Grades).HasForeignKey(d => d.SessionId);

            entity.HasOne(d => d.User).WithMany(p => p.Grades).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<Group>(entity =>
        {
            entity.Property(e => e.GroupId).HasColumnName("group_id");
            entity.Property(e => e.Name).HasColumnName("name");
        });

        modelBuilder.Entity<Session>(entity =>
        {
            entity.Property(e => e.SessionId).HasColumnName("session_id");
            entity.Property(e => e.DateFinish).HasColumnName("date_finish");
            entity.Property(e => e.DateStart).HasColumnName("date_start");
            entity.Property(e => e.Difficult).HasColumnName("difficult");
            entity.Property(e => e.SessionType).HasColumnName("session_type");
            entity.Property(e => e.Time).HasColumnName("time");

            entity.HasMany(d => d.Groups).WithMany(p => p.Sessions)
                .UsingEntity<Dictionary<string, object>>(
                    "GroupSession",
                    r => r.HasOne<Group>().WithMany().HasForeignKey("GroupId"),
                    l => l.HasOne<Session>().WithMany().HasForeignKey("SessionId"),
                    j =>
                    {
                        j.HasKey("SessionId", "GroupId");
                        j.ToTable("GroupSessions");
                        j.HasIndex(new[] { "GroupId" }, "IX_GroupSessions_group_id");
                        j.IndexerProperty<int>("SessionId").HasColumnName("session_id");
                        j.IndexerProperty<int>("GroupId").HasColumnName("group_id");
                    });
        });

        modelBuilder.Entity<SessionTest>(entity =>
        {
            entity.HasKey(e => new { e.SessionId, e.TestId });

            entity.Property(e => e.SessionId).HasColumnName("session_id");
            entity.Property(e => e.TestId).HasColumnName("test_id");

            entity.HasOne(d => d.Session).WithMany(p => p.SessionTests).HasForeignKey(d => d.SessionId);
        });

        modelBuilder.Entity<SolutionsByProgram>(entity =>
        {
            entity.HasKey(e => new { e.SessionId, e.TestId, e.ProgramStep, e.ProgramLineNumber, e.OrderNumber });

            entity.HasOne(d => d.Session).WithMany(p => p.SolutionsByPrograms).HasForeignKey(d => d.SessionId);
        });

        modelBuilder.Entity<SolutionsByUser>(entity =>
        {
            entity.HasKey(e => new { e.SessionId, e.UserId, e.UserStep, e.UserLineNumber, e.OrderNumber, e.TestId });

            entity.HasIndex(e => e.UserId, "IX_SolutionsByUsers_UserId");

            entity.HasOne(d => d.Session).WithMany(p => p.SolutionsByUsers).HasForeignKey(d => d.SessionId);

            entity.HasOne(d => d.User).WithMany(p => p.SolutionsByUsers).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(e => e.GroupId, "IX_Users_group_id");

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.GroupId).HasColumnName("group_id");
            entity.Property(e => e.IconPath).HasColumnName("icon_path");
            entity.Property(e => e.Login).HasColumnName("login");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.Password).HasColumnName("password");
            entity.Property(e => e.Role).HasColumnName("role");

            entity.HasOne(d => d.Group).WithMany(p => p.Users).HasForeignKey(d => d.GroupId);
        });

        modelBuilder.Entity<VariablesSolutionsByProgram>(entity =>
        {
            entity.HasKey(e => new { e.ProgramStep, e.ProgramLineNumber, e.OrderNumber, e.TestId, e.VarName });
        });

        modelBuilder.Entity<VariablesSolutionsByUser>(entity =>
        {
            entity.HasKey(e => new { e.UserStep, e.UserLineNumber, e.OrderNumber, e.TestId, e.VarName });
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
