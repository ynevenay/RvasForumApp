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


        public async Task<IActionResult> Index(int? categoryId, string searchTerm, string sortOrder,int page=1, int pageSize=9)
        {

            var themesQuery = _context.Themes
                .Include(t => t.Category)
                .Include(t => t.User)
                .Include(t=>t.Votes)
                .AsQueryable();

            // Ako je izabrana kategorija, filtriraj po njoj
            if (categoryId.HasValue)
            {
                themesQuery = themesQuery.Where(t => t.CategoryId == categoryId.Value);
            }


            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                themesQuery = themesQuery.Where(t => t.Title.Contains(searchTerm));
            }

            // Sortiranje
            themesQuery = sortOrder switch
            {
                "title_desc" => themesQuery.OrderByDescending(t => t.Title),
                "votes_up" => themesQuery.OrderByDescending(t => t.Votes.Count(v => v.IsUpVote)),
                "votes_down" => themesQuery.OrderByDescending(t => t.Votes.Count(v => !v.IsUpVote)),
                _ => themesQuery.OrderBy(t => t.Title), // podrazmevano poo title asc
            };


            var totalItems = await themesQuery.CountAsync();
            var themes = await themesQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            //var themes = await themesQuery.ToListAsync();

            // Kategorije za dropdown
            var kategorije = await _context.Categories
                .Where(k => k.ParentCategoryId == null)
                .ToListAsync();

            var selectList = new SelectList(kategorije, "CategoryId", "Name", categoryId);

            var pagedResult = new PagedResult<Theme>
            {
                Items = themes,
                PageNumber = page,
                PageSize = pageSize,
                TotalItems = totalItems
            };

            var viewModel = new ThemeFilterViewModel
            {
                PagedThemes =pagedResult,
                CategorySelectList = selectList,
                SelectedCategoryId = categoryId,
                SearchTerm = searchTerm,
                SortOrder = sortOrder
            };

            ViewBag.CurrentUserId = _userManager.GetUserId(User);
            return View(viewModel);
        }




        public async Task<IActionResult> Details(int id)
        {
            var t = await _context.Themes
                .Include(t => t.Category)
                .Include(t => t.User)
                .Include(t => t.Comments)
                    .ThenInclude(k => k.User)
                .Include(t => t.Comments)
                    .ThenInclude(k => k.Replies)
                .Include(t => t.Comments)
                    .ThenInclude(k => k.Votes)
                .Include(t => t.Votes)
                .FirstOrDefaultAsync(t => t.ThemeId == id);

            if (t == null) return NotFound();
            ViewData["LoggedUserId"] = _userManager.GetUserId(User);
            return View(t);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var theme = await _context.Themes
                .FirstOrDefaultAsync(t => t.ThemeId == id);

            if (theme == null)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);
            if (theme.UserId != userId)
            {
                return Forbid();
            }

            var viewModel = new ThemeEditViewModel
            {
                ThemeId = theme.ThemeId,
                Content = theme.Content
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ThemeEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var theme = await _context.Themes
                .FirstOrDefaultAsync(t => t.ThemeId == model.ThemeId);

            if (theme == null)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);
            if (theme.UserId != userId)
            {
                return Forbid();
            }

            theme.Content = model.Content;
            theme.CreatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { id = theme.ThemeId });
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> UpTheme(int themeId)
        {
            var userId = _userManager.GetUserId(User);
            var postojeciGlas = await _context.Votes.FirstOrDefaultAsync(v => v.UserId == userId && v.ThemeId == themeId);


            if (postojeciGlas != null)
            {
                if (postojeciGlas.IsUpVote)
                {

                    //ponistavanje glasa
                    _context.Votes.Remove(postojeciGlas);

                }
                else
                {
                    //iz dislike u like
                    postojeciGlas.IsUpVote = true;
                }
            }
            else
            {
                _context.Votes.Add(new Vote
                {
                    ThemeId = themeId,
                    UserId = userId,
                    IsUpVote = true
                });
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Details", new { id = themeId });

        }


        [HttpPost]
        [Authorize]
        public async Task<IActionResult> DownTheme(int themeId)
        {
            var userId = _userManager.GetUserId(User);
            var postojeciGlas = await _context.Votes.FirstOrDefaultAsync(v => v.UserId == userId && v.ThemeId == themeId);


            if (postojeciGlas != null)
            {
                if (!postojeciGlas.IsUpVote)
                {

                    //ponistavanje glasa
                    _context.Votes.Remove(postojeciGlas);

                }
                else
                {
                    //iz lajka u dislike
                    postojeciGlas.IsUpVote = false;
                }
            }
            else
            {
                _context.Votes.Add(new Vote
                {
                    ThemeId = themeId,
                    UserId = userId,
                    IsUpVote = false
                });
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Details", new { id = themeId });

        }
    }
}
