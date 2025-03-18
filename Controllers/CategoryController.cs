using ForumApp.Data;
using ForumApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ForumApp.Controllers
{
    [Authorize(Roles ="Admin")]
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CategoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.ParentCategories = new SelectList(await _context.Categories.ToListAsync(), "CategoryId", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category model)
        {
            if (ModelState.IsValid)
            {
                //var c = new Category()
                //{
                //    Name = model.Name
                //};

                _context.Categories.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.ParentCategories = new SelectList(await _context.Categories.ToListAsync(), "CategoryId", "Name");
            return View(model);
        }

        public async Task<IActionResult> Index()
        {
            var kategorije = await _context.Categories
                .Include(c=>c.Subcategories)
                .Where(c=>c.ParentCategoryId == null)
                .ToListAsync();
            return View(kategorije);
        }


        public async Task<IActionResult> Edit(int id)
        {
            var c = await _context.Categories.FindAsync(id);

            if (c == null) return NotFound();


            ViewBag.ParentCategories = new SelectList(
                await _context.Categories.Where(c => c.CategoryId != id).ToListAsync(),
                "CategoryId",
                "Name"
            );

            return View(c);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Category model)
        {
            if (id != model.CategoryId) return NotFound();

            if (ModelState.IsValid)
            {
                var kategorija = await _context.Categories.FindAsync(id);

                if (kategorija == null) return NotFound();

                kategorija.Name = model.Name;
                kategorija.ParentCategoryId = model.ParentCategoryId;
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.ParentCategories = new SelectList(
                await _context.Categories.Where(c => c.CategoryId != id).ToListAsync(),
                "CategoryId",
                "Name"
            );
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var c = await _context.Categories.FindAsync(id);

            _context.Categories.Remove(c);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var c = await _context.Categories
                   .Include(c => c.Subcategories)
                   .FirstOrDefaultAsync(c => c.CategoryId == id);

            if (c == null) return NotFound();


            return View(c);
        }
    }
}
