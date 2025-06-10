using System.Threading.Tasks;
using Factory360.Data;
using Factory360.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Factory360.Controllers;


[Authorize(Roles = "Admin")]
public class EmployeeController : Controller
{
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<IdentityRole<int>> _roleManager;
    private readonly DataContext _context;

    public EmployeeController(UserManager<AppUser> userManager, RoleManager<IdentityRole<int>> roleManager, DataContext context)
    {
        _userManager = userManager;
        _context = context;
        _roleManager = roleManager;
    }

    public async Task<IActionResult> Index(string q, int? role = null)
    {
        ViewBag.Roles = new SelectList(_roleManager.Roles.ToList(), "Id", "Name");

        var query = _context.Users.AsQueryable();

        if (!string.IsNullOrEmpty(q))
        {
            query = query.Where(u => u.FullName.ToLower().Contains(q.ToLower()));
        }

        var users = await query.OrderBy(u => u.Id).ToListAsync();
        var employeeList = new List<EmployeeGetModel>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);

            if (role.HasValue)
            {
                var roleEntity = await _roleManager.FindByIdAsync(role.Value.ToString());
                if (roleEntity == null || !roles.Contains(roleEntity.Name!))
                {
                    continue; // Bu kullanıcı bu role sahip değilse listeye ekleme
                }

                ViewBag.SelectedRoleName = roleEntity.Name;
            }

            employeeList.Add(new EmployeeGetModel
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email!,
                HourlyWage = user.HourlyWage,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt,
                Roles = roles.ToList()
            });
        }

        ViewData["q"] = q;
        ViewData["role"] = role;

        return View(employeeList);
    }


    public IActionResult Create()
    {
        var model = new EmployeeCreateModel
        {
            SelectedRoles = new List<string>() // boş liste olarak başlat
        };
        ViewBag.Roles = _roleManager.Roles.Select(r => r.Name).ToList();
        return View();
    }

    [HttpPost]
    public async Task<ActionResult> Create(EmployeeCreateModel model)
    {
        if (ModelState.IsValid)
        {
            var user = new AppUser
            {
                UserName = model.Email.Split('@')[0],
                FullName = model.FullName,
                Email = model.Email,
                HourlyWage = model.HourlyWage,
                IsActive = model.IsActive,
                CreatedAt = DateTime.UtcNow,
                MustChangePassword = true // Yeni kullanıcılar için şifre değişikliği zorunlu
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                if (model.SelectedRoles != null && model.SelectedRoles.Any())
                {
                    await _userManager.AddToRolesAsync(user, model.SelectedRoles);
                }

                TempData["Message"] = "Employee created successfully.";
                return RedirectToAction("Index");
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
        ViewBag.Roles = new MultiSelectList(_roleManager.Roles.ToList(), "Name", "Name");
        return View(model);
    }

    // GET: Employee/Delete/{id}
    public async Task<IActionResult> Delete(int id)
    {
        var user = await _context.Users
            .Include(u => u.AttendanceRecords)
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user == null)
        {
            return RedirectToAction("Index");
        }

        return View(user);
    }



    // POST: Employee/DeleteConfirm
    [HttpPost]
    public ActionResult DeleteConfirm(int id)
    {
        var user = _context.Users
            .Include(u => u.AttendanceRecords)
            .FirstOrDefault(u => u.Id == id);

        if (user != null)
        {
            // İlişkili kayıtları da silmek istiyorsan (isteğe bağlı)
            if (user.AttendanceRecords != null)
            {
                _context.AttendanceRecords.RemoveRange(user.AttendanceRecords);
            }


            _context.Users.Remove(user);
            _context.SaveChanges();

            TempData["Message"] = $"Employee {user.FullName} deleted successfully.";
            return RedirectToAction("Index");
        }

        TempData["Message"] = "Employee could not be deleted.";
        return RedirectToAction("Index");
    }


    public async Task<ActionResult> Edit(int id)
    {
        var user = _context.Users
            .Include(u => u.AttendanceRecords)
            .FirstOrDefault(u => u.Id == id);

        if (user == null)
        {
            TempData["Message"] = "Employee not found.";
            return RedirectToAction("Index");
        }

        var userRoles = await _userManager.GetRolesAsync(user);

        // Salary kayıtları varsa onları kullan, yoksa AttendanceRecord'tan hesapla
        var salaries = new List<decimal>();


        // Örnek olarak bu ay için mesai saatlerini hesapla
        var currentMonth = DateTime.Now.Month;
        var currentYear = DateTime.Now.Year;

        var totalHoursWorked = user.AttendanceRecords
            .Where(a => a.CheckIn.Month == currentMonth && a.CheckIn.Year == currentYear && a.TotalHours.HasValue)
            .Sum(a => a.TotalHours!.Value);

        var calculatedSalary = user.HourlyWage * totalHoursWorked;

        salaries.Add(calculatedSalary);


        // BURAYA EKLE
        ViewBag.Roles = _roleManager.Roles.Select(r => r.Name).ToList();

        var model = new EmployeeEditModel
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email!,
            HourlyWage = user.HourlyWage,
            IsActive = user.IsActive,
            SelectedRoles = userRoles,
            Salaries = salaries,
        };

        return View(model);
    }


    [HttpPost]
    public async Task<ActionResult> Edit(int id, EmployeeEditModel model)
    {
        if (id != model.Id)
        {
            TempData["Message"] = "Employee not found.";
            return RedirectToAction("Index");
        }

        if (ModelState.IsValid)
        {
            var user = _context.Users
                .Include(u => u.AttendanceRecords)
                .FirstOrDefault(u => u.Id == id);

            if (user != null)
            {
                user.FullName = model.FullName;
                user.Email = model.Email;
                user.HourlyWage = model.HourlyWage;
                user.IsActive = model.IsActive;

                if (!string.IsNullOrEmpty(model.Password))
                {
                    var passwordHasher = new PasswordHasher<AppUser>();
                    user.PasswordHash = passwordHasher.HashPassword(user, model.Password);
                }

                // Roller güncelleniyor
                var currentRoles = await _userManager.GetRolesAsync(user);
                if (model.SelectedRoles != null)
                {
                    var rolesToAdd = model.SelectedRoles.Except(currentRoles).ToList();
                    var rolesToRemove = currentRoles.Except(model.SelectedRoles).ToList();

                    if (rolesToAdd.Any())
                    {
                        await _userManager.AddToRolesAsync(user, rolesToAdd);
                    }
                    if (rolesToRemove.Any())
                    {
                        await _userManager.RemoveFromRolesAsync(user, rolesToRemove);
                    }
                }

                _context.SaveChanges();
                TempData["Message"] = "Employee updated successfully.";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["Message"] = "Employee not found.";
            }
        }
        ViewBag.Roles = new MultiSelectList(_roleManager.Roles.ToList(), "Name", "Name");
        return View(model);
    }




}