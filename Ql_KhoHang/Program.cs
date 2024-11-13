using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
// ??ng ký d?ch v? HttpClient
builder.Services.AddHttpClient();
var connectionString =
builder.Configuration.GetConnectionString("WebsiteQLKhohangConnection");


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
     defaults: new { controller = "NguoiDungWeb", action = "Login" });

    endpoints.MapControllerRoute(
     name: "trang-chu-admin",
     pattern: "trang-chu-admin",
     defaults: new { controller = "NguoiDungWeb", action = "Index" });

    endpoints.MapControllerRoute(
     name: "loai-san-pham",
     pattern: "loai-san-pham",
     defaults: new { controller = "LoaiSanPhamWeb", action = "Index" });

    endpoints.MapControllerRoute(
     name: "san-pham",
     pattern: "san-pham",
     defaults: new { controller = "SanPhamWeb", action = "Index" });

});
app.Run();
