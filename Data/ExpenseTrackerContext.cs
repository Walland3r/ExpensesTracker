using Microsoft.EntityFrameworkCore;
using ExpenseTracker.Models;

namespace ExpenseTracker.Data
{
public class ExpenseTrackerContext : DbContext
{
    public ExpenseTrackerContext(DbContextOptions<ExpenseTrackerContext> options)
        : base(options) { }

    public required DbSet<Expense> Expenses { get; set; }
    public required DbSet<Category> Categories { get; set; }
    public required DbSet<Budget> Budgets { get; set; }
    public required DbSet<Report> Reports { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Database=ExpenseTrackerDb;Username=yourusername;Password=yourpassword");
        }
    }
}
}
