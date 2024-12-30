using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Models;

public class Category
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; }

    public int UserId { get; set; }
    public User User { get; set; }
    public ICollection<Expense> Expenses { get; set; }
}