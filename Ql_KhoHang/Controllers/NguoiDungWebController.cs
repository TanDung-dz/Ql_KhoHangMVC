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

            if (user != null && user.MaNguoiDung > 0)
            {
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
            SetUserClaims();

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            // Đăng xuất người dùng bằng cách xóa cookie đăng nhập
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
        [HttpGet]
        public async Task<IActionResult> Index2(string? keyword, int pageNumber = 1, int pageSize = 10)
        {
            SetUserClaims();
            IEnumerable<NguoiDungWebDtos> allUsers;

            if (!string.IsNullOrEmpty(keyword))
            {
                // Tìm kiếm người dùng theo từ khóa
                allUsers = await _nguoiDungService.SearchAsync(keyword);
                if (!allUsers.Any())
                {
                    TempData["ErrorMessage"] = "Không tìm thấy người dùng nào.";
                }
                else
                {
                    TempData["SuccessMessage"] = $"Tìm thấy {allUsers.Count()} kết quả.";
                }
            }
            else
            {
                // Lấy tất cả người dùng nếu không có từ khóa tìm kiếm
                allUsers = await _nguoiDungService.GetAllAsync();
            }
            Console.WriteLine("Dữ liệu người dùng: " + string.Join(", ", allUsers.Select(u => u.TenNguoiDung)));

            // Phân trang
            var paginatedUsers = allUsers
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Tính tổng số trang
            int totalPages = (int)Math.Ceiling(allUsers.Count() / (double)pageSize);

            // Gửi thông tin phân trang tới View
            ViewBag.CurrentPage = pageNumber;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = totalPages;
            ViewBag.Keyword = keyword; // Để giữ từ khóa tìm kiếm trong URL

            return View(paginatedUsers);
        }


        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            SetUserClaims();
            var user = await _nguoiDungService.GetByIdAsync(id);
            if (user == null)
            {
                TempData["ErrorMessage"] = "Người dùng không tồn tại.";
                return RedirectToAction("IndexAdmin");
            }
            return View(user);
        }

        // Action CreateAdmin - Thêm người dùng mới
        [HttpGet]
        public IActionResult Create()
        {
            var model = new NguoiDungWebDtos(); // Khởi tạo một instance của Model
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(NguoiDungWebDtos newUser)
        {
            if (ModelState.IsValid)
            {
                var success = await _nguoiDungService.CreateAsync(newUser);

                if (success)
                {
                    TempData["SuccessMessage"] = "Tạo người dùng thành công!";
                    return RedirectToAction("Index2");
                }
                else
                {
                    TempData["ErrorMessage"] = "Không thể tạo người dùng.";
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Dữ liệu không hợp lệ.";
            }
            return View(newUser);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            SetUserClaims();
            var user = await _nguoiDungService.GetByIdAsync(id);
            if (user == null)
            {
                TempData["ErrorMessage"] = "Người dùng không tồn tại.";
                return RedirectToAction("Index2");
            }
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, NguoiDungWebDtos updatedUser)
        {
            if (ModelState.IsValid)
            {
                var success = await _nguoiDungService.UpdateAsync(id, updatedUser);

                if (success)
                {
                    TempData["SuccessMessage"] = "Cập nhật thông tin người dùng thành công!";
                    return RedirectToAction("Index2");
                }
                else
                {
                    TempData["ErrorMessage"] = "Không thể cập nhật người dùng.";
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Dữ liệu không hợp lệ.";
            }

            return View(updatedUser);
        }

        // Action DeleteAdmin - Xóa người dùng
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _nguoiDungService.DeleteAsync(id);

            if (success)
            {
                TempData["SuccessMessage"] = "Xóa người dùng thành công!";
            }
            else
            {
                TempData["ErrorMessage"] = "Không thể xóa người dùng.";
            }

            return RedirectToAction("Index2");
        }
        public async Task<IActionResult> _MenuPartial()
        {
            return PartialView();
        }

        public async Task<IActionResult> _SidebarPartial()
        {
            return PartialView();
        }
        private void SetUserClaims()
        {
            ViewBag.Username = User.Identity?.Name;
            ViewBag.Role = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
        }
    }
}
