using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Ql_KhoHang.Dtos;
using Ql_KhoHang.Models;
using System.Diagnostics;
using System.Net.Http;
using System.Text.Json;

namespace Ql_KhoHang.Controllers
{
    //Test123
	public class HomeController : Controller
	{
        private readonly ILogger<HomeController> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _apiBaseUrl;


        public HomeController(ILogger<HomeController> logger, IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _apiBaseUrl = _apiBaseUrl = configuration["ApiSettings:BaseUrl"];
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
            List<MenuWebDtos> menus = new List<MenuWebDtos>();

            try
            {
                var client = _httpClientFactory.CreateClient();
                var response = await client.GetAsync($"{_apiBaseUrl}/api/menu/get");

                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    menus = JsonConvert.DeserializeObject<List<MenuWebDtos>>(data);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Failed to load menus from the API.");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"An error occurred: {ex.Message}");
            }           
            return View(menus);
        }


		public IActionResult Privacy()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
