namespace ForumApp.Models
{
    public class Category
    {
        public int CategoryId { get; set; }
        public string Name { get; set; } = string.Empty;

        public int? ParentCategoryId { get; set; }
        public Category? ParentCategory { get; set; }
        public List<Category> Subcategories { get; set; } = new();

        public List<Theme> Themes { get; set; } = new();
    }
}
