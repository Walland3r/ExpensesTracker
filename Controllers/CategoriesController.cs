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
    }

    public async Task<IActionResult> Index()
    {
        var categories = await _context.Categories.AsNoTracking().ToListAsync();
        return View(categories);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Category category)
    {
        if (string.IsNullOrEmpty(category.Name))
        {
            ModelState.AddModelError("Name", "The Name field is required.");
            return View(category);
        }

        _context.Add(category);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            ModelState.AddModelError("Name", "The Name field is required.");
            return View(new Category { Id = id, Name = name });
        }

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