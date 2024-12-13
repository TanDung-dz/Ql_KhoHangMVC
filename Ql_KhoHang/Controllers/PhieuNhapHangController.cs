using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Ql_KhoHang.Dtos;
using Ql_KhoHang.Services;
using System.Security.Claims;

namespace Ql_KhoHang.Controllers
{
    public class PhieuNhapHangController : Controller
    {
        private readonly PhieuNhapHangService _importOrderService;
        private readonly SanPhamService _sanPhamService;
        private readonly NhaCungCapService _nccService;
        private readonly ChiTietPhieuNhapHangService _importOrderDetailService;

        public PhieuNhapHangController(PhieuNhapHangService importOrderService, ChiTietPhieuNhapHangService importOrderDetailService, 
                                                    SanPhamService sanPhamService, NhaCungCapService nhaCungCapService)
        {
            _importOrderService = importOrderService;
            _sanPhamService = sanPhamService;
            _nccService = nhaCungCapService;
            _importOrderDetailService = importOrderDetailService;
        }
        [HttpGet]
        public async Task<IActionResult> Index(string? keyword, DateTime? startDate, DateTime? endDate, int pageNumber = 1, int pageSize = 10)
        {
            SetUserClaims();

            // Gọi service để tìm kiếm nếu có từ khóa
            List<PhieuNhapHangDto> importOrders;
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                importOrders = await _importOrderService.Search(keyword);
            }
            else
            {
                importOrders = await _importOrderService.GetAllAsync();
            }

            // Lọc theo ngày nếu cung cấp
            if (startDate.HasValue)
            {
                importOrders = importOrders.Where(p => p.NgayNhap >= startDate.Value).ToList();
            }
            if (endDate.HasValue)
            {
                var adjustedEndDate = endDate.Value.Date.AddDays(1).AddTicks(-1);
                importOrders = importOrders.Where(p => p.NgayNhap <= adjustedEndDate).ToList();
            }

            // Phân trang
            var paginatedOrders = importOrders
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            int totalPages = (int)Math.Ceiling(importOrders.Count / (double)pageSize);

            // Gửi dữ liệu sang ViewBag
            ViewBag.CurrentPage = pageNumber;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = totalPages;
            ViewBag.Keyword = keyword;
            ViewBag.StartDate = startDate;
            ViewBag.EndDate = endDate;

            return View(paginatedOrders); // Truyền danh sách phiếu nhập đã lọc tới view
        }



        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            SetUserClaims();
            var importOrder = await _importOrderService.GetByIdAsync(id);
            if (importOrder == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy phiếu nhập hàng.";
                return RedirectToAction("Index");
            }

            var details = await _importOrderDetailService.GetByImportOrderIdAsync(id);

            ViewBag.Details = details; // Danh sách chi tiết phiếu nhập
            return View(importOrder);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            SetUserClaims();

            // Sử dụng await để lấy dữ liệu thực tế từ các phương thức bất đồng bộ
            ViewBag.Products = await _sanPhamService.GetAllAsync();
            ViewBag.Suppliers = await _nccService.GetAllAsync();

            return View(new PhieuNhapHangDto());
        }


        [HttpPost]
        public async Task<IActionResult> Create(PhieuNhapHangDto newOrder)
        {
            
            if (ModelState.IsValid)
            {
                // Lấy mã người dùng từ claim
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "MaNguoiDung")?.Value;

                if (!string.IsNullOrEmpty(userIdClaim) && int.TryParse(userIdClaim, out var userId))
                {
                    newOrder.MaNguoiDung = userId; // Gắn mã người dùng
                    newOrder.NgayNhap = DateTime.Now; // Gắn ngày nhập hiện tại
                    newOrder.TrangThai = 1;
                }

                // Gọi service để tạo mới phiếu nhập hàng và chi tiết phiếu
                var success = await _importOrderService.CreateAsync(newOrder);

                if (success)
                {
                    TempData["SuccessMessage"] = "Thêm mới phiếu nhập hàng thành công!";
                    return RedirectToAction("Index");
                }

                TempData["ErrorMessage"] = "Không thể thêm mới phiếu nhập hàng.";
            }

            // Nạp lại dữ liệu nhà cung cấp và sản phẩm nếu có lỗi
            ViewBag.Products = await _sanPhamService.GetAllAsync();
            ViewBag.Suppliers = await _nccService.GetAllAsync();

            return View(newOrder);
        }



        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            SetUserClaims();
            ViewBag.Products = await _sanPhamService.GetAllAsync();
            ViewBag.Suppliers = await _nccService.GetAllAsync();
            // Lấy thông tin phiếu nhập hàng
            var importOrder = await _importOrderService.GetByIdAsync(id);
            if (importOrder == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy phiếu nhập hàng.";
                return RedirectToAction("Index");
            }

            // Lấy danh sách chi tiết phiếu nhập hàng
            var details = await _importOrderDetailService.GetByImportOrderIdAsync(id);

            // Thêm danh sách chi tiết vào DTO phiếu nhập
            importOrder.Details = details; // Đảm bảo `importOrder` có thuộc tính `Details`

            return View(importOrder);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(int id, PhieuNhapHangDto updatedOrder, List<ChiTietPhieuNhapHangDto> details)
        {
            // Kiểm tra trạng thái ModelState
            if (!ModelState.IsValid)
            {
                ViewBag.Products = await _sanPhamService.GetAllAsync();
                ViewBag.Suppliers = await _nccService.GetAllAsync();
                return View(updatedOrder);
            }
            var oldOrder = await _importOrderService.GetByIdAsync(updatedOrder.MaPhieuNhapHang);
            updatedOrder.NgayNhap = oldOrder.NgayNhap;
            // Lấy mã người dùng từ Claim
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "MaNguoiDung")?.Value;
            if (!string.IsNullOrEmpty(userIdClaim) && int.TryParse(userIdClaim, out var userId))
            {
                updatedOrder.MaNguoiDung = userId;
            }

            // Đảm bảo gán mã phiếu nhập hàng cho từng chi tiết
            foreach (var detail in details)
            {
                detail.MaPhieuNhapHang = updatedOrder.MaPhieuNhapHang;
            }

            // Lấy danh sách chi tiết hiện tại từ cơ sở dữ liệu
            var existingDetails = await _importOrderDetailService.GetByImportOrderIdAsync(id);

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
            var success = await _importOrderService.UpdateAsync(id, updatedOrder, newDetails, updatedDetails, deletedDetails);
            if (success)
            {
                TempData["SuccessMessage"] = "Cập nhật phiếu nhập hàng thành công!";
                return RedirectToAction("Index");
            }

            // Xử lý khi có lỗi
            TempData["ErrorMessage"] = "Không thể cập nhật phiếu nhập hàng.";
            ViewBag.Products = await _sanPhamService.GetAllAsync();
            ViewBag.Suppliers = await _nccService.GetAllAsync();
            return View(updatedOrder);
        }




        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _importOrderService.DeleteAsync(id);
            if (success)
            {
                TempData["SuccessMessage"] = "Xóa phiếu nhập hàng thành công!";
            }
            else
            {
                TempData["ErrorMessage"] = "Không thể xóa phiếu nhập hàng.";
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
