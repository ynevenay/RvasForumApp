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


            var mod = new ApplicationUser
            {
                UserName = "moderator_1@forum.rs",
                Email = "moderator_1@forum.rs",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true
            };

            var modInDb = await userManager.FindByEmailAsync(mod.Email);

            if (modInDb == null)
            {
                await userManager.CreateAsync(mod, "Admin123$");
                await userManager.AddToRoleAsync(mod, Roles.Moderator.ToString());
            }

        }


        public static async Task SeedCategoriesAsync(ApplicationDbContext context)
        {
            if (!context.Categories.Any())
            {
                var kategorije = new List<Category>()
                {
                    //glavne kategorije
                    new Category() {Name = "Nekategorizovano", ParentCategoryId =null},
                    new Category() {Name = "Drustvo", ParentCategoryId =null},
                    new Category() {Name = "Zivot", ParentCategoryId =null},
                    new Category() {Name = "Zabava", ParentCategoryId =null},
                    new Category() {Name = "Zdravlje", ParentCategoryId =null},
                    new Category() {Name = "Priroda", ParentCategoryId =null},
                    new Category() {Name = "Nauka i tehnika", ParentCategoryId =null},
                    new Category() {Name = "Muzika", ParentCategoryId =null},
                    new Category() {Name = "Regioni", ParentCategoryId =null},
                    new Category() {Name = "Umetnost i kultura", ParentCategoryId =null},
                    new Category() {Name = "Informacije o forumu i pomoc", ParentCategoryId =null}
                };
                await context.AddRangeAsync(kategorije);
                await context.SaveChangesAsync();
            }
        }
    }
}
