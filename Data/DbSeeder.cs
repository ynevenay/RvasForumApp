using ForumApp.Constants;
using ForumApp.Models;
using Microsoft.AspNetCore.Identity;

namespace ForumApp.Data
{
    public static class DbSeeder
    {
        public static async Task SeedRolesAsync(IServiceProvider service)
        {
            var userManager = service.GetService<UserManager<ApplicationUser>>();
            var roleManager = service.GetService<RoleManager<IdentityRole>>();

            await roleManager.CreateAsync(new IdentityRole(Roles.Admin.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Roles.Moderator.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Roles.User.ToString()));

            var user = new ApplicationUser
            {
                UserName = "admin@forum.rs",
                Email = "admin@forum.rs",
                EmailConfirmed=true,
                PhoneNumberConfirmed=true
            };

            var userInDb = await userManager.FindByEmailAsync(user.Email);
            if(userInDb == null)
            {
                await userManager.CreateAsync(user,"Admin123$");
                await userManager.AddToRoleAsync(user, Roles.Admin.ToString());
            }
        
        }
    }
}
