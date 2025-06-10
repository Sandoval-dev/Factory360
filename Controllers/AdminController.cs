using Factory360.Data;
using Factory360.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Factory360.Controllers;


[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly UserManager<AppUser> _userManager;
    private readonly DataContext _context;
    public AdminController(UserManager<AppUser> userManager, DataContext context)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<ActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Index");
        }

        var activeWorkingUsers = await _context.AttendanceRecords
        .Where(ar => ar.CheckOut == null)
        .Select(ar => ar.UserId)
        .Distinct()
        .ToListAsync();

        //record kaydı null olmayanları getir, user a göre grupla seçme işlemi yapıp her kullanıcın çalışma saatini toplayıp tersten sıralayıp ilk değeri getirdik.
        var mostWorking = await _context.AttendanceRecords
        .Where(ar => ar.TotalHours != null)
        .GroupBy(ar => ar.User)
        .Select(g => new
        {
            User = g.Key,
            TotalHours = g.Sum(ar => ar.TotalHours ?? 0)
        })
        .OrderByDescending(x => (double)x.TotalHours)
        .FirstOrDefaultAsync();

        var lastWorker = await _context.Users
        .OrderByDescending(u => u.CreatedAt)
        .FirstOrDefaultAsync();

        var model = new AdminDashboardModel
        {
            CurrentUser = user,
            TotalUsers = await _context.Users.CountAsync(),
            ActiveUsers = activeWorkingUsers.Count,
            MostWorkingUserFullName = mostWorking?.User.FullName,
            LastWorker=lastWorker?.FullName
        };

        return View(model);
    }
}

