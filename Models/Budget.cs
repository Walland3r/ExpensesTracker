namespace ExpenseTracker.Models;

public class Budget
{
    public int Id { get; set; }
    public float Amount { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public User User { get; set; }
    public ICollection<Expense> Expenses { get; set; }
}