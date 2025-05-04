using System.Diagnostics;
using ForumApp.Models;
using Microsoft.AspNetCore.Mvc;
using ForumApp.Data;
using ForumApp.Models;
using ForumApp.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace ForumApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ApplicationDbContext context,ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;

        }

        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("Ucitavanjee sadrzaja stranice");

            var kategorije = await _context.Categories
                .Include(c => c.Subcategories)
                .Where(c => c.ParentCategoryId == null)
                .ToListAsync();

            var recent = await _context.Themes
                .Include(t => t.User)
                .Include(t => t.Category)
                .OrderByDescending(t => t.CreatedAt)
                .Take(5)
                .ToListAsync();

            var vm = new HomeViewModel
            {
                Categories = kategorije,
                RecentThemes = recent
            };

            return View(vm);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
