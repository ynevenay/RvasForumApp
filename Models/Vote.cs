namespace ForumApp.Models
{
    public class Vote
    {
        public int VoteId { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public int? ThemeId { get; set; }
        public Theme? Theme { get; set; }

        public int? CommentId { get; set; }
        public Comment? Comment { get; set; }

        public bool IsUpVote { get; set; }
    }
}
