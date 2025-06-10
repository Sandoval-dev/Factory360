using System.Threading.Tasks;
using Factory360.Data;
using Factory360.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Factory360.Controllers;


[Authorize(Roles = "Admin")]
public class InfoController : Controller
{
    private readonly DataContext _context;
    public InfoController(DataContext context)
    {
        _context = context;
    }

    public async Task<ActionResult> Index(int page = 1)
    {
        int pageSize = 12;
        var totalRecords = await _context.AttendanceRecords.CountAsync();
        int totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

        var pagedRecords = await _context.AttendanceRecords
        .Include(r => r.User)
        .OrderByDescending(m => m.CheckIn)
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();

        var viewModel = new PaginatedAttendanceViewModel
        {
            Records = pagedRecords,
            TotalPages = totalPages,
            CurrentPage = page
        };

        return View(viewModel);
    }

    public async Task<ActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return RedirectToAction("Index");
        }
        var record = await _context.AttendanceRecords
        .Include(r => r.User)
        .FirstOrDefaultAsync(i => i.Id == id);
        if (record != null)
        {
            return View(record);
        }
        return RedirectToAction("Index");
    }

    [HttpPost]
    public ActionResult DeleteConfirm(int id)
    {
        var record = _context.AttendanceRecords
        .Include(u=>u.User)
        .FirstOrDefault(r => r.Id == id);
        if (record != null)
        {
            _context.AttendanceRecords.Remove(record);
            _context.SaveChanges();
            TempData["Message"] = $"{record.User.FullName}'s record deleted successfully.";
            return RedirectToAction("Index");
        }
        return RedirectToAction("Index");
    }
}