using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using SKAT_Interface.Models.DbTaskModels;

namespace SKAT_Interface.Data;

public partial class DbTaskContext : DbContext
{
    public DbTaskContext(DbContextOptions<DbTaskContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Algorithm> Algorithms { get; set; }

    public virtual DbSet<Algorithmstep> Algorithmsteps { get; set; }

    public virtual DbSet<Test> Tests { get; set; }

    public virtual DbSet<Testinputdatum> Testinputdata { get; set; }

    public virtual DbSet<Trackedvariable> Trackedvariables { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Algorithm>(entity =>
        {
            entity.HasKey(e => e.AlgoId);

            entity.ToTable("algorithms");

            entity.Property(e => e.AlgoId).HasColumnName("algo_id");
            entity.Property(e => e.AlgoName)
                .HasDefaultValueSql("''::text")
                .HasColumnName("algo_name");
            entity.Property(e => e.PicPath)
                .HasDefaultValueSql("''::text")
                .HasColumnName("pic_path");
            entity.Property(e => e.SrcPath).HasColumnName("src_path");
        });

        modelBuilder.Entity<Algorithmstep>(entity =>
        {
            entity.HasKey(e => new { e.AlgoStep, e.AlgoId });

            entity.ToTable("algorithmsteps");

            entity.HasIndex(e => new { e.AlgoId, e.AlgoStep }, "AK_algorithmsteps_algo_id_algo_step").IsUnique();

            entity.Property(e => e.AlgoStep).HasColumnName("algo_step");
            entity.Property(e => e.AlgoId).HasColumnName("algo_id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Difficult).HasColumnName("difficult");

            entity.HasOne(d => d.Algo).WithMany(p => p.Algorithmsteps).HasForeignKey(d => d.AlgoId);
        });

        modelBuilder.Entity<Test>(entity =>
        {
            entity.ToTable("tests");

            entity.HasIndex(e => e.AlgoId, "IX_tests_algo_id");

            entity.Property(e => e.TestId).HasColumnName("test_id");
            entity.Property(e => e.AlgoId).HasColumnName("algo_id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Difficult).HasColumnName("difficult");
            entity.Property(e => e.SolvedCount).HasColumnName("solved_count");
            entity.Property(e => e.TestName).HasColumnName("test_name");
            entity.Property(e => e.UnsolvedCount).HasColumnName("unsolved_count");

            entity.HasOne(d => d.Algo).WithMany(p => p.Tests).HasForeignKey(d => d.AlgoId);
        });

        modelBuilder.Entity<Testinputdatum>(entity =>
        {
            entity.HasKey(e => new { e.TestId, e.VarName });

            entity.ToTable("testinputdata");

            entity.Property(e => e.TestId).HasColumnName("test_id");
            entity.Property(e => e.VarName).HasColumnName("var_name");
            entity.Property(e => e.LineNumber).HasColumnName("line_number");
            entity.Property(e => e.VarType).HasColumnName("var_type");
            entity.Property(e => e.VarValue).HasColumnName("var_value");
        });

        modelBuilder.Entity<Trackedvariable>(entity =>
        {
            entity.HasKey(e => e.Sequence);

            entity.ToTable("trackedvariables");

            entity.HasIndex(e => new { e.AlgoId, e.AlgoStep }, "IX_trackedvariables_algo_id_algo_step");

            entity.Property(e => e.Sequence).HasColumnName("sequence");
            entity.Property(e => e.AlgoId).HasColumnName("algo_id");
            entity.Property(e => e.AlgoStep).HasColumnName("algo_step");
            entity.Property(e => e.LineNumber).HasColumnName("line_number");
            entity.Property(e => e.VarName).HasColumnName("var_name");
            entity.Property(e => e.VarType).HasColumnName("var_type");

            entity.HasOne(d => d.Algorithmstep).WithMany(p => p.Trackedvariables)
                .HasPrincipalKey(p => new { p.AlgoId, p.AlgoStep })
                .HasForeignKey(d => new { d.AlgoId, d.AlgoStep });
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
