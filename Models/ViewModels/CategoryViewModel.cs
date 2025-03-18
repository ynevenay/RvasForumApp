namespace ForumApp.Models.ViewModels
{
    public class CategoryViewModel
    {
        public int CategoryId { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<CategoryViewModel> Subcategories { get; set; } = new();
    }
}
