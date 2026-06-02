namespace DegisenDusunceler.Models
{
    public class User
    {
        public int Id { get; set; }

        // Kullanıcının adı soyadı
        public string FullName { get; set; } = string.Empty;

        // Giriş için kullanılacak, veritabanında tekil olmalı
        public string Email { get; set; } = string.Empty;

        // Düz şifre değil, BCrypt ile hashlenmiş hali saklanacak
        public string PasswordHash { get; set; } = string.Empty;

        // Kayıt tarihi — otomatik atanacak
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Bu kullanıcıya ait tüm anılar
        public List<Memory> Memories { get; set; } = new();

        // Bu kullanıcının verdiği tüm tepkiler
        public List<Reaction> Reactions { get; set; } = new();
    }
}