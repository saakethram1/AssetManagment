using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;   

namespace AssetManagment.Data.Identity
{
    public class IdentitySeeder
    {
        public static async Task SeedAsync(IServiceProvider services,
                                           string adminEmail,
                                           string adminPassword)
        {
            using var scope = services.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            // make sure database exists and migrations are applied
            await db.Database.MigrateAsync();

            var admin = await userManager.FindByEmailAsync(adminEmail);
            if (admin == null)
            {
                var user = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(user, adminPassword);
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    throw new Exception($"Failed to create admin user: {errors}");
                }
            }
        }

    }
}
