using Microsoft.AspNetCore.Mvc;
using Ql_KhoHang.Dtos;
using Ql_KhoHang.Services;
using System.Security.Claims;

namespace Ql_KhoHang.Controllers
{
    public class ViTriController : Controller
    {
        private readonly ViTriService _viTriService;

        public ViTriController(ViTriService viTriService)
        {
            _viTriService = viTriService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? keyword, int pageNumber = 1, int pageSize = 10)
        {
            SetUserClaims();
            IEnumerable<VitriDto> allLocations;

            if (!string.IsNullOrEmpty(keyword))
            {
                allLocations = await _viTriService.GetAllAsync();
                allLocations = allLocations.Where(vt => vt.KhuVuc.Contains(keyword, StringComparison.OrdinalIgnoreCase));
                if (!allLocations.Any())
                {
                    TempData["ErrorMessage"] = "Không tìm thấy vị trí nào.";
                }
                else
                {
                    TempData["SuccessMessage"] = $"Tìm thấy {allLocations.Count()} kết quả.";
                }
            }
            else
            {
                allLocations = await _viTriService.GetAllAsync();
            }

            // Phân trang
            var paginatedLocations = allLocations
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Tổng số trang
            int totalPages = (int)Math.Ceiling(allLocations.Count() / (double)pageSize);

            // Truyền dữ liệu tới view
            ViewBag.CurrentPage = pageNumber;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = totalPages;
            ViewBag.Keyword = keyword;

            return View(paginatedLocations);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            SetUserClaims();
            var location = await _viTriService.GetByIdAsync(id);
            if (location == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy vị trí.";
                return RedirectToAction("Index");
            }

            return View(location);
        }

        [HttpGet]
        public IActionResult Create()
        {
            SetUserClaims();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(VitriDto newLocation)
        {
            if (ModelState.IsValid)
            {
                var success = await _viTriService.CreateAsync(newLocation);

                if (success)
                {
                    TempData["SuccessMessage"] = "Tạo vị trí thành công!";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["ErrorMessage"] = "Không thể tạo vị trí.";
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Dữ liệu không hợp lệ.";
            }

            return View(newLocation);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            SetUserClaims();
            var location = await _viTriService.GetByIdAsync(id);
            if (location == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy vị trí.";
                return RedirectToAction("Index");
            }

            return View(location);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, VitriDto updatedLocation)
        {
            if (ModelState.IsValid)
            {
                var success = await _viTriService.UpdateAsync(id, updatedLocation);

                if (success)
                {
                    TempData["SuccessMessage"] = "Cập nhật vị trí thành công!";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["ErrorMessage"] = "Không thể cập nhật vị trí.";
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Dữ liệu không hợp lệ.";
            }

            return View(updatedLocation);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _viTriService.DeleteAsync(id);

            if (success)
            {
                TempData["SuccessMessage"] = "Xóa vị trí thành công!";
            }
            else
            {
                TempData["ErrorMessage"] = "Không thể xóa vị trí.";
            }

            return RedirectToAction("Index");
        }

        private void SetUserClaims()
        {
            ViewBag.Username = User.Identity?.Name;
            ViewBag.Role = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            ViewBag.HinhAnh = User.Claims.FirstOrDefault(c => c.Type == "HinhAnh")?.Value;
        }
    }
}
