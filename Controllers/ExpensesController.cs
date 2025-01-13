using Microsoft.AspNetCore.Mvc;
using ExpenseTracker.Models;
using ExpenseTracker.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Text;

[Authorize]
public class ExpensesController : Controller
{
    private readonly ExpenseTrackerContext _context;

    public ExpensesController(ExpenseTrackerContext context, ILogger<ExpensesController> logger)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var categories = await _context.Categories.Where(c => c.UserId == userId).ToListAsync();
        ViewBag.Categories = new SelectList(categories, "Id", "Name");

        if (!categories.Any())
        {
            TempData["InfoMessage"] = "Please create a category first.";
        }

        var viewModel = new ExpenseViewModel
        {
            Expenses = await _context.Expenses
                .Include(e => e.Category)
                .Include(e => e.Budget)
                .ToListAsync(),
            Budgets = await _context.Budgets.Where(b => b.UserId == userId).ToListAsync()
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateBudget(Budget budget)
    {
        if (string.IsNullOrEmpty(budget.Title) || budget.Amount <= 0 || budget.StartDate == default || budget.EndDate == default)
        {
            TempData["ErrorMessage"] = "All fields are required and must be valid.";
            return RedirectToAction(nameof(Index));
        }

        if (!decimal.TryParse(budget.Amount.ToString(), out decimal amount))
        {
            TempData["ErrorMessage"] = "Budget amount must be a number.";
            return RedirectToAction(nameof(Index));
        }

        if (amount <= 0)
        {
            TempData["ErrorMessage"] = "Budget amount has to be greater than 0.";
            return RedirectToAction(nameof(Index));
        }

        if (budget.StartDate >= budget.EndDate)
        {
            TempData["ErrorMessage"] = "Start Date must be earlier than End Date.";
            return RedirectToAction(nameof(Index));
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        budget.UserId = int.Parse(userId);
        _context.Budgets.Add(budget);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteBudget(int id)
    {
        var budget = await _context.Budgets.FindAsync(id);
        if (budget != null)
        {
            _context.Budgets.Remove(budget);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditBudget(int id, Budget budget)
    {
        if (id != budget.Id)
        {
            return NotFound();
        }

        if (string.IsNullOrEmpty(budget.Title) || budget.Amount <= 0 || budget.StartDate == default || budget.EndDate == default)
        {
            TempData["ErrorMessage"] = "All fields are required and must be valid.";
            return RedirectToAction(nameof(Index));
        }

        if (!decimal.TryParse(budget.Amount.ToString(), out decimal amount))
        {
            TempData["ErrorMessage"] = "Amount must be a number.";
            return RedirectToAction(nameof(Index));
        }

        if (amount <= 0)
        {
            TempData["ErrorMessage"] = "Amount has to be greater than 0.";
            return RedirectToAction(nameof(Index));
        }

        if (budget.StartDate >= budget.EndDate)
        {
            TempData["ErrorMessage"] = "Start Date must be earlier than End Date.";
            return RedirectToAction(nameof(Index));
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        budget.UserId = int.Parse(userId);

        _context.Update(budget);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddExpenseToBudget(int budgetId, Expense expense)
    {
        if (expense.Amount <= 0)
        {
            TempData["ErrorMessage"] = "Expense amount must be greater than 0.";
            return RedirectToAction(nameof(Index));
        }

        var budget = await _context.Budgets.FindAsync(budgetId);
        if (expense.Date < budget.StartDate || expense.Date > budget.EndDate)
        {
            TempData["ErrorMessage"] = "Expense date must be within the budget's start and end dates.";
            return RedirectToAction(nameof(Index));
        }

        if (string.IsNullOrEmpty(expense.Description) || expense.CategoryId == 0)
        {
            TempData["ErrorMessage"] = "All fields are required.";
            return RedirectToAction(nameof(Index));
        }

        expense.BudgetId = budgetId;
        _context.Expenses.Add(expense);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteExpense(int id)
    {
        var expense = await _context.Expenses.FindAsync(id);
        if (expense != null)
        {
            _context.Expenses.Remove(expense);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditExpense(int id, Expense expense)
    {
        if (id != expense.Id)
        {
            return NotFound();
        }

        if (expense.Amount <= 0)
        {
            TempData["ErrorMessage"] = "Expense amount must be greater than 0.";
            return RedirectToAction(nameof(Index));
        }

        var budget = await _context.Budgets.FindAsync(expense.BudgetId);
        if (budget == null)
        {
            TempData["ErrorMessage"] = "Budget not found.";
            return RedirectToAction(nameof(Index));
        }

        if (expense.Date < budget.StartDate || expense.Date > budget.EndDate)
        {
            TempData["ErrorMessage"] = "Expense date must be within the budget's start and end dates.";
            return RedirectToAction(nameof(Index));
        }

        if (string.IsNullOrEmpty(expense.Description) || expense.CategoryId == 0)
        {
            TempData["ErrorMessage"] = "All fields are required.";
            return RedirectToAction(nameof(Index));
        }

        _context.Update(expense);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ExportBudgetToCsv(int budgetId)
    {
        var budget = await _context.Budgets
            .Include(b => b.Expenses)
            .ThenInclude(e => e.Category)
            .FirstOrDefaultAsync(b => b.Id == budgetId);

        if (budget == null)
        {
            return NotFound();
        }

        var csv = new StringBuilder();
        csv.AppendLine("Title;Amount;StartDate;EndDate");
        csv.AppendLine($"{budget.Title};{budget.Amount};{budget.StartDate.ToShortDateString()};{budget.EndDate.ToShortDateString()}");
        csv.AppendLine("Amount;Date;Category;Description");

        foreach (var expense in budget.Expenses)
        {
            csv.AppendLine($"{expense.Amount};{expense.Date.ToShortDateString()};{expense.Category.Name};{expense.Description}");
        }

        return File(Encoding.UTF8.GetBytes(csv.ToString()), "text/csv", $"{budget.Title}_budget.csv");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ImportBudgetFromCsv(IFormFile csvFile)
    {
        if (csvFile == null || csvFile.Length == 0)
        {
            TempData["ErrorMessage"] = "Please upload a valid CSV file.";
            return RedirectToAction(nameof(Index));
        }

        using (var reader = new StreamReader(csvFile.OpenReadStream()))
        {
            await reader.ReadLineAsync(); // Skip header line

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var line = await reader.ReadLineAsync();

            if (line != null)
            {
                var values = line.Split(';');
                if (values.Length != 4 || values.Any(string.IsNullOrEmpty) || !float.TryParse(values[1], out float budgetAmount) || !DateTime.TryParse(values[2], out DateTime startDate) || !DateTime.TryParse(values[3], out DateTime endDate))
                {
                    TempData["ErrorMessage"] = "Invalid budget data in CSV file.";
                    return RedirectToAction(nameof(Index));
                }

                var budget = new Budget
                {
                    Title = values[0],
                    Amount = budgetAmount,
                    StartDate = startDate,
                    EndDate = endDate,
                    UserId = userId
                };

                _context.Budgets.Add(budget);
                await _context.SaveChangesAsync();

                await reader.ReadLineAsync(); // Skip header line

                while ((line = await reader.ReadLineAsync()) != null)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    values = line.Split(';');
                    if (values.Length != 4 || values.Any(string.IsNullOrEmpty) || !float.TryParse(values[0], out float expenseAmount) || !DateTime.TryParse(values[1], out DateTime expenseDate))
                    {
                        TempData["ErrorMessage"] = "Invalid expense data in CSV file.";
                        return RedirectToAction(nameof(Index));
                    }

                    var category = await _context.Categories.FirstOrDefaultAsync(c => c.Name == values[2] && c.UserId == userId);
                    if (category == null)
                    {
                        category = new Category { Name = values[2], UserId = userId };
                        _context.Categories.Add(category);
                        await _context.SaveChangesAsync();
                        TempData["InfoMessage"] = $"Category '{category.Name}' has been created automatically.";
                    }

                    var expense = new Expense
                    {
                        Amount = expenseAmount,
                        Date = expenseDate,
                        CategoryId = category.Id,
                        Description = values[3],
                        BudgetId = budget.Id,
                        Category = category,
                        Budget = budget
                    };

                    _context.Expenses.Add(expense);
                }

                await _context.SaveChangesAsync();
            }
        }

        return RedirectToAction(nameof(Index));
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GenerateReport(int BudgetId)
    {
        var budget = await _context.Budgets
            .Include(b => b.Expenses)
            .ThenInclude(e => e.Category)
            .FirstOrDefaultAsync(b => b.Id == BudgetId);

        if (budget == null)
        {
            return NotFound();
        }

        var expenses = budget.Expenses.ToList();

        var viewModel = new ReportViewModel
        {
            Budget = budget,
            Expenses = expenses
        };

        return View("Report", viewModel);
    }
}

