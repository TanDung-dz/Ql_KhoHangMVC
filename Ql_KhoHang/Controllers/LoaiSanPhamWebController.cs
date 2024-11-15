using Microsoft.AspNetCore.Mvc;
using Ql_KhoHang.Dtos;
using Ql_KhoHang.Services;

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
            var categories = await _loaiSanPhamService.GetAllAsync();
            return View(categories);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var category = await _loaiSanPhamService.GetByIdAsync(id);
            return View(category);
        }

        [HttpGet]
        public async Task<IActionResult> Search(string keyword)
        {
            var categories = await _loaiSanPhamService.SearchAsync(keyword);
            return View("Index", categories);
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
    }
}
