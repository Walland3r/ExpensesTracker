using Microsoft.AspNetCore.Mvc;
using ExpenseTracker.Models;
using ExpenseTracker.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

public class ExpensesController : Controller
{
    private readonly ExpenseTrackerContext _context;

    public ExpensesController(ExpenseTrackerContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var expenses = await _context.Expenses.Include(e => e.Category).ToListAsync();
        var categories = await _context.Categories.ToListAsync();
        var budgets = await _context.Budgets.ToListAsync();
        ViewBag.Categories = new SelectList(categories, "Id", "Name");

        var viewModel = new ExpenseViewModel
        {
            Expenses = expenses,
            Budgets = budgets
        };

        return View(viewModel);
    }

    public IActionResult Create()
    {
        var categories = _context.Categories.ToList();
        var budgets = _context.Budgets.ToList();
        ViewBag.IsCategoryEmpty = !categories.Any();
        ViewBag.Categories = new SelectList(categories, "Id", "Name");
        ViewBag.Budgets = new SelectList(budgets, "Id", "Title");
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Expense expense)
    {
        var categories = _context.Categories.ToList();
        var budgets = _context.Budgets.ToList();
        ViewBag.IsCategoryEmpty = !categories.Any();

        // Detach each Category entity instance
        foreach (var category in categories)
        {
            _context.Entry(category).State = EntityState.Detached;
        }
        _context.Add(expense);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateBudget(Budget budget)
    {
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
        try
        {
            _context.Update(budget);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!BudgetExists(budget.Id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }
        return RedirectToAction(nameof(Index));
    }

    private bool BudgetExists(int id)
    {
        return _context.Budgets.Any(e => e.Id == id);
    }
}

