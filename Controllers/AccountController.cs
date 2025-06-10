using System.Threading.Tasks;
using Factory360.Data;
using Factory360.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Factory360.Controllers;

public class AccountController : Controller
{
    private UserManager<AppUser> _userManager { get; set; }
    private SignInManager<AppUser> _signInManager { get; set; }
    private readonly RoleManager<IdentityRole<int>> _roleManager;


    public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, RoleManager<IdentityRole<int>> roleManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
    }

    public IActionResult Login()
    {
        return View();
    }

    public ActionResult AccessDenied()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(AccountLoginModel model, string? returnUrl)
    {
        if (ModelState.IsValid)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null)
            {
                await _signInManager.SignOutAsync();
                var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, false);
                if (result.Succeeded)
                {
                    // Kullanıcı giriş yaptıktan sonra AttendanceRecord'a CheckIn ekle
                    var checkInRecord = new AttendanceRecord
                    {
                        UserId = user.Id,
                        CheckIn = DateTime.Now
                    };

                    using (var scope = HttpContext.RequestServices.CreateScope())
                    {
                        var db = scope.ServiceProvider.GetRequiredService<DataContext>();
                        db.AttendanceRecords.Add(checkInRecord);
                        await db.SaveChangesAsync();
                    }

                    if (user.MustChangePassword)
                    {
                        TempData["Message"] = "Please change your password before proceeding.";
                        return RedirectToAction("ChangePassword", "Account");
                    }

                    if (await _userManager.IsInRoleAsync(user, "Admin"))
                    {
                        return RedirectToAction("Index", "Admin");
                    }
                    if (!string.IsNullOrEmpty(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    return RedirectToAction("Index", "Home");
                }
                else if (result.IsLockedOut)
                {
                    var lockoutDate = await _userManager.GetLockoutEndDateAsync(user);
                    var timeLeft = lockoutDate - DateTimeOffset.UtcNow;
                    ModelState.AddModelError(string.Empty, $"Your account is locked out until {lockoutDate}. Time left: {timeLeft?.TotalMinutes} minutes.");
                    TempData["Message"] = $"Your account is locked out until {lockoutDate}. Time left: {timeLeft?.TotalMinutes} minutes.";
                }
                else
                {
                    TempData["Message"] = "Invalid login attempt. Please check your email and password.";
                    ModelState.AddModelError(string.Empty, "Invalid login attempt. Please check your email and password.");
                }
            }
            else
            {
                TempData["Message"] = "Invalid login attempt. User not found.";
                ModelState.AddModelError(string.Empty, "Invalid login attempt. User not found.");
            }
        }
        return View(model);
    }



    public async Task<ActionResult> Logout()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user != null)
        {
            using (var scope = HttpContext.RequestServices.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();
                // Kullanıcının son CheckIn kaydını güncelle
                var latestRecord = db.AttendanceRecords
                .Where(a => a.UserId == user.Id && a.CheckOut == null)
                .OrderByDescending(a => a.CheckIn)
                .FirstOrDefault();

                if (latestRecord != null)
                {
                    latestRecord.CheckOut = DateTime.Now;
                    var duration = latestRecord.CheckOut - latestRecord.CheckIn;
                    latestRecord.TotalHours = (decimal)(latestRecord.CheckOut.Value - latestRecord.CheckIn).TotalMinutes;
                    await db.SaveChangesAsync();
                }
            }
        }
        await _signInManager.SignOutAsync();
        return RedirectToAction("Login", "Account");
    }


    [Authorize]
    public ActionResult ChangePassword()
    {
        return View();
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult> ChangePassword(AccountChangePasswordModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var user = await _userManager.GetUserAsync(User);
        if (user == null) return RedirectToAction("Login", "Account");

        var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
        if (result.Succeeded)
        {
            user.MustChangePassword = false; //password changed
            await _userManager.UpdateAsync(user);
            TempData["Message"] = "Password changed successfully.";
            return RedirectToAction("Index", "Home");
        }
        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        return View(model);
    }
}