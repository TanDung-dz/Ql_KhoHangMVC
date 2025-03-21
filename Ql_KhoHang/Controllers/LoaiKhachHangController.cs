﻿using Microsoft.AspNetCore.Mvc;
using Ql_KhoHang.Dtos;
using Ql_KhoHang.Services;
using System.Security.Claims;

namespace Ql_KhoHang.Controllers
{
    public class LoaiKhachHangController : Controller
    {
        private readonly LoaiKhachHangService _loaiKhachHangService;

        public LoaiKhachHangController(LoaiKhachHangService loaiKhachHangService)
        {
            _loaiKhachHangService = loaiKhachHangService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? keyword, int pageNumber = 1, int pageSize = 10)
        {
            SetUserClaims();
            IEnumerable<LoaiKhachHangDto> allLoaiKhachHang;

            if (!string.IsNullOrEmpty(keyword))
            {
                allLoaiKhachHang = await _loaiKhachHangService.SearchAsync(keyword);
                if (!allLoaiKhachHang.Any())
                {
                    TempData["ErrorMessage"] = "Không tìm thấy loại khách hàng nào.";
                }
                else
                {
                    TempData["SuccessMessage"] = $"Tìm thấy {allLoaiKhachHang.Count()} kết quả.";
                }
            }
            else
            {
                allLoaiKhachHang = await _loaiKhachHangService.GetAllAsync();
            }

            // Tính toán dữ liệu phân trang
            var paginatedLoaiKhachHang = allLoaiKhachHang
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Tính tổng số trang
            int totalPages = (int)Math.Ceiling(allLoaiKhachHang.Count() / (double)pageSize);

            // Gửi thông tin phân trang tới View
            ViewBag.CurrentPage = pageNumber;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = totalPages;
            ViewBag.Keyword = keyword; // Để giữ từ khóa tìm kiếm

            return View(paginatedLoaiKhachHang);
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
        public async Task<IActionResult> Create(LoaiKhachHangDto newLoaiKhachHang)
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
        public async Task<IActionResult> Edit(int id, LoaiKhachHangDto loaiKhachHang)
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
            ViewBag.HinhAnh = User.Claims.FirstOrDefault(c => c.Type == "HinhAnh")?.Value;
        }
    }
}
