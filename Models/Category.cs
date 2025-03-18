namespace ForumApp.Models
{
    public class Category
    {
        public int CategoryId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int? ParentCategoryId { get; set; } //dodato na postojeci model

        public Category? ParentCategory { get; set; } //dodato na postojeci model
        public List<Category> Subcategories { get; set; } = new(); //dodato na postojeci model

        public List<Theme> Themes { get; set; } = new();
    }
}
