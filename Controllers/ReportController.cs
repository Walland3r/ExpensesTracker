using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ExpenseTracker.Data;
using ExpenseTracker.Models;

namespace ExpenseTracker.Controllers
{
    public class ReportController : Controller
    {
        private readonly ExpenseTrackerContext _context;

        public ReportController(ExpenseTrackerContext context)
        {
            _context = context;
        }

        // GET: Report
        public async Task<IActionResult> Index()
        {
            var reports = await _context.Reports.ToListAsync();
            return View(reports);
        }

        // GET: Report/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var report = await _context.Reports.FindAsync(id);
            if (report == null)
                return NotFound();

            return View(report);
        }

        // GET: Report/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Report/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Report report)
        {
            if (ModelState.IsValid)
            {
                // Logika generowania raportu bezpośrednio w kontrolerze
                report.DateGenerated = DateTime.Now;

                _context.Add(report);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(report);
        }

        private string GenerateReportSummary()
        {
            // Przykładowa logika generowania podsumowania raportu
            var expenses = _context.Expenses.ToList();
            var totalExpenses = expenses.Sum(e => e.Amount);
            return $"Total Expenses: {totalExpenses:C}";
        }

        // GET: Report/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var report = await _context.Reports.FindAsync(id);
            if (report == null)
                return NotFound();

            return View(report);
        }

        // POST: Report/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var report = await _context.Reports.FindAsync(id);
            _context.Reports.Remove(report);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
