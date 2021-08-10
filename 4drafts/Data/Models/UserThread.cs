namespace _4drafts.Data.Models
{
    public class UserThread
    {
        public string UserId { get; set; }
        public User User { get; set; }
        public string ThreadId { get; set; }
        public Thread Thread { get; set; }
    }
}
