# 🪐 Değişen Düşünceler

> _Satürn'ün halkalarında saklı gibi — bazı anılar sadece sana ait, bazıları evrene bırakılmak için yazılır._

Değişen Düşünceler; kullanıcıların günlük düşüncelerini, anılarını ve hislerini dijital ortamda saklayabildikleri ve istediklerinde herkesle paylaşabildikleri bir web platformudur.

---

## 🚀 Özellikler

- 🔐 Kayıt ol / Giriş yap / Şifre sıfırlama
- ✦ Anı yazma, düzenleme ve silme
- 🌌 Evren sayfası — herkese açık anıların akışı
- 🔒 Gizli veya herkese açık anı seçeneği
- ❤️ Emoji tepki sistemi (❤️ 😂 😢 😲 🔥 👏 💡)
- 🔍 Canlı arama ve filtreleme
- 📊 Kişisel istatistik paneli
- ⭐ Görüntülenme sayacı
- 🪐 Gece gökyüzü + Satürn temalı arayüz

---

## 🛠️ Teknolojiler

| Teknoloji                      | Kullanım Amacı               |
| ------------------------------ | ---------------------------- |
| ASP.NET Core MVC (.NET 10)     | Web uygulama çatısı          |
| SQLite + Entity Framework Core | Veritabanı yönetimi          |
| BCrypt.Net-Next                | Şifre hashleme               |
| Bootstrap 5                    | Responsive arayüz            |
| Vanilla JavaScript             | DOM manipülasyonu, fetch API |
| Razor (.cshtml)                | Dinamik HTML şablonları      |
| HTML5 / CSS3                   | Sayfa yapısı ve tema         |

---

## 📂 Proje Yapısı

```
DegisenDusunceler/
├── Controllers/
│   ├── AccountController.cs   # Giriş, kayıt, şifre sıfırlama
│   └── MemoryController.cs    # 5 ana sayfa + emoji tepki
├── Models/
│   ├── User.cs
│   ├── Memory.cs
│   ├── Reaction.cs
│   └── AppDbContext.cs
├── Views/
│   ├── Account/               # Login, Register, ResetPassword, NewPassword
│   ├── Memory/                # Index, MyCapsule, Create, Details, Profile
│   └── Shared/                # _Layout.cshtml, _Navbar.cshtml
├── wwwroot/
│   ├── css/site.css
│   └── js/site.js
└── Program.cs
```

---

## ⚙️ Kurulum

### Gereksinimler

- .NET 10 SDK
- Git

### Adımlar

```bash
# Repoyu klonla
git clone https://github.com/Gulsmilee/Deg-senDusunceler.git
cd Deg-senDusunceler

# Bağımlılıkları yükle
dotnet restore

# Uygulamayı çalıştır
dotnet run
```

Tarayıcıda `https://localhost:xxxx` adresine git. Veritabanı ilk çalıştırmada otomatik oluşturulur.

---

## 🗄️ Veritabanı

SQLite kullanılmaktadır. Uygulama ilk açıldığında `degisendusunceler.db` dosyası otomatik oluşturulur.

**Tablolar:**

- `Users` — Kullanıcı bilgileri
- `Memories` — Anı kayıtları
- `Reactions` — Emoji tepkileri

---

## 📸 Ekran Görüntüleri

| Evren Sayfası                            | Yeni Anı                     | Profil              |
| ---------------------------------------- | ---------------------------- | ------------------- |
| Açık anılar, emoji tepkiler, canlı arama | JS tarih/saat, kelime sayacı | İstatistik kartları |

---

## 🎓 Ders Bilgisi

Bu proje **Web Programlama** dersi kapsamında geliştirilmiştir.

---

_Değişen Düşünceler · 2026_
