namespace ForumApp.Models
{
    public class Category
    {
        public int CategoryId { get; set; }
        public string Name { get; set; } = string.Empty;

        public List<Theme> Themes { get; set; } = new();
    }
}
