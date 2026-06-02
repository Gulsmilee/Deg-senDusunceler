namespace DegisenDusunceler.Models
{
    public class Reaction
    {
        public int Id { get; set; }

        // Hangi anıya tepki verildiği
        public int MemoryId { get; set; }
        public Memory? Memory { get; set; }

        // Tepkiyi veren kullanıcı
        public int UserId { get; set; }
        public User? User { get; set; }

        // Emoji tipi: "heart", "fire", "cry", "wow", "clap", "bulb", "laugh"
        public string EmojiType { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}