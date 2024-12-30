using Microsoft.AspNetCore.Mvc;
using ExpenseTracker.Models;
using ExpenseTracker.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

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
        var expenses = await _context.Expenses
            .Include(e => e.Category)
            .Include(e => e.Budget)
            .ToListAsync();
        var categories = await _context.Categories.ToListAsync();
        var budgets = await _context.Budgets.ToListAsync();
        ViewBag.Categories = new SelectList(categories, "Id", "Name");
        ViewBag.Budgets = new SelectList(budgets, "Id", "Title");

        var viewModel = new ExpenseViewModel
        {
            Expenses = expenses,
            Budgets = budgets
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateBudget(Budget budget)
    {
        budget.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Add this line
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
        _context.Update(budget);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddExpenseToBudget(int budgetId, Expense expense)
    {
        expense.BudgetId = budgetId;
        expense.CategoryId = expense.CategoryId;
        expense.Description = expense.Description;
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

        expense.Description = expense.Description;
        _context.Update(expense);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}

