using Microsoft.EntityFrameworkCore;
using LineManagementSystem.Models;

namespace LineManagementSystem.Services;

public class DatabaseContext : DbContext
{
    public DbSet<LineGroup> LineGroups { get; set; }
    public DbSet<PhoneLine> PhoneLines { get; set; }
    public DbSet<Alert> Alerts { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=linemanagement.db");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<LineGroup>()
            .HasMany(g => g.Lines)
            .WithOne(l => l.Group)
            .HasForeignKey(l => l.GroupId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Alert>()
            .HasOne(a => a.Group)
            .WithMany()
            .HasForeignKey(a => a.GroupId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    public void EnsureCreated()
    {
        Database.EnsureCreated();
    }
}
