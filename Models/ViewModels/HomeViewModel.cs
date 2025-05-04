namespace ForumApp.Models.ViewModels
{
    public class HomeViewModel
    {
        public List<Category> Categories { get; set; } = new();
        public List<Theme> RecentThemes { get; set; } = new();
    }
}
