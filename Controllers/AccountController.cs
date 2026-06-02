using Microsoft.AspNetCore.Mvc;
using DegisenDusunceler.Models;
using BCrypt.Net;

namespace DegisenDusunceler.Controllers
{
    public class AccountController : Controller
    {
        // Veritabanı bağlantısı — constructor injection ile geliyor
        private readonly AppDbContext _db;

        public AccountController(AppDbContext db)
        {
            _db = db;
        }

        // ══════════════════════════════
        // GET: /Account/Login
        // ══════════════════════════════
        public IActionResult Login()
        {
            // Zaten giriş yapmışsa direkt Index'e yönlendir
            if (HttpContext.Session.GetString("UserId") != null)
                return RedirectToAction("Index", "Memory");

            return View();
        }

        // ══════════════════════════════
        // POST: /Account/Login
        // ══════════════════════════════
        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            // Boş alan kontrolü
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                ViewBag.Error = "Email ve şifre boş bırakılamaz.";
                return View();
            }

            // Emaile göre kullanıcıyı bul
            var user = _db.Users.FirstOrDefault(u => u.Email == email);

            if (user == null)
            {
                ViewBag.Error = "Bu email ile kayıtlı hesap bulunamadı.";
                return View();
            }

            // BCrypt ile şifre doğrulama
            bool isPasswordCorrect = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);

            if (!isPasswordCorrect)
            {
                ViewBag.Error = "Şifre hatalı.";
                return View();
            }

            // Doğruysa session'a kullanıcı bilgilerini yaz
            HttpContext.Session.SetString("UserId", user.Id.ToString());
            HttpContext.Session.SetString("UserFullName", user.FullName);
            HttpContext.Session.SetString("UserEmail", user.Email);

            return RedirectToAction("Index", "Memory");
        }

        // ══════════════════════════════
        // GET: /Account/Register
        // ══════════════════════════════
        public IActionResult Register()
        {
            if (HttpContext.Session.GetString("UserId") != null)
                return RedirectToAction("Index", "Memory");

            return View();
        }

        // ══════════════════════════════
        // POST: /Account/Register
        // ══════════════════════════════
        [HttpPost]
        public IActionResult Register(string fullName, string email, string password, string passwordConfirm)
        {
            // Boş alan kontrolü
            if (string.IsNullOrWhiteSpace(fullName) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password))
            {
                ViewBag.Error = "Tüm alanlar doldurulmalıdır.";
                return View();
            }

            // Şifre eşleşme kontrolü
            if (password != passwordConfirm)
            {
                ViewBag.Error = "Şifreler eşleşmiyor.";
                return View();
            }

            // Şifre uzunluk kontrolü — en az 6 karakter
            if (password.Length < 6)
            {
                ViewBag.Error = "Şifre en az 6 karakter olmalıdır.";
                return View();
            }

            // Aynı email ile kayıt var mı?
            bool emailExists = _db.Users.Any(u => u.Email == email);
            if (emailExists)
            {
                ViewBag.Error = "Bu email zaten kayıtlı.";
                return View();
            }

            // Yeni kullanıcı oluştur — şifreyi BCrypt ile hashle
            var newUser = new User
            {
                FullName = fullName,
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                CreatedAt = DateTime.Now
            };

            _db.Users.Add(newUser);
            _db.SaveChanges();

            // Kayıt başarılı — otomatik giriş yaptır
            HttpContext.Session.SetString("UserId", newUser.Id.ToString());
            HttpContext.Session.SetString("UserFullName", newUser.FullName);
            HttpContext.Session.SetString("UserEmail", newUser.Email);

            return RedirectToAction("Index", "Memory");
        }

        // ══════════════════════════════
        // GET: /Account/ResetPassword
        // ══════════════════════════════
        public IActionResult ResetPassword()
        {
            return View();
        }

        // ══════════════════════════════
        // POST: /Account/ResetPassword — 1. adım: email doğrula
        // ══════════════════════════════
        [HttpPost]
        public IActionResult ResetPassword(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                ViewBag.Error = "Email boş bırakılamaz.";
                return View();
            }

            var user = _db.Users.FirstOrDefault(u => u.Email == email);

            if (user == null)
            {
                ViewBag.Error = "Bu email ile kayıtlı hesap bulunamadı.";
                return View();
            }

            // Email doğrulandı — yeni şifre formunu göster
            // Email'i TempData ile taşıyoruz (bir sonraki isteğe kadar yaşar)
            TempData["ResetEmail"] = email;
            return RedirectToAction("NewPassword");
        }

        // ══════════════════════════════
        // GET: /Account/NewPassword
        // ══════════════════════════════
        public IActionResult NewPassword()
        {
            // TempData yoksa yani direkt bu sayfaya gelmeye çalışıyorsa geri gönder
            if (TempData["ResetEmail"] == null)
                return RedirectToAction("ResetPassword");

            // TempData'yı ViewBag'e aktar — View'da kullanacağız
            ViewBag.ResetEmail = TempData["ResetEmail"];
            return View();
        }

        // ══════════════════════════════
        // POST: /Account/NewPassword
        // ══════════════════════════════
        [HttpPost]
        public IActionResult NewPassword(string email, string newPassword, string newPasswordConfirm)
        {
            if (newPassword != newPasswordConfirm)
            {
                ViewBag.Error = "Şifreler eşleşmiyor.";
                ViewBag.ResetEmail = email;
                return View();
            }

            if (newPassword.Length < 6)
            {
                ViewBag.Error = "Şifre en az 6 karakter olmalıdır.";
                ViewBag.ResetEmail = email;
                return View();
            }

            var user = _db.Users.FirstOrDefault(u => u.Email == email);
            if (user == null)
                return RedirectToAction("ResetPassword");

            // Yeni şifreyi hashle ve kaydet
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            _db.SaveChanges();

            ViewBag.Success = "Şifren başarıyla güncellendi! Giriş yapabilirsin.";
            return RedirectToAction("Login");
        }

        // ══════════════════════════════
        // GET: /Account/Logout
        // ══════════════════════════════
        public IActionResult Logout()
        {
            // Tüm session verilerini sil
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}