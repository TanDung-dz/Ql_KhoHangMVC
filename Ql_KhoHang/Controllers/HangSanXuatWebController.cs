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
            // Truy xuất thông tin người dùng từ Claims
            SetUserClaims();
            var manufacturers = await _hangSanXuatService.GetAllAsync();
            return View(manufacturers);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            // Truy xuất thông tin người dùng từ Claims
            SetUserClaims();
            var manufacturer = await _hangSanXuatService.GetByIdAsync(id);
            return View(manufacturer);
        }

        [HttpGet]
        public async Task<IActionResult> Search(string keyword)
        {
            // Truy xuất thông tin người dùng từ Claims
            SetUserClaims();
            var manufacturers = await _hangSanXuatService.SearchAsync(keyword);
            return View("Index", manufacturers);
        }

        [HttpGet]
        public IActionResult Create()
        {
            // Truy xuất thông tin người dùng từ Claims
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
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Failed to create manufacturer.");
                }
            }

            return View(newManufacturer);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            // Truy xuất thông tin người dùng từ Claims
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
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Failed to update manufacturer.");
                }
            }

            return View(manufacturer);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _hangSanXuatService.DeleteAsync(id);

            if (!success)
            {
                ModelState.AddModelError(string.Empty, "Failed to delete manufacturer.");
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
