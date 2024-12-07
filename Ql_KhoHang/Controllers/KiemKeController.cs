using Microsoft.AspNetCore.Mvc;
using Ql_KhoHang.Dtos;
using Ql_KhoHang.Services;
using System.Security.Claims;

namespace Ql_KhoHang.Controllers
{
    public class KiemKeController : Controller
    {
        private readonly KiemKeService _kiemKeService;
        private readonly ChiTietKiemKeService _chiTietKiemKeService;
        private readonly SanPhamService _sanPhamService;
        private readonly NhanVienKhoService _nhanVienKhoService;

        public KiemKeController(KiemKeService kiemKeService, ChiTietKiemKeService chiTietKiemKeService,
                                SanPhamService sanPhamService, NhanVienKhoService nhanVienKhoService)
        {
            _kiemKeService = kiemKeService;
            _chiTietKiemKeService = chiTietKiemKeService;
            _sanPhamService = sanPhamService;
            _nhanVienKhoService = nhanVienKhoService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? keyword, int pageNumber = 1, int pageSize = 10)
        {
            SetUserClaims();
            var kiemKes = await _kiemKeService.GetAllAsync(keyword);

            // Phân trang
            var paginatedKiemKes = kiemKes.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            int totalPages = (int)Math.Ceiling(kiemKes.Count / (double)pageSize);

            ViewBag.CurrentPage = pageNumber;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = totalPages;
            ViewBag.Keyword = keyword;

            return View(paginatedKiemKes);
        }
        //
        
        //
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            SetUserClaims();
            var kiemKe = await _kiemKeService.GetByIdAsync(id);
            if (kiemKe == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy phiếu kiểm kê.";
                return RedirectToAction("Index");
            }

            var details = await _chiTietKiemKeService.GetByInventoryCheckIdAsync(id);

            ViewBag.Details = details; // Danh sách chi tiết kiểm kê
            return View(kiemKe);
        }
        
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            SetUserClaims();

            // Sử dụng await để lấy dữ liệu thực tế từ các phương thức bất đồng bộ
            ViewBag.Products = await _sanPhamService.GetAllAsync();
            ViewBag.Employees = await _nhanVienKhoService.GetAllAsync("");

            return View(new KiemKeDto());
        }
        [HttpPost]
        public async Task<IActionResult> Create(KiemKeDto newInventoryCheck)
        {
            if (ModelState.IsValid)
            {
                // Lấy mã người dùng từ claim
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "MaNguoiDung")?.Value;

                if (!string.IsNullOrEmpty(userIdClaim) && int.TryParse(userIdClaim, out var userId))
                {
                     newInventoryCheck.NgayKiemKe = DateTime.Now; // Gắn ngày kiểm kê hiện tại
                }

                // Gọi service để tạo mới phiếu kiểm kê và chi tiết phiếu
                var success = await _kiemKeService.CreateAsync(newInventoryCheck);

                if (success)
                {
                    TempData["SuccessMessage"] = "Thêm mới phiếu kiểm kê thành công!";
                    return RedirectToAction("Index");
                }

                TempData["ErrorMessage"] = "Không thể thêm mới phiếu kiểm kê.";
            }

            // Nạp lại dữ liệu sản phẩm và nhân viên kho nếu có lỗi
            ViewBag.Products = await _sanPhamService.GetAllAsync();
            ViewBag.Employees = await _nhanVienKhoService.GetAllAsync("");

            return View(newInventoryCheck);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            SetUserClaims();

            ViewBag.Products = await _sanPhamService.GetAllAsync();
            ViewBag.Employees = await _nhanVienKhoService.GetAllAsync("");
            var kiemKe = await _kiemKeService.GetByIdAsync(id);
            if (kiemKe == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy phiếu kiểm kê.";
                return RedirectToAction("Index");
            }
            // lấy danh sách chi tiết
            var details = await _chiTietKiemKeService.GetByInventoryCheckIdAsync(id);
            // thêm danh sách vào dto phiểu kiểm kê
            kiemKe.Details = details;
            return View(kiemKe);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, KiemKeDto updatedInventoryCheck, List<ChiTietKiemKeDto> details)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Products = await _sanPhamService.GetAllAsync();
                ViewBag.Employees = await _nhanVienKhoService.GetAllAsync("");
                return View(updatedInventoryCheck);
            }
            var oldKiemke = await _kiemKeService.GetByIdAsync(updatedInventoryCheck.MaKiemKe);
            updatedInventoryCheck.NgayKiemKe = oldKiemke.NgayKiemKe;
            //Gán mã phiếu cho từng chi tiết
            foreach (var kvp in details)
            {
                kvp.MaKiemKe = updatedInventoryCheck.MaKiemKe;
            }
            // Lấy danh sách chi tiết hiện tại từ cơ sở dữ liệu
            var existingDetails = await _chiTietKiemKeService.GetByInventoryCheckIdAsync(id);

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
            var success = await _kiemKeService.UpdateAsync(id, updatedInventoryCheck, newDetails, updatedDetails, deletedDetails);
            if (success)
            {
                TempData["SuccessMessage"] = "Cập nhật phiếu kiểm kê thành công!";
                return RedirectToAction("Index");
            }

            // Xử lý khi có lỗi
            TempData["ErrorMessage"] = "Không thể cập nhật phiếu kiểm kê.";
            ViewBag.Products = await _sanPhamService.GetAllAsync();
            ViewBag.Employees = await _nhanVienKhoService.GetAllAsync("");
            return View(updatedInventoryCheck);
        }
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _kiemKeService.DeleteAsync(id);
            if (success)
            {
                TempData["SuccessMessage"] = "Xóa phiếu kiểm kê thành công!";
            }
            else
            {
                TempData["ErrorMessage"] = "Không thể xóa phiếu kiểm kê.";
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
