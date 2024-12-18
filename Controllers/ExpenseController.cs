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
        return View(expenses);
    }

    public IActionResult Create()
    {
        var categories = _context.Categories.ToList();
        foreach (var category in categories)
        {
            _context.Entry(category).State = EntityState.Detached; // Detach entity
        }
        ViewBag.IsCategoryEmpty = !categories.Any();
        if (ViewBag.IsCategoryEmpty)
        {
            ViewBag.Message = "Please create a category first.";
        }
        ViewBag.Categories = new SelectList(categories, "Id", "Name");
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Expense expense)
    {
        var categories = _context.Categories.ToList();
        foreach (var category in categories)
        {
            _context.Entry(category).State = EntityState.Detached; // Detach entity
        }
        ViewBag.IsCategoryEmpty = !categories.Any();
        if (ViewBag.IsCategoryEmpty)
        {
            ViewBag.Message = "Please create a category first.";
        }
        else
        {
            _context.Add(expense);
            await _context.SaveChangesAsync();
            _context.Entry(expense).State = EntityState.Detached; // Detach entity
            return RedirectToAction(nameof(Index));
        }
        ViewBag.Categories = new SelectList(categories, "Id", "Name");
        return View(expense);
    }
}
