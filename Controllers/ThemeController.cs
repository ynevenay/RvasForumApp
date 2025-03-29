using ForumApp.Data;
using ForumApp.Models;
using ForumApp.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ForumApp.Controllers
{
    [Authorize]
    public class ThemeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ThemeController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Create()
        {
            var c = await _context.Categories
                .Include(c => c.Subcategories)
                .Where(c => c.ParentCategoryId == null)
                .ToListAsync();

            var selectListItems = DohvatiKategorije(c);

            var viewModel = new ThemeCreateViewModel
            {
                Categories = selectListItems
            };
            return View(viewModel);
        }


        private List<SelectListItem> DohvatiKategorije(List<Category> categories, int nivo = 0)
        {
            var selectList = new List<SelectListItem>();

            foreach (var category in categories)
            {
                selectList.Add(new SelectListItem
                {
                    //ZA GLAVNE KATEGORIJE
                    Value = category.CategoryId.ToString(),
                    Text = new string('-', nivo * 3) + " " + category.Name
                });
                if (category.Subcategories.Any())
                {
                    selectList.AddRange(DohvatiKategorije(category.Subcategories, nivo + 1));
                }
            }
            return selectList;
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ThemeCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var k = await _context.Categories
                    .Include(k => k.Subcategories)
                    .Where(k => k.ParentCategoryId == null)
                    .ToListAsync();

                model.Categories = DohvatiKategorije(k);
                return View(model);
            }

            var t = new Theme
            {
                Title = model.Title,
                Content = model.Content,
                CategoryId = model.CategoryId,
                CreatedAt = DateTime.UtcNow,
                UserId = _userManager.GetUserId(User)
            };


            _context.Themes.Add(t);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }


        public async Task<IActionResult> Index()
        {
            var teme = await _context.Themes
                .Include(t => t.Category)
                .Include(t => t.User)
                .ToListAsync();

            return View(teme);
        }


        public async Task<IActionResult> Details(int id)
        {
            var t = await _context.Themes
                .Include(t => t.Category)
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.ThemeId == id);

            if (t == null) return NotFound();

            return View(t);
        }




    }
}
