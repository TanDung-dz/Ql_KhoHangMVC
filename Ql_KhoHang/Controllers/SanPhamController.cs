using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Ql_KhoHang.Dtos;
using Ql_KhoHang.Services;
using System.Security.Claims;

namespace Ql_KhoHang.Controllers
{
    public class SanPhamController : Controller
    {
        private readonly SanPhamService _sanPhamService;
        private readonly LoaiSanPhamService _loaiSanPhamService;
        private readonly HangSanXuatService _hangSanXuatService;
        private readonly SanPhamViTriService _sanPhamViTriService;
        private readonly ViTriService _ViTriService;
        private readonly NhaCungCapService _nhaCungCapService;
        private readonly ChiTietPhieuNhapHangService _chiTietPhieuNhapHangService;
        private readonly PhieuNhapHangService _phieuNhapHangService;
        public SanPhamController(
            SanPhamService sanPhamService,
            LoaiSanPhamService loaiSanPhamService,
            HangSanXuatService hangSanXuatService,
            SanPhamViTriService sanPhamViTriService,
            ViTriService viTriService,
            NhaCungCapService nhaCungCapService,
            ChiTietPhieuNhapHangService chiTietPhieuNhapHangService,
            PhieuNhapHangService phieuNhapHangService
            ) // Thêm dịch vụ
        {
            _sanPhamService = sanPhamService;
            _loaiSanPhamService = loaiSanPhamService;
            _hangSanXuatService = hangSanXuatService;
            _sanPhamViTriService = sanPhamViTriService; // Khởi tạo dịch vụ
            _ViTriService = viTriService;
            _nhaCungCapService = nhaCungCapService;
            _chiTietPhieuNhapHangService = chiTietPhieuNhapHangService;
            _phieuNhapHangService = phieuNhapHangService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 9, string keyword = "", string loaiSanpham = "tatca")
        {
            SetUserClaims();

            List<SanPhamDto> allProducts;
            ViewBag.CurrentLoaiSanPham = loaiSanpham; // Truyền ID loại hiện tại tới View
            if (!string.IsNullOrEmpty(keyword))
            {
                // Nếu có keyword, thực hiện tìm kiếm
                allProducts = await _sanPhamService.SearchAsync(keyword);
            }
            else if (loaiSanpham != "tatca")
            {
                // Lọc sản phẩm theo loại sản phẩm
                allProducts = await _sanPhamService.GetByLoaiSanPhamAsync(loaiSanpham);
            }
            else
            {
                // Nếu không có keyword, lấy toàn bộ sản phẩm
                allProducts = await _sanPhamService.GetAllAsync();
            }

            // Tính toán dữ liệu phân trang
            var paginatedProducts = allProducts
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Tính tổng số trang
            int totalPages = (int)Math.Ceiling(allProducts.Count / (double)pageSize);

            // Gửi thông tin phân trang tới View
            ViewBag.LoaiSanPhams = await _loaiSanPhamService.GetAllAsync();
            
            ViewBag.CurrentPage = pageNumber;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = totalPages;
            ViewBag.Keyword = keyword; // Gửi keyword hiện tại tới View để giữ nguyên giá trị tìm kiếm

            return View(paginatedProducts);
        }
        public async Task<IActionResult> GetImportBatchesByProductId(int productId)
        {
            // Lấy danh sách phiếu nhập liên quan
            var phieuNhapLienQuan = await _chiTietPhieuNhapHangService.GetByImportOrderProductIdAsync(productId);

            // Truyền dữ liệu qua ViewBag
            ViewBag.PhieuNhapLienQuan = phieuNhapLienQuan;

            // Trả về PartialView
            return PartialView("_ImportBatches");
        }
        public async Task<IActionResult> _ImportBatches()
        {
            return PartialView();
        }
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            SetUserClaims();
            var product = await _sanPhamService.GetByIdAsync(id);
            var vitris = await _sanPhamViTriService.GetBySanPhamAsync(product.MaSanPham);
            ViewBag.ViTris = vitris;
            return View(product);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            SetUserClaims();
            // Lấy danh sách loại sản phẩm và hãng sản xuất
            var loaiSanPhams = await _loaiSanPhamService.GetAllAsync();
            var hangSanXuats = await _hangSanXuatService.GetAllAsync();
            var nhacc = await _nhaCungCapService.GetAllAsync();
            var vitris = await _ViTriService.GetAllAsync();

            // Gửi dữ liệu đến View thông qua ViewBag
            ViewBag.LoaiSanPhams = loaiSanPhams;
            ViewBag.HangSanXuats = hangSanXuats;
            ViewBag.NhaCungCaps = nhacc;
            ViewBag.Vitris = vitris;
            return View(new SanPhamDto());
        }

