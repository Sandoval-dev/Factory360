using Factory360.Data;
using Microsoft.AspNetCore.Identity;

public class IdentityInitializer
{
    public static async Task SeedAsync(UserManager<AppUser> userManager, RoleManager<IdentityRole<int>> roleManager)
    {
        string adminRole = "Admin";
        string adminEmail = "admin@gmail.com";
        string adminPassword = "Admin123.";

        if (!await roleManager.RoleExistsAsync(adminRole))
        {
            await roleManager.CreateAsync(new IdentityRole<int>(adminRole));
        }

        //check if the admin user exists
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            adminUser = new AppUser
            {
                UserName = adminEmail.Split('@')[0],
                Email = adminEmail,
                EmailConfirmed = true,
                FullName = "Admin User",
                HourlyWage = 0, // Admins typically don't have an hourly wage
                IsActive = true,
            };

            var result = await userManager.CreateAsync(adminUser, adminPassword);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, adminRole);
            }
        }
    }
}