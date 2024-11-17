using Microsoft.AspNetCore.Mvc;
using Ql_KhoHang.Dtos;
using Ql_KhoHang.Services;
using System.Security.Claims;

namespace Ql_KhoHang.Controllers
{
    public class HangSanXuatWebController : Controller
    {
        private readonly HangSanXuatService _hangSanXuatService;

        public HangSanXuatWebController(HangSanXuatService hangSanXuatService)
        {
            _hangSanXuatService = hangSanXuatService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? keyword, int pageNumber = 1, int pageSize = 10)
        {
            SetUserClaims();
            IEnumerable<HangSanXuatWebDtos> allManufacturers;

            if (!string.IsNullOrEmpty(keyword))
            {
                allManufacturers = await _hangSanXuatService.SearchAsync(keyword);
                if (!allManufacturers.Any())
                {
                    TempData["ErrorMessage"] = "Không tìm thấy hãng sản xuất nào.";
                }
                else
                {
                    TempData["SuccessMessage"] = $"Tìm thấy {allManufacturers.Count()} kết quả.";
                }
            }
            else
            {
                allManufacturers = await _hangSanXuatService.GetAllAsync();
            }

            // Tính toán dữ liệu phân trang
            var paginatedManufacturers = allManufacturers
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Tính tổng số trang
            int totalPages = (int)Math.Ceiling(allManufacturers.Count() / (double)pageSize);

            // Gửi thông tin phân trang tới View
            ViewBag.CurrentPage = pageNumber;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = totalPages;
            ViewBag.Keyword = keyword; // Để giữ từ khóa tìm kiếm

            return View(paginatedManufacturers);
        }


        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            SetUserClaims();
            var manufacturer = await _hangSanXuatService.GetByIdAsync(id);
            return View(manufacturer);
        }

        [HttpGet]
        public async Task<IActionResult> Search(string keyword)
        {
            SetUserClaims();
            var manufacturers = await _hangSanXuatService.SearchAsync(keyword);
            return View("Index", manufacturers);
        }

        [HttpGet]
        public IActionResult Create()
        {
            SetUserClaims();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(HangSanXuatWebDtos newManufacturer)
        {
            if (ModelState.IsValid)
            {
                var success = await _hangSanXuatService.CreateAsync(newManufacturer);

                if (success)
                {
                    TempData["SuccessMessage"] = "Tạo nhà sản xuất thành công!";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["ErrorMessage"] = "Không thể tạo nhà sản xuất.";
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Dữ liệu không hợp lệ.";
            }

            return View(newManufacturer);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            SetUserClaims();
            var manufacturer = await _hangSanXuatService.GetByIdAsync(id);
            return View(manufacturer);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, HangSanXuatWebDtos manufacturer)
        {
            if (ModelState.IsValid)
            {
                var success = await _hangSanXuatService.UpdateAsync(id, manufacturer);

                if (success)
                {
                    TempData["SuccessMessage"] = "Cập nhật nhà sản xuất thành công!";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["ErrorMessage"] = "Không thể cập nhật nhà sản xuất.";
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Dữ liệu không hợp lệ.";
            }

            return View(manufacturer);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _hangSanXuatService.DeleteAsync(id);

            if (success)
            {
                TempData["SuccessMessage"] = "Xóa nhà sản xuất thành công!";
            }
            else
            {
                TempData["ErrorMessage"] = "Không thể xóa nhà sản xuất.";
            }

            return RedirectToAction("Index");
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
