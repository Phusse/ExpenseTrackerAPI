using ExpenseTracker.ExpensesTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.ExpensesTracker.Data
{
    public class ExpenseTrackerDbContext : DbContext
    {
        public ExpenseTrackerDbContext(DbContextOptions<ExpenseTrackerDbContext> options)
            : base(options)
        {
        }

        public DbSet<Expense> Expenses { get; set; }
    }
}
