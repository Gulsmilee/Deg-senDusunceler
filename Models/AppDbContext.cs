using Microsoft.EntityFrameworkCore;

namespace DegisenDusunceler.Models
{
    public class AppDbContext : DbContext
    {
        // Program.cs'teki bağlantı ayarlarını buraya alır
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // Her tablo için bir DbSet
        public DbSet<User> Users { get; set; }
        public DbSet<Memory> Memories { get; set; }
        public DbSet<Reaction> Reactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Aynı email ile iki hesap açılamasın
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // Kullanıcı silinince anıları da silinsin
            modelBuilder.Entity<Memory>()
                .HasOne(m => m.User)
                .WithMany(u => u.Memories)
                .HasForeignKey(m => m.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Anı silinince tepkileri de silinsin
            modelBuilder.Entity<Reaction>()
                .HasOne(r => r.Memory)
                .WithMany(m => m.Reactions)
                .HasForeignKey(r => r.MemoryId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}