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
            using (var context = serviceProvider.GetRequiredService<AppDbContext>())
            {
                context.Database.EnsureCreated();
            
                if (!context.UserRoles.Any(user => user.RoleId == "1"))
                {
                    var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
                
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
}