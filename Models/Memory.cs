namespace DegisenDusunceler.Models
{
    public class Memory
    {
        public int Id { get; set; }

        // Anının başlığı
        public string Title { get; set; } = string.Empty;

        // Anının tam metni
        public string Content { get; set; } = string.Empty;

        // JavaScript Date() ile formdan gelecek
        public DateTime CreatedAt { get; set; }

        // true = herkese açık, false = sadece sahibi görür
        public bool IsPublic { get; set; }

        // Her Details sayfası açılışında +1 artar
        public int ViewCount { get; set; } = 0;

        // Hangi kullanıcıya ait — foreign key
        public int UserId { get; set; }

        // UserId'ye karşılık gelen User nesnesi
        public User? User { get; set; }

        // Bu anıya ait tüm emoji tepkileri
        public List<Reaction> Reactions { get; set; } = new();
    }
}