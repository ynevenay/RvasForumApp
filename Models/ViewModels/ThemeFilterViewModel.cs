using Microsoft.AspNetCore.Mvc.Rendering;

namespace ForumApp.Models.ViewModels
{
    public class ThemeFilterViewModel
    {
        public PagedResult<Theme> PagedThemes { get; set; }


        //public IEnumerable<Theme> Themes { get; set; }
        public SelectList CategorySelectList { get; set; }
        public int? SelectedCategoryId { get; set; }
        public string SearchTerm { get; set; }
        public string SortOrder { get; set; }

    }
}
