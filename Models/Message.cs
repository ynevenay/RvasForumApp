namespace ForumApp.Models
{
    public class Message
    {
        public int MessageId { get; set; }

        public string SenderId { get; set; }
        public ApplicationUser Sender { get; set; }


        public string ReciverId { get; set; }
        public ApplicationUser Reciver { get; set; }

        public string Content { get; set; } = string.Empty;
        public DateTime SentAt { get; set; } = DateTime.UtcNow;
    }
}
