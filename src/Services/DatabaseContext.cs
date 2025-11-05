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
        
        // Manual migration: Add LineSystem column if it doesn't exist
        try
        {
            using var connection = Database.GetDbConnection();
            connection.Open();
            using var command = connection.CreateCommand();
            
            // Check if LineSystem column exists
            command.CommandText = "PRAGMA table_info(PhoneLines)";
            using var reader = command.ExecuteReader();
            bool lineSystemExists = false;
            while (reader.Read())
            {
                if (reader.GetString(1) == "LineSystem")
                {
                    lineSystemExists = true;
                    break;
                }
            }
            reader.Close();
            
            // Add LineSystem column if it doesn't exist
            if (!lineSystemExists)
            {
                command.CommandText = "ALTER TABLE PhoneLines ADD COLUMN LineSystem TEXT NULL";
                command.ExecuteNonQuery();
            }
        }
        catch
        {
            // If migration fails, ignore (table might not exist yet)
        }
    }
}
