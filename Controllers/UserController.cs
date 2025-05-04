using ForumApp.Constants;
using ForumApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ForumApp.Models.ViewModels;
using ForumApp.Data;
namespace ForumApp.Controllers
{
    [Authorize(Roles = "Admin,Moderator")]
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;
        public UserController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Promote(string userId)
        {
            var korisnik = await _userManager.FindByIdAsync(userId);

            if (korisnik == null)
                return NotFound();

            if (await _userManager.IsInRoleAsync(korisnik, nameof(Roles.User)))
                await _userManager.RemoveFromRoleAsync(korisnik, nameof(Roles.User));
            if (!await _userManager.IsInRoleAsync(korisnik, nameof(Roles.Moderator)))
                await _userManager.AddToRoleAsync(korisnik, nameof(Roles.Moderator));
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Menu()
        {
            return View("Menu");
        }
    }
}
