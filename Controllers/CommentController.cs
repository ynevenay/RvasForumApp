using ForumApp.Data;
using ForumApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ForumApp.Controllers
{
    [Authorize]
    public class CommentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CommentController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        //dodavanje komentaraa
        [HttpPost]
        public async Task<IActionResult> AddComment(int temaId, string sadrzaj, int? roditeljskiKomentarId)
        {
            if (string.IsNullOrWhiteSpace(sadrzaj)) return BadRequest("Komentar ne moze biti prazan");

            var korisnikId = _userManager.GetUserId(User);

            if (roditeljskiKomentarId.HasValue)
            {
                var parentComment = await _context.Comments
                    .Include(k => k.ParentComment)
                    .FirstOrDefaultAsync(k => k.CommentId == roditeljskiKomentarId);


                if (parentComment?.ParentCommentId != null)
                    return BadRequest("....");
            }

            var komentar = new Comment
            {
                Content = sadrzaj,
                ThemeId = temaId,
                UserId = korisnikId,
                ParentCommentId = roditeljskiKomentarId
            };

            _context.Comments.Add(komentar);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", "Theme", new { id = temaId });
        }


        [HttpPost]
        [Authorize]
        public async Task<IActionResult> LikeComment(int commentId)
        {
            var userId = _userManager.GetUserId(User);

            var komentar = await _context.Comments.FindAsync(commentId);

            if (komentar == null) return NotFound();


            var posojeciGlas = await _context.Votes.FirstOrDefaultAsync(v => v.UserId == userId && v.CommentId == commentId);


            if (posojeciGlas != null)
            {
                if (posojeciGlas.IsUpVote)
                {
                    //ako klikne opet na lajk brise se taj glas tj ponistava se 
                    _context.Votes.Remove(posojeciGlas);
                }
                else
                {
                    // iz dislajka u lajk
                    posojeciGlas.IsUpVote = true;
                }
            }
            else
            {
                _context.Votes.Add(new Vote
                {
                    UserId = userId,
                    CommentId = commentId,
                    IsUpVote = true
                });
            }
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "Theme", new { id = komentar.ThemeId });
        }



        [HttpPost]
        [Authorize]
        public async Task<IActionResult> DislikeComment(int commentId)
        {
            var userId = _userManager.GetUserId(User);

            var komentar = await _context.Comments.FindAsync(commentId);

            if (komentar == null) return NotFound();


            var posojeciGlas = await _context.Votes.FirstOrDefaultAsync(v => v.UserId == userId && v.CommentId == commentId);


            if (posojeciGlas != null)
            {
                if (!posojeciGlas.IsUpVote)
                {
                    //ako klikne opet na lajk brise se taj glas tj ponistava se 
                    _context.Votes.Remove(posojeciGlas);
                }
                else
                {
                    // iz lajka u dislajk
                    posojeciGlas.IsUpVote = false;
                }
            }
            else
            {
                _context.Votes.Add(new Vote
                {
                    UserId = userId,
                    CommentId = commentId,
                    IsUpVote = false
                });
            }
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "Theme", new { id = komentar.ThemeId });
        }
    }
}
