using Microsoft.AspNetCore.Mvc;
using Ql_KhoHang.Dtos;
using Ql_KhoHang.Services;
using System.Security.Claims;

namespace Ql_KhoHang.Controllers
{
    public class SanPhamWebController : Controller
    {
        private readonly SanPhamService _sanPhamService;
        private readonly LoaiSanPhamService _loaiSanPhamService;
        private readonly HangSanXuatService _hangSanXuatService;

        public SanPhamWebController(SanPhamService sanPhamService, LoaiSanPhamService loaiSanPhamService, HangSanXuatService hangSanXuatService)
        {
            _sanPhamService = sanPhamService;
            _loaiSanPhamService = loaiSanPhamService;
            _hangSanXuatService = hangSanXuatService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            SetUserClaims();
            var products = await _sanPhamService.GetAllAsync();
            return View(products);
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
            return View(new SanPhamWebDtos());
        }

        [HttpPost]
        public async Task<IActionResult> Create(SanPhamWebDtos newProduct, IFormFile Img)
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
        public async Task<IActionResult> Edit(int id, SanPhamWebDtos product, IFormFile Img)
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

            if (!success)
            {
                ModelState.AddModelError(string.Empty, "Failed to delete product.");
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Search(string keyword)
        {
            var products = await _sanPhamService.SearchAsync(keyword);
            return View("Index", products);
        }
        private void SetUserClaims()
        {
            ViewBag.Username = User.Identity?.Name;
            ViewBag.Role = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
        }
    }
}
