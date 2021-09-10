namespace _4drafts.Data.Models
{
    public class GenreThread
    {
        public int GenreId { get; set; }
        public Genre Genre { get; set; }
        public string ThreadId { get; set; }
        public Thread Thread { get; set; }
    }
}
