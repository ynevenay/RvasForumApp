using ForumApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ForumApp.Controllers
{
    [Authorize(Roles = "Admin,Moderator")]
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            var korisnici = await _userManager.Users.ToListAsync();

            var obicniKorisnici = new List<ApplicationUser>();


            foreach (var korisnik in korisnici)
            {
                if (await _userManager.IsInRoleAsync(korisnik, "User"))
                {
                    obicniKorisnici.Add(korisnik);
                }
            }

            return View(obicniKorisnici);
        }
    }
}
