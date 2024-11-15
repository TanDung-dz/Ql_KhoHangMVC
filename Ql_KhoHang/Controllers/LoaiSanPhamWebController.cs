﻿using Microsoft.AspNetCore.Mvc;
using Ql_KhoHang.Dtos;
using Ql_KhoHang.Services;
using System.Security.Claims;

namespace Ql_KhoHang.Controllers
{
    public class LoaiSanPhamWebController : Controller
    {
        private readonly LoaiSanPhamService _loaiSanPhamService;

        public LoaiSanPhamWebController(LoaiSanPhamService loaiSanPhamService)
        {
            _loaiSanPhamService = loaiSanPhamService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            SetUserClaims();
            var categories = await _loaiSanPhamService.GetAllAsync();
            return View(categories);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            SetUserClaims();
            var category = await _loaiSanPhamService.GetByIdAsync(id);
            return View(category);
        }

        [HttpGet]
        public async Task<IActionResult> Search(string keyword)
        {
            SetUserClaims();
            var categories = await _loaiSanPhamService.SearchAsync(keyword);
            return View("Index", categories);
        }
        [HttpGet]
        public IActionResult Create()
        {
            SetUserClaims();
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(LoaiSanPhamWebDtos newCategory)
        {
            if (ModelState.IsValid)
            {
                var success = await _loaiSanPhamService.CreateAsync(newCategory);

                if (success)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Failed to create category.");
                }
            }

            return View(newCategory);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            SetUserClaims();
            var category = await _loaiSanPhamService.GetByIdAsync(id);
            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, LoaiSanPhamWebDtos category)
        {
            if (ModelState.IsValid)
            {
                var success = await _loaiSanPhamService.UpdateAsync(id, category);

                if (success)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Failed to update category.");
                }
            }

            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _loaiSanPhamService.DeleteAsync(id);

            if (!success)
            {
                ModelState.AddModelError(string.Empty, "Failed to delete category.");
            }

            return RedirectToAction("Index");
        }
        private void SetUserClaims()
        {
            ViewBag.Username = User.Identity?.Name;
            ViewBag.Role = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
        }
    }
}
