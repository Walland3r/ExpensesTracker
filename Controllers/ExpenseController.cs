using Microsoft.AspNetCore.Mvc;
using ExpenseTracker.Models;
using ExpenseTracker.Data;
using Microsoft.EntityFrameworkCore;


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
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Expense expense)
    {
        if (ModelState.IsValid)
        {
            _context.Add(expense);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(expense);
    }
}
