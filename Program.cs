using Microsoft.EntityFrameworkCore;
using DegisenDusunceler.Models;

var builder = WebApplication.CreateBuilder(args);

// MVC desteği
builder.Services.AddControllersWithViews();

// SQLite bağlantısı — degisendusunceler.db dosyası otomatik oluşacak
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=degisendusunceler.db"));

// Session — giriş yapan kullanıcıyı sayfalar arası tanımak için
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();  // wwwroot klasörü aktif
app.UseRouting();
app.UseSession();      // mutlaka UseRouting'den sonra gelmeli
app.UseAuthorization();

// Uygulama açılınca direkt Login sayfasına git
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

// Veritabanını otomatik oluştur
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

app.Run();