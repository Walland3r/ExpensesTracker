using System.Collections.Generic;

namespace ExpenseTracker.Models
{
    public class ExpenseViewModel
    {
        public IEnumerable<Expense> Expenses { get; set; }
        public IEnumerable<Budget> Budgets { get; set; }
    }
}
