using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Ql_KhoHang.Dtos;
using Ql_KhoHang.Services;
using System.Security.Claims;

namespace Ql_KhoHang.Controllers
{
    public class PhieuXuatHangController : Controller
    {
        private readonly PhieuXuatHangService _exportOrderService;
        private readonly SanPhamService _sanPhamService;
        private readonly KhachHangService _khachHangService;
        private readonly ChiTietPhieuXuatHangService _exportOrderDetailService;

        public PhieuXuatHangController(PhieuXuatHangService exportOrderService, ChiTietPhieuXuatHangService exportOrderDetailService,
                                       SanPhamService sanPhamService, KhachHangService khachHangService)
        {
            _exportOrderService = exportOrderService;
            _sanPhamService = sanPhamService;
            _khachHangService = khachHangService;
            _exportOrderDetailService = exportOrderDetailService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? keyword, DateTime? startDate, DateTime? endDate, int pageNumber = 1, int pageSize = 10)
        {
            SetUserClaims();

            // Lấy danh sách phiếu xuất hàng
            var exportOrders = await _exportOrderService.GetAllAsync(keyword);

            // Lọc theo ngày nếu có startDate và endDate
            if (startDate.HasValue)
            {
                exportOrders = exportOrders.Where(o => o.NgayXuat >= startDate.Value).ToList();
            }
            if (endDate.HasValue)
            {
                var adjustedEndDate = endDate.Value.Date.AddDays(1).AddTicks(-1);
                exportOrders = exportOrders.Where(o => o.NgayXuat <= adjustedEndDate).ToList();
            }

            // Phân trang
            var paginatedOrders = exportOrders.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            int totalPages = (int)Math.Ceiling(exportOrders.Count / (double)pageSize);

            // Truyền dữ liệu vào ViewBag để hiển thị
            ViewBag.CurrentPage = pageNumber;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = totalPages;
            ViewBag.Keyword = keyword;
            ViewBag.StartDate = startDate;
            ViewBag.EndDate = endDate;

            return View(paginatedOrders);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            SetUserClaims();
            var exportOrder = await _exportOrderService.GetByIdAsync(id);
            if (exportOrder == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy phiếu xuất hàng.";
                return RedirectToAction("Index");
            }

            var details = await _exportOrderDetailService.GetByExportOrderIdAsync(id);

            ViewBag.Details = details; // Danh sách chi tiết phiếu xuất
            return View(exportOrder);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            SetUserClaims();

            // Sử dụng await để lấy dữ liệu thực tế từ các phương thức bất đồng bộ
            ViewBag.Products = await _sanPhamService.GetAllAsync();
            ViewBag.Customers = await _khachHangService.GetAllAsync();

            return View(new PhieuXuatHangDto());
        }

        [HttpPost]
        public async Task<IActionResult> Create(PhieuXuatHangDto newOrder)
        {
            if (ModelState.IsValid)
            {
                // Lấy mã người dùng từ claim
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "MaNguoiDung")?.Value;

                if (!string.IsNullOrEmpty(userIdClaim) && int.TryParse(userIdClaim, out var userId))
                {
                    newOrder.MaNguoiDung = userId; // Gắn mã người dùng
                    newOrder.NgayXuat = DateTime.Now; // Gắn ngày xuất hiện tại
                    newOrder.TrangThai = 1;
                    newOrder.PhiVanChuyen= 0;
                }

                // Gọi service để tạo mới phiếu xuất hàng và chi tiết phiếu
                var success = await _exportOrderService.CreateAsync(newOrder);

                if (success)
                {
                    TempData["SuccessMessage"] = "Thêm mới phiếu xuất hàng thành công!";
                    return RedirectToAction("Index");
                }

                TempData["ErrorMessage"] = "Không thể thêm mới phiếu xuất hàng.";
            }

            // Nạp lại dữ liệu khách hàng và sản phẩm nếu có lỗi
            ViewBag.Products = await _sanPhamService.GetAllAsync();
            ViewBag.Customers = await _khachHangService.GetAllAsync();

            return View(newOrder);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            SetUserClaims();
            ViewBag.Products = await _sanPhamService.GetAllAsync();
            ViewBag.Customers = await _khachHangService.GetAllAsync();

            // Lấy thông tin phiếu xuất hàng
            var exportOrder = await _exportOrderService.GetByIdAsync(id);
            if (exportOrder == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy phiếu xuất hàng.";
                return RedirectToAction("Index");
            }

            // Lấy danh sách chi tiết phiếu xuất hàng
            var details = await _exportOrderDetailService.GetByExportOrderIdAsync(id);

            // Thêm danh sách chi tiết vào DTO phiếu xuất
            exportOrder.Details = details; // Đảm bảo `exportOrder` có thuộc tính `Details`

            return View(exportOrder);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, PhieuXuatHangDto updatedOrder, List<ChiTietPhieuXuatHangDto> details)
        {

            // Kiểm tra trạng thái ModelState
            if (!ModelState.IsValid)
            {
                ViewBag.Products = await _sanPhamService.GetAllAsync();
                ViewBag.Customers = await _khachHangService.GetAllAsync();
                return View(updatedOrder);
            }
            var oldOrder = await _exportOrderService.GetByIdAsync(updatedOrder.MaPhieuXuatHang);
            updatedOrder.NgayXuat = oldOrder.NgayXuat;
            // Lấy mã người dùng từ Claim
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "MaNguoiDung")?.Value;
            if (!string.IsNullOrEmpty(userIdClaim) && int.TryParse(userIdClaim, out var userId))
            {
                updatedOrder.MaNguoiDung = userId;
            }

            // Đảm bảo gán mã phiếu xuất hàng cho từng chi tiết
            foreach (var detail in details)
            {
                detail.MaPhieuXuatHang = updatedOrder.MaPhieuXuatHang;
            }

            // Lấy danh sách chi tiết hiện tại từ cơ sở dữ liệu
            var existingDetails = await _exportOrderDetailService.GetByExportOrderIdAsync(id);

            // Tìm các chi tiết cần thêm mới
            var newDetails = details
                .Where(d => existingDetails.All(ed => ed.MaSanPham != d.MaSanPham))
                .ToList();

            // Tìm các chi tiết cần cập nhật
            var updatedDetails = details
                .Where(d => existingDetails.Any(ed => ed.MaSanPham == d.MaSanPham))
                .ToList();

            // Tìm các chi tiết cần xóa
            var deletedDetails = existingDetails
                .Where(ed => details.All(d => d.MaSanPham != ed.MaSanPham))
                .ToList();

            // Gọi Service để cập nhật
            var success = await _exportOrderService.UpdateAsync(id, updatedOrder, newDetails, updatedDetails, deletedDetails);
            if (success)
            {
                TempData["SuccessMessage"] = "Cập nhật phiếu xuất hàng thành công!";
                return RedirectToAction("Index");
            }

            // Xử lý khi có lỗi
            TempData["ErrorMessage"] = "Không thể cập nhật phiếu xuất hàng.";
            ViewBag.Products = await _sanPhamService.GetAllAsync();
            ViewBag.Customers = await _khachHangService.GetAllAsync();
            return View(updatedOrder);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _exportOrderService.DeleteAsync(id);
            if (success)
            {
                TempData["SuccessMessage"] = "Xóa phiếu xuất hàng thành công!";
            }
            else
            {
                TempData["ErrorMessage"] = "Không thể xóa phiếu xuất hàng.";
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
