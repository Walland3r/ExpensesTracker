namespace ExpenseTracker.Models;

public class Budget
{
    public int Id { get; set; }
    public float Amount { get; set; }
    public string Title { get; set; }
    private DateTime _startDate;
    private DateTime _endDate;

    public DateTime StartDate
    {
        get => _startDate;
        set => _startDate = DateTime.SpecifyKind(value, DateTimeKind.Utc);
    }

    public DateTime EndDate
    {
        get => _endDate;
        set => _endDate = DateTime.SpecifyKind(value, DateTimeKind.Utc);
    }
    public User User { get; set; }
    public ICollection<Expense> Expenses { get; set; } = new List<Expense>();
}