        [HttpPost]
        public async Task<IActionResult> Create(SanPhamDto newProduct, IFormFileCollection Images)
        {
            newProduct.TrangThai = true;
            if (ModelState.IsValid)
            {
                // Gọi service để lưu sản phẩm mới
                var success = await _sanPhamService.CreateAsync(newProduct, Images);
                if (success)
                {
                    TempData["SuccessMessage"] = "Thêm sản phẩm thành công!";
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Failed to create product.");
                }
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine(error.ErrorMessage);
                }
            }

            // Nếu có lỗi, tải lại danh sách Loại Sản Phẩm và Hãng Sản Xuất
            ViewBag.LoaiSanPhams = await _loaiSanPhamService.GetAllAsync();
            ViewBag.HangSanXuats = await _hangSanXuatService.GetAllAsync();
            ViewBag.NhaCungCaps = await _nhaCungCapService.GetAllAsync();
            return View(newProduct);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            SetUserClaims();
            var product = await _sanPhamService.GetByIdAsync(id);
            // Lấy danh sách loại sản phẩm và hãng sản xuất
            var loaiSanPhams = await _loaiSanPhamService.GetAllAsync();
            var hangSanXuats = await _hangSanXuatService.GetAllAsync();
            var nhacc = await _nhaCungCapService.GetAllAsync();
            var vitris = await _ViTriService.GetAllAsync();
            // lấy danh sách chi tiết
            var details = await _sanPhamViTriService.GetBySanPhamAsync(id);
            // thêm danh sách vào dto 
            product.ViTriSanPhams = details;
            // Gửi dữ liệu đến View thông qua ViewBag
            ViewBag.LoaiSanPhams = loaiSanPhams;
            ViewBag.HangSanXuats = hangSanXuats;
            ViewBag.NhaCungCaps = nhacc;
            ViewBag.Vitris = vitris;
            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, SanPhamDto product, IFormFileCollection Images, List<SanPhamViTriDto> vitrisanphams)
        {
            product.TrangThai = true;
            if (!ModelState.IsValid)
            {
                ViewBag.LoaiSanPhams = await _loaiSanPhamService.GetAllAsync();
                ViewBag.HangSanXuats = await _hangSanXuatService.GetAllAsync();
                ViewBag.NhaCungCaps = await _nhaCungCapService.GetAllAsync();
                ViewBag.Vitris = await _ViTriService.GetAllAsync();
                return View(product);
            }
            //Gán mã sp cho từng chi tiết
            foreach (var kvp in vitrisanphams)
            {
                kvp.MaSanPham = product.MaSanPham;
            }

            // Lấy danh sách vị trí hiện tại từ cơ sở dữ liệu
            var existingDetails = await _sanPhamViTriService.GetBySanPhamAsync(id);
            // Tìm các chi tiết cần thêm mới
            var newDetails = vitrisanphams
                .Where(d => existingDetails.All(ed => ed.MaViTri != d.MaViTri))
                .ToList();

            // Tìm các chi tiết cần cập nhật
            var updatedDetails = vitrisanphams
                .Where(d => existingDetails.Any(ed => ed.MaViTri == d.MaViTri))
                .ToList();

            // Tìm các chi tiết cần xóa
            var deletedDetails = existingDetails
                .Where(ed => vitrisanphams.All(d => d.MaViTri != ed.MaViTri))
                .ToList();


            var success = await _sanPhamService.UpdateAsync(id, product, Images, newDetails, updatedDetails, deletedDetails);

            if (success)
            {
                TempData["SuccessMessage"] = "Sửa sản phẩm thành công!";
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Failed to update product.");
            }
            // Nếu có lỗi, tải lại danh sách Loại Sản Phẩm và Hãng Sản Xuất
            TempData["ErrorMessage"] = "Không thể cập nhật sản phẩm.";
            ViewBag.LoaiSanPhams = await _loaiSanPhamService.GetAllAsync();
            ViewBag.HangSanXuats = await _hangSanXuatService.GetAllAsync();
            ViewBag.NhaCungCaps = await _nhaCungCapService.GetAllAsync();
            ViewBag.Vitris = await _ViTriService.GetAllAsync();
            return View(product);
        }


        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _sanPhamService.DeleteAsync(id);
            if (success)
            {
                TempData["SuccessMessage"] = "Xóa sản phẩm thành công!";
                return RedirectToAction("Index");
            }
            else
                ModelState.AddModelError(string.Empty, "Failed to delete product.");
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
