using Factory360.Data;
using Factory360.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Factory360.Controllers;


[Authorize(Roles = "Admin")]
public class RoleController : Controller
{
    private readonly RoleManager<IdentityRole<int>> _roleManager;
    private readonly UserManager<AppUser> _userManager;

    public RoleController(RoleManager<IdentityRole<int>> roleManager, UserManager<AppUser> userManager)
    {
        _roleManager = roleManager;
        _userManager = userManager;
    }

    public ActionResult Index()
    {
        return View(_roleManager.Roles);
    }

    public ActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<ActionResult> Create(RoleCreateModel model)
    {
        if (ModelState.IsValid)
        {
            var result = await _roleManager.CreateAsync(new IdentityRole<int> { Name = model.RoleName });

            if (result.Succeeded)
            {
                TempData["Message"] = "Role created successfully.";
                return RedirectToAction("Index");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
        }
        return View(model);
    }


    public async Task<ActionResult> Edit(int id)
    {
        var entity = await _roleManager.FindByIdAsync(id.ToString());
        if (entity != null)
        {
            return View(new RoleEditModel()
            {
                Id = entity.Id,
                RoleName = entity.Name!
            });
        }
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<ActionResult> Edit(int id, RoleEditModel model)
    {
        if (ModelState.IsValid)
        {
            var entity = await _roleManager.FindByIdAsync(id.ToString());
            if (entity != null)
            {
                entity.Name = model.RoleName;
                var result = await _roleManager.UpdateAsync(entity);
                if (result.Succeeded)
                {
                    TempData["Message"] = "Role updated successfully.";
                    return RedirectToAction("Index");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
        }
        return View(model);
    }


    public async Task<ActionResult> Delete(int id)
    {
        if (id.ToString() == null)
        {
            return RedirectToAction("Index");
        }
        var role = await _roleManager.FindByIdAsync(id.ToString());
        if (role != null)
        {
            ViewBag.Users = await _userManager.GetUsersInRoleAsync(role.Name!);
            return View(role);
        }
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<ActionResult> DeleteConfirm(int id)
    {
        if (id.ToString() == null)
        {
            return RedirectToAction("Index");
        }

        var role = await _roleManager.FindByIdAsync(id.ToString());
        if (role != null)
        {
            var result = await _roleManager.DeleteAsync(role);
            if (result.Succeeded)
            {
                TempData["Message"] = "Role deleted successfully.";
                return RedirectToAction("Index");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
        }
        return RedirectToAction("Index");
    }
}