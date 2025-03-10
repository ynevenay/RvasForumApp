using ForumApp.Data;
using ForumApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ForumApp.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CategoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category model)
        {
            if (ModelState.IsValid)
            {
                var c = new Category()
                {
                    Name = model.Name
                };

                _context.Categories.Add(c);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(model);
        }

        public async Task<IActionResult> Index()
        {
            var kategorije = await _context.Categories.ToListAsync();
            return View(kategorije);
        }


        public async Task<IActionResult> Edit(int id)
        {
            var c = await _context.Categories.FindAsync(id);

            if (c == null) return NotFound();


            var kategorija = new Category
            {
                CategoryId = c.CategoryId,
                Name = c.Name
            };

            return View(kategorija);
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
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
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
            var c = await _context.Categories.FindAsync(id);

            if (c == null) return NotFound();

            var k = new Category()
            {
                CategoryId = c.CategoryId,
                Name = c.Name
            };

            return View(k);
        }
    }
}
