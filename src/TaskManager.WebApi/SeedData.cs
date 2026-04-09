using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TaskManager.Infrastructure.Identity;
using TaskManager.Infrastructure.Persistence;

namespace TaskManager.WebApi;

public static class SeedData
{
    public static async Task InitializeAsync(IServiceProvider sp, IConfiguration config)
    {
        using var scope = sp.CreateScope();
        var services = scope.ServiceProvider;

        var db = services.GetRequiredService<AppDbContext>();
        await db.Database.MigrateAsync();

        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        foreach (var role in new[] { "user", "admin" })
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        var adminEmail = config["SeedAdmin:Email"];
        var adminPassword = config["SeedAdmin:Password"];
        if (string.IsNullOrWhiteSpace(adminEmail) || string.IsNullOrWhiteSpace(adminPassword))
        {
            return;
        }

        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var existing = await userManager.FindByEmailAsync(adminEmail.Trim().ToLowerInvariant());
        if (existing is null)
        {
            existing = new ApplicationUser
            {
                UserName = adminEmail.Trim().ToLowerInvariant(),
                Email = adminEmail.Trim().ToLowerInvariant()
            };

            var created = await userManager.CreateAsync(existing, adminPassword);
            if (!created.Succeeded)
            {
                return;
            }
        }

        if (!await userManager.IsInRoleAsync(existing, "admin"))
        {
            await userManager.AddToRoleAsync(existing, "admin");
        }
    }
}

