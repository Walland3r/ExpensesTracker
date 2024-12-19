namespace ExpenseTracker.Models;

public class Expense
{
    public int Id { get; set; }
    public float Amount { get; set; }
    private DateTime _date;
    public DateTime Date
    {
        get => _date;
        set => _date = DateTime.SpecifyKind(value, DateTimeKind.Utc);
    }
    public required Category Category { get; set; }
    public required int CategoryId { get; set; } // Foreign key for Category
    public required Budget Budget { get; set; }
    public required int BudgetId { get; set; } // Foreign key for Budget
}