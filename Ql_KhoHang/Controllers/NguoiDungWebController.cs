using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Ql_KhoHang.Dtos;
using Ql_KhoHang.Services;
using System.Security.Claims;

namespace Ql_KhoHang.Controllers
{
    public class NguoiDungWebController : Controller
    {
        private readonly NguoiDungService _nguoiDungService;

        public NguoiDungWebController(NguoiDungService nguoiDungService)
        {
            _nguoiDungService = nguoiDungService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError(string.Empty, "Username and password are required.");
                return View();
            }

            var user = await _nguoiDungService.LoginAsync(username, password);

            if (user != null)
            {
                // Thiết lập thông tin đăng nhập dưới dạng Claims
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.TenNguoiDung ?? ""),
                    new Claim("MaNguoiDung", user.MaNguoiDung.ToString()),
                    new Claim(ClaimTypes.Role, user.Quyen.ToString() ?? "0")
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true
                };

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

                return RedirectToAction("Index", "NguoiDungWeb");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }

            return View();
        }

        [HttpGet]
        public IActionResult Index()
        {
            // Truy xuất thông tin người dùng từ Claims
            var employeeId = User.Claims.FirstOrDefault(c => c.Type == "MaNguoiDung")?.Value;
            var name = User.Identity?.Name;
            var role = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            // Tạo một ViewModel để truyền thông tin vào View
            var userViewModel = new NguoiDungWebDtos
            {
                MaNguoiDung = int.Parse(employeeId ?? "0"),
                TenNguoiDung = name,
                Quyen = int.Parse(role ?? "0")
            };

            return View(userViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            // Đăng xuất người dùng bằng cách xóa cookie đăng nhập
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        public async Task<IActionResult> _MenuPartial()
        {
            return PartialView();
        }

        public async Task<IActionResult> _SidebarPartial()
        {
            return PartialView();
        }
    }
}
