
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ForumApp.Models.ViewModels
{
    public class ThemeCreateViewModel
    {
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public int CategoryId { get; set; }

        public List<SelectListItem> Categories { get; set; } = new();
    }
}
