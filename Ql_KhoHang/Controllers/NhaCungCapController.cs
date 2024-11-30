using Microsoft.AspNetCore.Mvc;
using Ql_KhoHang.Dtos;
using Ql_KhoHang.Services;
using System.Security.Claims;

namespace Ql_KhoHang.Controllers
{
    public class NhaCungCapController : Controller
    {
        private readonly NhaCungCapService _nhaCungCapService;

        public NhaCungCapController(NhaCungCapService nhaCungCapService)
        {
            _nhaCungCapService = nhaCungCapService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? keyword, int pageNumber = 1, int pageSize = 9)
        {
            SetUserClaims();
            IEnumerable<NhacungcapDto> allSuppliers;

            if (!string.IsNullOrEmpty(keyword))
            {
                allSuppliers = await _nhaCungCapService.SearchAsync(keyword);
                if (!allSuppliers.Any())
                {
                    TempData["ErrorMessage"] = "Không tìm thấy nhà cung cấp nào.";
                }
                else
                {
                    TempData["SuccessMessage"] = $"Tìm thấy {allSuppliers.Count()} kết quả.";
                }
            }
            else
            {
                allSuppliers = await _nhaCungCapService.GetAllAsync();
            }

            // Tính toán dữ liệu phân trang
            var paginatedSuppliers = allSuppliers
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Tính tổng số trang
            int totalPages = (int)Math.Ceiling(allSuppliers.Count() / (double)pageSize);

            // Gửi thông tin phân trang tới View
            ViewBag.CurrentPage = pageNumber;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = totalPages;
            ViewBag.Keyword = keyword; // Để giữ từ khóa trong input tìm kiếm

            return View(paginatedSuppliers);
        }


        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            SetUserClaims();
            var supplier = await _nhaCungCapService.GetByIdAsync(id);
            if (supplier == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy nhà cung cấp.";
                return RedirectToAction("Index");
            }
            return View(supplier);
        }

        [HttpGet]
        public IActionResult Create()
        {
            SetUserClaims();
            return View(new NhacungcapDto());
        }

        [HttpPost]
        public async Task<IActionResult> Create(NhacungcapDto newSupplier, IFormFile Img)
        {
            if (ModelState.IsValid)
            {
                var success = await _nhaCungCapService.CreateAsync(newSupplier, Img);
                if (success)
                {
                    TempData["SuccessMessage"] = "Thêm mới nhà cung cấp thành công!";
                    return RedirectToAction("Index");
                }
                TempData["ErrorMessage"] = "Không thể thêm mới nhà cung cấp.";
            }
            return View(newSupplier);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            SetUserClaims();
            var supplier = await _nhaCungCapService.GetByIdAsync(id);
            if (supplier == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy nhà cung cấp.";
                return RedirectToAction("Index");
            }
            return View(supplier);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, NhacungcapDto updatedSupplier, IFormFile Img)
        {

            if (ModelState.IsValid)
            {
                var success = await _nhaCungCapService.UpdateAsync(id, updatedSupplier, Img);
                if (success)
                {
                    TempData["SuccessMessage"] = "Cập nhật nhà cung cấp thành công!";
                    return RedirectToAction("Index");
                }
                TempData["ErrorMessage"] = "Không thể cập nhật nhà cung cấp.";
            }
            return View(updatedSupplier);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _nhaCungCapService.DeleteAsync(id);
            if (success)
            {
                TempData["SuccessMessage"] = "Xóa nhà cung cấp thành công!";
            }
            else
            {
                TempData["ErrorMessage"] = "Không thể xóa nhà cung cấp.";
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
            ViewBag.HinhAnh = User.Claims.FirstOrDefault(c => c.Type == "HinhAnh")?.Value;
        }
    }
}
