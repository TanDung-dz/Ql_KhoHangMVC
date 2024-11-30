using Microsoft.AspNetCore.Mvc;
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

        public SanPhamController(SanPhamService sanPhamService, LoaiSanPhamService loaiSanPhamService, HangSanXuatService hangSanXuatService)
        {
            _sanPhamService = sanPhamService;
            _loaiSanPhamService = loaiSanPhamService;
            _hangSanXuatService = hangSanXuatService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 4, string keyword = "")
        {
            SetUserClaims();

            List<SanPhamDto> allProducts;

            if (!string.IsNullOrEmpty(keyword))
            {
                // Nếu có keyword, thực hiện tìm kiếm
                allProducts = await _sanPhamService.SearchAsync(keyword);
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
            ViewBag.CurrentPage = pageNumber;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = totalPages;
            ViewBag.Keyword = keyword; // Gửi keyword hiện tại tới View để giữ nguyên giá trị tìm kiếm

            return View(paginatedProducts);
        }




        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            SetUserClaims();
            var product = await _sanPhamService.GetByIdAsync(id);
            return View(product);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            SetUserClaims();
            // Lấy danh sách loại sản phẩm và hãng sản xuất
            var loaiSanPhams = await _loaiSanPhamService.GetAllAsync();
            var hangSanXuats = await _hangSanXuatService.GetAllAsync();

            // Gửi dữ liệu đến View thông qua ViewBag
            ViewBag.LoaiSanPhams = loaiSanPhams;
            ViewBag.HangSanXuats = hangSanXuats;
            return View(new SanPhamDto());
        }

        [HttpPost]
        public async Task<IActionResult> Create(SanPhamDto newProduct, IFormFile Img)
        {
            if (ModelState.IsValid)
            {
                // Gọi service để lưu sản phẩm mới
                var success = await _sanPhamService.CreateAsync(newProduct, Img);

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

            // Gửi dữ liệu đến View thông qua ViewBag
            ViewBag.LoaiSanPhams = loaiSanPhams;
            ViewBag.HangSanXuats = hangSanXuats;
            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, SanPhamDto product, IFormFile Img)
        {
            if (ModelState.IsValid)
            {
                var success = await _sanPhamService.UpdateAsync(id, product, Img);

                if (success)
                {
                    TempData["SuccessMessage"] = "Sửa sản phẩm thành công!";
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Failed to update product.");
                }
            }
            // Nếu có lỗi, tải lại danh sách Loại Sản Phẩm và Hãng Sản Xuất
            ViewBag.LoaiSanPhams = await _loaiSanPhamService.GetAllAsync();
            ViewBag.HangSanXuats = await _hangSanXuatService.GetAllAsync();
            return View(product);
        }


        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _sanPhamService.DeleteAsync(id);
            if (success)
            {
                TempData["SuccessMessage"] = "Sửa sản phẩm thành công!";
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
