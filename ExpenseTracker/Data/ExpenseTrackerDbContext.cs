using ExpenseTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Data;

public class ExpenseTrackerDbContext(DbContextOptions<ExpenseTrackerDbContext> options) : DbContext(options)
{
    public DbSet<Expense> Expenses { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Expense>(b =>
        {
            b.Property(e => e.Id).ValueGeneratedOnAdd();
            b.Property(e => e.Category).IsRequired().HasConversion<string>();
            b.Property(e => e.Amount).IsRequired().HasColumnType("decimal(18,2)");

            b.Property(e => e.DateRecorded)
                .HasColumnType("datetime(6)")
                .HasDefaultValueSql("CURRENT_TIMESTAMP(6)");

            b.ToTable("Expenses");
        });
    }
}
