namespace ExpenseTracker.Models;

public class Report
{
    public int Id { get; set; }
    public required User User { get; set; }
    public required Budget Budget { get; set; }
    public required string Title { get; set; }
    public DateTime DateGenerated { get; set; }
}