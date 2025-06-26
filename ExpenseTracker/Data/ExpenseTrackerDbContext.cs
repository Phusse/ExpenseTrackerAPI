using ExpenseTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Data;

public class ExpenseTrackerDbContext(DbContextOptions<ExpenseTrackerDbContext> options) : DbContext(options)
{
    public DbSet<Expense> Expenses { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Budget> Budgets { get; set; }
    public DbSet<SavingGoal> SavingGoals { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure User entity
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.PasswordHash).IsRequired();
        });

        // Configure Expense entity
        modelBuilder.Entity<Expense>(entity =>
        {
            entity.HasKey(e => e.Id);

            // Convert enum to string
            entity.Property(e => e.Category)
                  .HasConversion<string>()
                  .IsRequired();

            entity.Property(e => e.Amount).IsRequired();
            entity.Property(e => e.UserId).IsRequired();

            // Configure relationship
            entity.HasOne(e => e.User)
                  .WithMany(u => u.Expenses)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
            modelBuilder.Entity<Budget>()
        .HasIndex(b => new { b.UserId, b.Category, b.Month })
        .IsUnique();
    }
}
