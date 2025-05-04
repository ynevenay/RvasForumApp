using ForumApp.Data;
using ForumApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ForumApp.Controllers
{
    [Authorize]
    public class ReportController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ReportController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        [HttpPost]
        public async Task<IActionResult> ReportTheme (int themeId, string reason)
        {
            var korisnik = await _userManager.GetUserAsync(User);

            var report = new Report
            {
                ThemeId = themeId,
                UserId = korisnik.Id,
                Reason = reason,
                ReportedAt = DateTime.UtcNow,
                Status = "Na cekanju"
            };

            _context.Reports.Add(report);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", "Theme", new { id = themeId });
        }

        [HttpPost]
        public async Task<IActionResult> ReportComment(int commentId, string reason)
        {
            var user = await _userManager.GetUserAsync(User);

            var report = new Report
            {
                CommentId = commentId,
                UserId = user.Id,
                Reason = reason,
                ReportedAt = DateTime.UtcNow,
                Status = "Na cekanju"
            };

            _context.Reports.Add(report);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Theme");
        }

        [Authorize(Roles = "Admin,Moderator")]
        public async Task<IActionResult> Dashboard()
        {
            var reports = await _context.Reports
                .IgnoreQueryFilters()
                .Include(r => r.User)
                .Include(r => r.Theme)
                .Include(r => r.Comment)
                    .ThenInclude(c=>c.Theme)
                .OrderByDescending(r => r.ReportedAt)
                .ToListAsync();

            return View(reports);
        }

        [Authorize(Roles = "Admin,Moderator")]
        [HttpPost]
        public async Task<IActionResult> ChangeStatus(int reportId, string status)
        {
            var report = await _context.Reports.FindAsync(reportId);
            if (report != null)
            {
                report.Status = status;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Dashboard");
        }

        [Authorize(Roles = "Admin,Moderator")]
        [HttpPost]
        public async Task<IActionResult> HideTheme(int themeId)
        {
            var tema = await _context.Themes.FindAsync(themeId);
            if (tema != null)
            {
                tema.IsHidden = true;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Dashboard");
        }

        [Authorize(Roles = "Admin,Moderator")]
        [HttpPost]
        public async Task<IActionResult> HideComment(int commentId)
        {
            var komentar = await _context.Comments.FindAsync(commentId);
            if (komentar != null)
            {
                komentar.IsHidden = true;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Dashboard");
        }

        [Authorize(Roles = "Admin,Moderator")]
        [HttpPost]
        public async Task<IActionResult> UnhideTheme(int themeId)
        {
            var tema = await _context.Themes
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(t=>t.ThemeId == themeId);

            if (tema != null)
            {
                tema.IsHidden = false;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Dashboard");
        }

        [Authorize(Roles = "Admin,Moderator")]
        [HttpPost]
        public async Task<IActionResult> UnhideComment(int commentId)
        {
            var komentar = await _context.Comments
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(c=>c.CommentId == commentId);

            if (komentar != null)
            {
                komentar.IsHidden = false;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Dashboard");
        }
    }
}
