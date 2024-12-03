using Microsoft.AspNetCore.Mvc;
using ExpenseTracker.Models;
using ExpenseTracker.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

public class CategoriesController : Controller
{
    private readonly ExpenseTrackerContext _context;

    // Konstruktor przyjmujący kontekst bazy danych
    public CategoriesController(ExpenseTrackerContext context)
    {
        _context = context;
    }

    // Akcja do wyświetlania wszystkich kategorii
    public async Task<IActionResult> Index()
    {
        var categories = await _context.Categories.ToListAsync(); // Pobranie kategorii z bazy
        return View(categories); // Przekazanie do widoku
    }

    // Akcja do wyświetlania formularza do tworzenia nowej kategorii
    public IActionResult Create()
    {
        return View();
    }

    // Akcja POST do tworzenia nowej kategorii
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Category category)
    {
        if (ModelState.IsValid)
        {
            _context.Add(category); // Dodanie kategorii do kontekstu
            await _context.SaveChangesAsync(); // Zapisanie zmian w bazie
            return RedirectToAction(nameof(Index)); // Po zapisaniu, przekierowanie na stronę z listą kategorii
        }
        return View(category); // Jeśli wystąpił błąd, ponownie wyświetl formularz
    }
}
