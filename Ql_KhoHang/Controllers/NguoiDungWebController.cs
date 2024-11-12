using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Text;
using Ql_KhoHang.Dtos;

namespace Ql_KhoHang.Controllers
{
    public class NguoiDungWebController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _apiBaseUrl;

        public NguoiDungWebController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _apiBaseUrl = configuration["ApiSettings:BaseUrl"];
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

            var client = _httpClientFactory.CreateClient();
            var loginUrl = $"{_apiBaseUrl}/api/NguoiDung/Login/login?username=" + username + "&password=" + password;

            var loginData = new
            {
                username = username,
                password = password
            };

            var jsonContent = new StringContent(JsonConvert.SerializeObject(loginData), Encoding.UTF8, "application/json");
            var response = await client.PostAsync(loginUrl, jsonContent);

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var user = JsonConvert.DeserializeObject<NguoiDungWebDtos>(data);

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
                    ModelState.AddModelError(string.Empty, "Login failed: Unable to retrieve user information.");
                }
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
            var employeeId = User.Claims.FirstOrDefault(c => c.Type == "EmployeeId")?.Value;
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
