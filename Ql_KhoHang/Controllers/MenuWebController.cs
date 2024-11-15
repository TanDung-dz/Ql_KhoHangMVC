using Microsoft.AspNetCore.Mvc;
using Ql_KhoHang.Dtos;
using Ql_KhoHang.Services;
using System.Security.Claims;

namespace Ql_KhoHang.Controllers
{
    public class MenuWebController : Controller
    {
        private readonly MenuService _menuService;

        public MenuWebController(MenuService menuService)
        {
            _menuService = menuService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            SetUserClaims();
            var menus = await _menuService.GetAllAsync();
            return View(menus);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            SetUserClaims();
            var menu = await _menuService.GetByIdAsync(id);
            return View(menu);
        }

        [HttpGet]
        public IActionResult Create()
        {
            SetUserClaims();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(MenuWebDtos newMenu)
        {
            if (ModelState.IsValid)
            {
                var success = await _menuService.CreateAsync(newMenu);

                if (success)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Failed to create menu.");
                }
            }

            return View(newMenu);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            SetUserClaims();
            var menu = await _menuService.GetByIdAsync(id);
            return View(menu);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, MenuWebDtos menu)
        {
            if (ModelState.IsValid)
            {
                var success = await _menuService.UpdateAsync(id, menu);

                if (success)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Failed to update menu.");
                }
            }

            return View(menu);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _menuService.DeleteAsync(id);

            if (!success)
            {
                ModelState.AddModelError(string.Empty, "Failed to delete menu.");
            }

            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<IActionResult> Search(string keyword)
        {
            SetUserClaims();
            var menus = await _menuService.SearchAsync(keyword);
            return View("Index", menus);
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
