using Microsoft.AspNetCore.Mvc;
using Ql_KhoHang.Dtos;
using Ql_KhoHang.Services;
using System.Security.Claims;

namespace Ql_KhoHang.Controllers
{
    public class LoaiKhachHangWebController : Controller
    {
        private readonly LoaiKhachHangService _loaiKhachHangService;

        public LoaiKhachHangWebController(LoaiKhachHangService loaiKhachHangService)
        {
            _loaiKhachHangService = loaiKhachHangService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            SetUserClaims();
            var loaiKhachHangs = await _loaiKhachHangService.GetAllAsync();
            return View(loaiKhachHangs);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            SetUserClaims();
            var loaiKhachHang = await _loaiKhachHangService.GetByIdAsync(id);
            return View(loaiKhachHang);
        }

        [HttpGet]
        public IActionResult Create()
        {
            SetUserClaims();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(LoaiKhachHangWebDtos newLoaiKhachHang)
        {
            if (ModelState.IsValid)
            {
                var success = await _loaiKhachHangService.CreateAsync(newLoaiKhachHang);

                if (success)
                {
                    TempData["SuccessMessage"] = "Tạo loại khách hàng thành công!";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["ErrorMessage"] = "Không thể tạo loại khách hàng.";
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Dữ liệu không hợp lệ.";
            }

            return View(newLoaiKhachHang);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            SetUserClaims();
            var loaiKhachHang = await _loaiKhachHangService.GetByIdAsync(id);
            return View(loaiKhachHang);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, LoaiKhachHangWebDtos loaiKhachHang)
        {
            if (ModelState.IsValid)
            {
                var success = await _loaiKhachHangService.UpdateAsync(id, loaiKhachHang);

                if (success)
                {
                    TempData["SuccessMessage"] = "Cập nhật loại khách hàng thành công!";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["ErrorMessage"] = "Không thể cập nhật loại khách hàng.";
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Dữ liệu không hợp lệ.";
            }

            return View(loaiKhachHang);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _loaiKhachHangService.DeleteAsync(id);

            if (success)
            {
                TempData["SuccessMessage"] = "Xóa loại khách hàng thành công!";
            }
            else
            {
                TempData["ErrorMessage"] = "Không thể xóa loại khách hàng.";
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Search(string keyword)
        {
            SetUserClaims();
            var result = await _loaiKhachHangService.SearchAsync(keyword);
            if (result == null || !result.Any())
            {
                TempData["ErrorMessage"] = "Không tìm thấy loại khách hàng.";
            }
            else
            {
                TempData["SuccessMessage"] = $"Tìm thấy {result.Count()} kết quả.";
            }
            return View("Search", result);
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
