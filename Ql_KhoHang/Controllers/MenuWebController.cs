using Microsoft.AspNetCore.Mvc;
using Ql_KhoHang.Dtos;
using Ql_KhoHang.Services;
using System.Security.Claims;

namespace Ql_KhoHang.Controllers
{
    public class MenuWebController : Controller
    {
        private readonly MenuService _menuService;

        public MenuWebController(MenuService menuService)
        {
            _menuService = menuService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? keyword, int pageNumber = 1, int pageSize = 10)
        {
            SetUserClaims();
            IEnumerable<MenuWebDtos> allMenus;

            if (!string.IsNullOrEmpty(keyword))
            {
                allMenus = await _menuService.SearchAsync(keyword);
                if (!allMenus.Any())
                {
                    TempData["ErrorMessage"] = "Không tìm thấy menu nào.";
                }
                else
                {
                    TempData["SuccessMessage"] = $"Tìm thấy {allMenus.Count()} kết quả.";
                }
            }
            else
            {
                allMenus = await _menuService.GetAllAsync();
            }

            // Tính toán dữ liệu phân trang
            var paginatedMenus = allMenus
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Tính tổng số trang
            int totalPages = (int)Math.Ceiling(allMenus.Count() / (double)pageSize);

            // Gửi thông tin phân trang tới View
            ViewBag.CurrentPage = pageNumber;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = totalPages;
            ViewBag.Keyword = keyword; // Để giữ từ khóa trong input tìm kiếm

            return View(paginatedMenus);
        }


        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            SetUserClaims();
            var menu = await _menuService.GetByIdAsync(id);
            return View(menu);
        }

        [HttpGet]
        public IActionResult Create()
        {
            SetUserClaims();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(MenuWebDtos newMenu)
        {
            // Lấy thông tin người dùng từ Claims
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "MaNguoiDung")?.Value;

            if (int.TryParse(userIdClaim, out var userId))
            {
                newMenu.MaNguoiDung = userId;
            }
            else
            {
                TempData["ErrorMessage"] = "Không thể lấy thông tin người dùng.";
                return RedirectToAction("Index");
            }
            if (ModelState.IsValid)
            {
                var success = await _menuService.CreateAsync(newMenu);

                if (success)
                {
                    TempData["SuccessMessage"] = "Tạo menu thành công!";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["ErrorMessage"] = "Không thể tạo menu.";
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Dữ liệu không hợp lệ.";
            }

            return View(newMenu);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            SetUserClaims();
            var menu = await _menuService.GetByIdAsync(id);
            return View(menu);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, MenuWebDtos menu)
        {
            // Lấy thông tin người dùng từ Claims
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "MaNguoiDung")?.Value;

            if (int.TryParse(userIdClaim, out var userId))
            {
                menu.MaNguoiDung = userId;
            }
            else
            {
                TempData["ErrorMessage"] = "Không thể lấy thông tin người dùng.";
                return RedirectToAction("Index");
            }
            if (ModelState.IsValid)
            {
                var success = await _menuService.UpdateAsync(id, menu);

                if (success)
                {
                    TempData["SuccessMessage"] = "Cập nhật menu thành công!";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["ErrorMessage"] = "Không thể cập nhật menu.";
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Dữ liệu không hợp lệ.";
            }

            return View(menu);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _menuService.DeleteAsync(id);

            if (success)
            {
                TempData["SuccessMessage"] = "Xóa menu thành công!";
            }
            else
            {
                TempData["ErrorMessage"] = "Không thể xóa menu.";
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Search(string keyword)
        {
            SetUserClaims();
            var menus = await _menuService.SearchAsync(keyword);

            if (menus == null || !menus.Any())
            {
                TempData["ErrorMessage"] = "Không tìm thấy menu.";
            }
            else
            {
                TempData["SuccessMessage"] = $"Tìm thấy {menus.Count()} kết quả.";
            }

            return View("Index", menus);
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
