using Microsoft.AspNetCore.Mvc;
using ExpenseTracker.Models;
using ExpenseTracker.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

[Authorize]
public class CategoriesController : Controller
{
    private readonly ExpenseTrackerContext _context;

    public CategoriesController(ExpenseTrackerContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var categories = await _context.Categories
            .Where(c => c.UserId == userId)
            .AsNoTracking()
            .ToListAsync();
        return View(categories);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Category category)
    {
        if (string.IsNullOrEmpty(category.Name))
        {
            TempData["ErrorMessage"] = "The Name field is required.";
            return RedirectToAction(nameof(Index));
        }

        category.UserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        _context.Add(category);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            TempData["ErrorMessage"] = "The Name field is required.";
            return RedirectToAction(nameof(Index));
        }

        var category = await _context.Categories.FindAsync(id);
        if (category == null || category.UserId != int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)))
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
        if (category != null && category.UserId == int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)))
        {
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
}