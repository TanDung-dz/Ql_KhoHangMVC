using Microsoft.AspNetCore.Authentication.Cookies;
using Ql_KhoHang.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
// ??ng ký d?ch v? HttpClient
builder.Services.AddHttpClient();
var connectionString =
builder.Configuration.GetConnectionString("WebsiteQLKhohangConnection");


builder.Services.AddScoped<LoaiSanPhamService>();
builder.Services.AddScoped<NguoiDungService>();
builder.Services.AddScoped<SanPhamService>();
builder.Services.AddScoped<HangSanXuatService>();
builder.Services.AddScoped<MenuService>();
builder.Services.AddScoped<LoaiKhachHangService>();
builder.Services.AddScoped<BlogService>();
builder.Services.AddScoped<NhaCungCapService>();
builder.Services.AddScoped<KhachHangService>();
builder.Services.AddScoped<NhanVienKhoService>();
builder.Services.AddScoped<ViTriService>();
builder.Services.AddScoped<PhieuNhapHangService>();
builder.Services.AddScoped<ChiTietPhieuNhapHangService>();
builder.Services.AddScoped<PhieuXuatHangService>();
builder.Services.AddScoped<ChiTietPhieuXuatHangService>();
builder.Services.AddScoped<KiemKeService>();
builder.Services.AddScoped<ChiTietKiemKeService>();


builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).
AddCookie(options =>
{
    options.Cookie.Name = "QLKhoHangCookie";
    options.LoginPath = "/User/Login";
});

var app = builder.Build();
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
    name: "trang-chu",
    pattern: "trang-chu",
    defaults: new { controller = "Home", action = "Index" });

    endpoints.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

    endpoints.MapControllerRoute(
     name: "dang-nhap",
     pattern: "dang-nhap",
     defaults: new { controller = "NguoiDung", action = "Login" });

    endpoints.MapControllerRoute(
     name: "trang-chu-admin",
     pattern: "trang-chu-admin",
     defaults: new { controller = "NguoiDung", action = "Index" });

    endpoints.MapControllerRoute(
     name: "loai-san-pham",
     pattern: "loai-san-pham",
     defaults: new { controller = "LoaiSanPham", action = "Index" });

    endpoints.MapControllerRoute(
     name: "san-pham",
     pattern: "san-pham",
     defaults: new { controller = "SanPham", action = "Index" });

});
app.Run();
