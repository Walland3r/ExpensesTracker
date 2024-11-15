namespace ExpenseTracker.Models;

public class Expense
{
    public int Id { get; set; }
    public float Amount { get; set; }
    public DateTime Date { get; set; }
    public required Category Category { get; set; }
    public required User User { get; set; }
    public required Budget Budget { get; set; }
}