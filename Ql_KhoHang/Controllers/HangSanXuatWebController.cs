using Microsoft.AspNetCore.Mvc;
using Ql_KhoHang.Dtos;
using Ql_KhoHang.Services;
using System.Security.Claims;

namespace Ql_KhoHang.Controllers
{
    public class HangSanXuatWebController : Controller
    {
        private readonly HangSanXuatService _hangSanXuatService;

        public HangSanXuatWebController(HangSanXuatService hangSanXuatService)
        {
            _hangSanXuatService = hangSanXuatService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            SetUserClaims();
            var manufacturers = await _hangSanXuatService.GetAllAsync();
            return View(manufacturers);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            SetUserClaims();
            var manufacturer = await _hangSanXuatService.GetByIdAsync(id);
            return View(manufacturer);
        }

        [HttpGet]
        public async Task<IActionResult> Search(string keyword)
        {
            SetUserClaims();
            var manufacturers = await _hangSanXuatService.SearchAsync(keyword);
            return View("Index", manufacturers);
        }

        [HttpGet]
        public IActionResult Create()
        {
            SetUserClaims();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(HangSanXuatWebDtos newManufacturer)
        {
            if (ModelState.IsValid)
            {
                var success = await _hangSanXuatService.CreateAsync(newManufacturer);

                if (success)
                {
                    TempData["SuccessMessage"] = "Tạo nhà sản xuất thành công!";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["ErrorMessage"] = "Không thể tạo nhà sản xuất.";
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Dữ liệu không hợp lệ.";
            }

            return View(newManufacturer);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            SetUserClaims();
            var manufacturer = await _hangSanXuatService.GetByIdAsync(id);
            return View(manufacturer);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, HangSanXuatWebDtos manufacturer)
        {
            if (ModelState.IsValid)
            {
                var success = await _hangSanXuatService.UpdateAsync(id, manufacturer);

                if (success)
                {
                    TempData["SuccessMessage"] = "Cập nhật nhà sản xuất thành công!";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["ErrorMessage"] = "Không thể cập nhật nhà sản xuất.";
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Dữ liệu không hợp lệ.";
            }

            return View(manufacturer);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _hangSanXuatService.DeleteAsync(id);

            if (success)
            {
                TempData["SuccessMessage"] = "Xóa nhà sản xuất thành công!";
            }
            else
            {
                TempData["ErrorMessage"] = "Không thể xóa nhà sản xuất.";
            }

            return RedirectToAction("Index");
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
