namespace ExpenseTracker.Models;
public class User
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
    public bool Theme { get; set; } // true for dark, false for light
    public ICollection<Expense> Expenses { get; set; }
    public ICollection<Budget> Budgets { get; set; }
}
