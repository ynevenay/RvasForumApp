namespace ForumApp.Models
{
    public class Report
    {
        public int ReportId { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public int? ThemeId { get; set; }
        public Theme? Theme { get; set; }

        public int? CommentId { get; set; }
        public Comment? Comment { get; set; }

        public string Reason { get; set; } = string.Empty;
        public DateTime ReportedAt { get; set; } = DateTime.UtcNow;
        public string Status { get; set; } = "Pending";
    }
}
