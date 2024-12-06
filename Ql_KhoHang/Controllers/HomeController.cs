using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Ql_KhoHang.Dtos;
using Ql_KhoHang.Models;
using Ql_KhoHang.Services;
using System.Diagnostics;
using System.Net.Http;
using System.Text.Json;

namespace Ql_KhoHang.Controllers
{
	public class HomeController : Controller
	{
        private readonly MenuService _menuService;
        public HomeController(MenuService menuService)
        {
            _menuService = menuService;
        }
        public async Task<IActionResult> _SlidePartial()
		{
			return PartialView();
		}
        public async Task<IActionResult> _MenuPartial()
        {
            return PartialView();
        }
        public async Task<IActionResult> Index()
        {
            var menus = await _menuService.GetAllAsync();
            return View(menus);
        }
		public async Task<IActionResult> Contact()
		{
            var menus = await _menuService.GetAllAsync();
            return View(menus);
        }
        public async Task<IActionResult> Blog()
        {
            var menus = await _menuService.GetAllAsync();
            return View(menus);
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
