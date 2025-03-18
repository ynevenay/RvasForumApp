using Microsoft.AspNetCore.Identity;

namespace ForumApp.Models
{
    public class ApplicationUser:IdentityUser
    {
        public string? DisplayName { get; set; }
        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
    }
}
