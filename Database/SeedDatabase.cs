using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace ABM_CMS.Database
{
    public static class SeedDatabase
    {
        public static async Task InitializeAdmin(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<AppDbContext>();
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
            context.Database.EnsureCreated();
            
            if (!context.Users.Any())
            {
                var adminUser = new IdentityUser()
                {
                    SecurityStamp = Guid.NewGuid().ToString(),
                    UserName = "Admin",
                    Email = "admin@admin.com", //change 
                };
                var result = await userManager.CreateAsync(adminUser, "@Admin123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }

        }
    }
}