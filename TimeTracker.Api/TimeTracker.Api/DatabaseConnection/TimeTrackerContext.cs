using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using TimeTracker.Api.Models;

namespace TimeTracker.Api.DatabaseConnection;

public partial class TimeTrackerContext : DbContext
{
    public TimeTrackerContext()
    {
    }

    public TimeTrackerContext(DbContextOptions<TimeTrackerContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Activity> Activities { get; set; }

    public virtual DbSet<Record> Records { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<StatusLog> StatusLogs { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserAssignment> UserAssignments { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.

    // => optionsBuilder.UseSqlServer("Addr=DESKTOP-HJGHC9D\\SQLEXPRESS; database=TimeTracker; Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True");
    => optionsBuilder.UseSqlServer("Addr=karldelfin.database.windows.net; Database=TimeTracker; User Id=KarlDelfin; Password=Karl123*");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Activity>(entity =>
        {
            entity.Property(e => e.ActivityId).ValueGeneratedNever();

            entity.HasOne(d => d.AssignedUser).WithMany(p => p.ActivityAssignedUsers).HasConstraintName("FK_Activity_User1");

            entity.HasOne(d => d.User).WithMany(p => p.ActivityUsers)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Activity_User");
        });

        modelBuilder.Entity<Record>(entity =>
        {
            entity.Property(e => e.RecordId).ValueGeneratedNever();

            entity.HasOne(d => d.Activity).WithMany(p => p.Records)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Record_Activity");

            entity.HasOne(d => d.User).WithMany(p => p.Records)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Record_User");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.Property(e => e.RoleId).ValueGeneratedNever();
        });

        modelBuilder.Entity<StatusLog>(entity =>
        {
            entity.Property(e => e.StatusLogId).ValueGeneratedNever();

            entity.HasOne(d => d.Record).WithMany(p => p.StatusLogs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_StatusLog_Record");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(e => e.UserId).ValueGeneratedNever();
        });

        modelBuilder.Entity<UserAssignment>(entity =>
        {
            entity.Property(e => e.UserAssignmentId).ValueGeneratedNever();

            entity.HasOne(d => d.Role).WithMany(p => p.UserAssignments)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserAssignment_Role");

            entity.HasOne(d => d.User).WithMany(p => p.UserAssignments)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserAssignment_User");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
