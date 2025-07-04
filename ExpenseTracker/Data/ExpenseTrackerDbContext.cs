using ExpenseTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Data;

public class ExpenseTrackerDbContext(DbContextOptions<ExpenseTrackerDbContext> options) : DbContext(options)
{
    public DbSet<Expense> Expenses { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Budget> Budgets { get; set; }
    public DbSet<SavingGoal> SavingGoals { get; set; }
    public DbSet<SavingGoalContribution> SavingGoalContributions { get; set; }

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

        // configure budget entity
        modelBuilder.Entity<Budget>(entity =>
        {
            entity.HasIndex(b => new { b.UserId, b.Category, b.Month }).IsUnique();
        });

        // configure saving goal entity
        modelBuilder.Entity<SavingGoal>(entity =>
        {
            entity.Property(s => s.Status).HasConversion<string>().IsRequired();
        });

        // cofigure saving goal cotribution entity
        modelBuilder.Entity<SavingGoalContribution>(entity =>
        {
            entity.HasKey(c => c.Id);

            entity.HasOne(c => c.SavingGoal)
                .WithMany(g => g.Contributions)
                .HasForeignKey(c => c.SavingGoalId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(c => c.Expense)
                .WithMany(e => e.Contributions)
                .HasForeignKey(c => c.ExpenseId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
