using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DegisenDusunceler.Models;

namespace DegisenDusunceler.Controllers
{
    public class MemoryController : Controller
    {
        private readonly AppDbContext _db;

        public MemoryController(AppDbContext db)
        {
            _db = db;
        }

        // ── Giriş kontrolü — her action'dan önce çağırıyoruz ──
        private int? GetUserId()
        {
            var idStr = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(idStr)) return null;
            return int.Parse(idStr);
        }

        // ══════════════════════════════
        // 1. SAYFA: Evren (Index)
        // GET: /Memory/Index
        // ══════════════════════════════
        public IActionResult Index()
        {
            if (GetUserId() == null)
                return RedirectToAction("Login", "Account");

            var memories = _db.Memories
                .Include(m => m.User)
                .Include(m => m.Reactions)
                .Where(m => m.IsPublic)
                .OrderByDescending(m => m.CreatedAt)
                .ToList();

            return View(memories);
        }

        // ══════════════════════════════
        // 2. SAYFA: Kişisel Kapsülüm
        // GET: /Memory/MyCapsule
        // ══════════════════════════════
        public IActionResult MyCapsule()
        {
            var userId = GetUserId();
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var memories = _db.Memories
                .Include(m => m.Reactions)
                .Where(m => m.UserId == userId)
                .OrderByDescending(m => m.CreatedAt)
                .ToList();

            return View(memories);
        }

        // ══════════════════════════════
        // 3. SAYFA: Yeni Anı / Düzenle
        // GET: /Memory/Create
        // GET: /Memory/Create/5 (düzenleme modu)
        // ══════════════════════════════
        public IActionResult Create(int? id)
        {
            if (GetUserId() == null)
                return RedirectToAction("Login", "Account");

            if (id != null)
            {
                var memory = _db.Memories.Find(id);

                if (memory == null || memory.UserId != GetUserId())
                    return RedirectToAction("MyCapsule");

                return View(memory);
            }

            return View();
        }

        // ══════════════════════════════
        // POST: /Memory/Create
        // ══════════════════════════════
        [HttpPost]
        public IActionResult Create(int? id, string title, string content,
                                    string createdAt, bool isPublic)
        {
            var userId = GetUserId();
            if (userId == null)
                return RedirectToAction("Login", "Account");

            if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(content))
            {
                ViewBag.Error = "Başlık ve içerik boş bırakılamaz.";
                return View();
            }

            // JavaScript'ten gelen tarih stringini parse et
            DateTime parsedDate;
            if (!DateTime.TryParse(createdAt, out parsedDate))
                parsedDate = DateTime.Now;

            if (id != null)
            {
                // Düzenleme modu
                var memory = _db.Memories.Find(id);
                if (memory == null || memory.UserId != userId)
                    return RedirectToAction("MyCapsule");

                memory.Title    = title;
                memory.Content  = content;
                memory.IsPublic = isPublic;
                _db.SaveChanges();
            }
            else
            {
                // Yeni anı
                var memory = new Memory
                {
                    Title     = title,
                    Content   = content,
                    CreatedAt = parsedDate,
                    IsPublic  = isPublic,
                    UserId    = userId.Value
                };
                _db.Memories.Add(memory);
                _db.SaveChanges();
            }

            return RedirectToAction("MyCapsule");
        }

        // ══════════════════════════════
        // 4. SAYFA: Anı Detayı
        // GET: /Memory/Details/5
        // ══════════════════════════════
        public IActionResult Details(int id)
        {
            if (GetUserId() == null)
                return RedirectToAction("Login", "Account");

            var memory = _db.Memories
                .Include(m => m.User)
                .Include(m => m.Reactions)
                .FirstOrDefault(m => m.Id == id);

            if (memory == null)
                return RedirectToAction("Index");

            // Gizli anıyı sadece sahibi görebilir
            if (!memory.IsPublic && memory.UserId != GetUserId())
                return RedirectToAction("Index");

            // Görüntülenme sayısını artır
            memory.ViewCount++;
            _db.SaveChanges();

            return View(memory);
        }

        // ══════════════════════════════
        // 5. SAYFA: Profil
        // GET: /Memory/Profile
        // ══════════════════════════════
        public IActionResult Profile()
        {
            var userId = GetUserId();
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var user = _db.Users.Find(userId);
            if (user == null)
                return RedirectToAction("Login", "Account");

            var allMemories   = _db.Memories.Where(m => m.UserId == userId).ToList();
            var totalMemories = allMemories.Count;
            var publicCount   = allMemories.Count(m => m.IsPublic);
            var privateCount  = allMemories.Count(m => !m.IsPublic);
            var totalViews    = allMemories.Sum(m => m.ViewCount);

            ViewBag.User          = user;
            ViewBag.TotalMemories = totalMemories;
            ViewBag.PublicCount   = publicCount;
            ViewBag.PrivateCount  = privateCount;
            ViewBag.TotalViews    = totalViews;

            return View();
        }

        // ══════════════════════════════
        // Anı Sil
        // POST: /Memory/Delete
        // ══════════════════════════════
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var userId = GetUserId();
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var memory = _db.Memories.Find(id);

            if (memory != null && memory.UserId == userId)
            {
                _db.Memories.Remove(memory);
                _db.SaveChanges();
            }

            return RedirectToAction("MyCapsule");
        }

        // ══════════════════════════════
        // Emoji Tepkisi Ekle/Kaldır
        // POST: /Memory/React
        // [FromBody] ile JSON body'den okuyoruz
        // [IgnoreAntiforgeryToken] fetch isteği token göndermediği için
        // ══════════════════════════════
        [HttpPost]
        [IgnoreAntiforgeryToken]
        public IActionResult React([FromBody] ReactionRequest request)
        {
            var userId = GetUserId();
            if (userId == null)
                return Json(new { success = false });

            // Aynı kullanıcı aynı emojiye daha önce bastı mı?
            var existing = _db.Reactions.FirstOrDefault(r =>
                r.MemoryId == request.MemoryId &&
                r.UserId   == userId &&
                r.EmojiType == request.EmojiType);

            if (existing != null)
            {
                // Varsa kaldır (toggle)
                _db.Reactions.Remove(existing);
            }
            else
            {
                // Yoksa ekle
                _db.Reactions.Add(new Reaction
                {
                    MemoryId  = request.MemoryId,
                    UserId    = userId.Value,
                    EmojiType = request.EmojiType,
                    CreatedAt = DateTime.Now
                });
            }

            _db.SaveChanges();

            // Güncel sayıyı JS'e döndür
            var count = _db.Reactions.Count(r =>
                r.MemoryId  == request.MemoryId &&
                r.EmojiType == request.EmojiType);

            return Json(new { success = true, count });
        }

    }   // ← MemoryController sınıfı kapanıyor

    // ── React endpoint'i için JSON body yardımcı sınıfı ──
    // fetch('Memory/React') ile gelen { memoryId, emojiType } JSON'ını
    // bu sınıfa otomatik map ediyoruz
    public class ReactionRequest
    {
        public int MemoryId { get; set; }
        public string EmojiType { get; set; } = string.Empty;
    }

}   // ← namespace kapanıyor