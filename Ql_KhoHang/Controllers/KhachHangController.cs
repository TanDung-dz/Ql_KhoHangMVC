using Microsoft.AspNetCore.Mvc;
using Ql_KhoHang.Dtos;
using Ql_KhoHang.Services;
using System.Security.Claims;

namespace Ql_KhoHang.Controllers
{
    public class KhachHangController : Controller
    {
        private readonly KhachHangService _khachHangService;
        private readonly LoaiKhachHangService _loaiKhachHangService;

        public KhachHangController(KhachHangService khachHangService, LoaiKhachHangService loaiKhachHangService)
        {
            _khachHangService = khachHangService;
            _loaiKhachHangService = loaiKhachHangService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string keyword, int pageNumber = 1, int pageSize = 9)
        {
            SetUserClaims();

            // Lấy tất cả khách hàng từ API và tìm kiếm nếu có từ khóa
            var allCustomers = string.IsNullOrEmpty(keyword)
                ? await _khachHangService.GetAllAsync()  // Nếu không có từ khóa tìm kiếm, lấy tất cả khách hàng
                : await _khachHangService.SearchAsync(keyword);  // Nếu có từ khóa tìm kiếm, gọi search

            // Tính toán dữ liệu phân trang
            var paginatedCustomers = allCustomers
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Tính tổng số trang
            int totalPages = (int)Math.Ceiling(allCustomers.Count / (double)pageSize);

            // Gửi thông tin phân trang và từ khóa tìm kiếm vào View
            ViewBag.CurrentPage = pageNumber;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = totalPages;
            ViewBag.Keyword = keyword;  // Gửi từ khóa tìm kiếm vào View để giữ lại khi tải lại trang

            return View(paginatedCustomers);
        }


        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            SetUserClaims();
            var customer = await _khachHangService.GetByIdAsync(id);
            return View(customer);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            SetUserClaims();
            // Lấy danh sách loại khách hàng
            var loaiKhachHangs = await _loaiKhachHangService.GetAllAsync();

            // Gửi dữ liệu đến View thông qua ViewBag
            ViewBag.LoaiKhachHangs = loaiKhachHangs;
            return View(new KhachHangDto());
        }

        [HttpPost]
        public async Task<IActionResult> Create(KhachHangDto newCustomer)
        {
            if (ModelState.IsValid)
            {
                // Gọi service để lưu khách hàng mới
                var success = await _khachHangService.CreateAsync(newCustomer);

                if (success)
                {
                    TempData["SuccessMessage"] = "Thêm khách hàng thành công!";
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Failed to create customer.");
                }
            }

            // Nếu có lỗi, tải lại danh sách Loại Khách Hàng
            ViewBag.LoaiKhachHangs = await _loaiKhachHangService.GetAllAsync();

            return View(newCustomer);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            SetUserClaims();
            var customer = await _khachHangService.GetByIdAsync(id);
            // Lấy danh sách loại khách hàng
            var loaiKhachHangs = await _loaiKhachHangService.GetAllAsync();

            // Gửi dữ liệu đến View thông qua ViewBag
            ViewBag.LoaiKhachHangs = loaiKhachHangs;
            return View(customer);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, KhachHangDto customer)
        {
            if (ModelState.IsValid)
            {
                var success = await _khachHangService.UpdateAsync(id, customer);

                if (success)
                {
                    TempData["SuccessMessage"] = "Sửa khách hàng thành công!";
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Failed to update customer.");
                }
            }

            // Nếu có lỗi, tải lại danh sách Loại Khách Hàng
            ViewBag.LoaiKhachHangs = await _loaiKhachHangService.GetAllAsync();

            return View(customer);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _khachHangService.DeleteAsync(id);

            if (success)
            {
				TempData["SuccessMessage"] = "Xóa khách hàng thành công!";
				return RedirectToAction("Index");
			}
            else
				ModelState.AddModelError(string.Empty, "Failed to delete customer.");
			return View("Index");
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
