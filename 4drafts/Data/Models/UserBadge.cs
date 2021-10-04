namespace _4drafts.Data.Models
{
    public class UserBadge
    {
        public string UserId { get; set; }
        public User User { get; set; }
        public int BadgeId { get; set; }
        public Badge Badge { get; set; }
    }
}
