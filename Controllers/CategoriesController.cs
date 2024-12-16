using Microsoft.AspNetCore.Mvc;
using ExpenseTracker.Models;
using ExpenseTracker.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

public class CategoriesController : Controller
{
    private readonly ExpenseTrackerContext _context;

    public CategoriesController(ExpenseTrackerContext context)
    {
        _context = context;
        Console.WriteLine("ExpenseTrackerContext initialized: " + _context);
    }

    public async Task<IActionResult> Index()
    {
        var categories = await _context.Categories.ToListAsync();
        return View(categories);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Category category)
    {
        _context.Add(category);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, string name)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category == null)
        {
            return NotFound();
        }

        category.Name = name;
        _context.Update(category);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category != null)
        {
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
}