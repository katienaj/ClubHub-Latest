using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using CSHonorsTemplate.Models;

namespace CSHonorsTemplate.Models;

public partial class DatabaseContext : DbContext
{
    public DatabaseContext()
    {
    }

    public DatabaseContext(DbContextOptions<DatabaseContext> options)
        : base(options)
    {
    }

    public DbSet<Club> Clubs { get; set; }
    public DbSet<Join> Joins { get; set; }
    public DbSet<Person> People { get; set; }
    public DbSet<UserPermission> UserPermissions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Club>().ToTable("Clubs");
        modelBuilder.Entity<Join>().ToTable("Joins");
        modelBuilder.Entity<Person>().ToTable("People");
        modelBuilder.Entity<UserPermission>().ToTable("UserPermissions");
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

public DbSet<CSHonorsTemplate.Models.ClubType> ClubType { get; set; } = default!;

public DbSet<CSHonorsTemplate.Models.Meeting> Meeting { get; set; } = default!;

public DbSet<CSHonorsTemplate.Models.Attendance> Attendance { get; set; } = default!;
}
