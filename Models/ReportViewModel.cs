using System;
using System.Collections.Generic;

namespace ExpenseTracker.Models
{
    public class ReportViewModel
    {
        public List<Budget> Budgets { get; set; }
        public Budget Budget { get; set; }
        public List<Expense> Expenses { get; set; }
    }
}
