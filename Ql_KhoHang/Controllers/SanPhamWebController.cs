using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Ql_KhoHang.Dtos;
using System.Text;

namespace Ql_KhoHang.Controllers
{
    public class SanPhamWebController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _apiBaseUrl;

        public SanPhamWebController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _apiBaseUrl = configuration["ApiSettings:BaseUrl"];
        }

        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"{_apiBaseUrl}/api/Menu/Get");

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var sanPhams = JsonConvert.DeserializeObject<IEnumerable<SanPhamWebDtos>>(data);
                return View(sanPhams);
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Failed to load products from the API.");
                return View(new List<SanPhamWebDtos>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View(new SanPhamWebDtos());
        }

        [HttpPost]
        public async Task<IActionResult> Create(SanPhamWebDtos newProduct)
        {
            if (ModelState.IsValid)
            {
                var client = _httpClientFactory.CreateClient();
                var jsonContent = new StringContent(JsonConvert.SerializeObject(newProduct), Encoding.UTF8, "application/json");
                var response = await client.PostAsync($"{_apiBaseUrl}/api/SanPham", jsonContent);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Failed to create product.");
                }
            }

            return View(newProduct);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"{_apiBaseUrl}/api/SanPham/{id}");

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var product = JsonConvert.DeserializeObject<SanPhamWebDtos>(data);
                return View(product);
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Failed to load product details.");
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, SanPhamWebDtos product)
        {
            if (ModelState.IsValid)
            {
                var client = _httpClientFactory.CreateClient();
                var jsonContent = new StringContent(JsonConvert.SerializeObject(product), Encoding.UTF8, "application/json");
                var response = await client.PutAsync($"{_apiBaseUrl}/api/SanPham/{id}", jsonContent);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Failed to update product.");
                }
            }

            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.DeleteAsync($"{_apiBaseUrl}/api/SanPham/{id}");

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Failed to delete product.");
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Search(string keyword)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"{_apiBaseUrl}/api/SanPham/search?keyword={keyword}");

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var products = JsonConvert.DeserializeObject<IEnumerable<SanPhamWebDtos>>(data);
                return View("Index", products);
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Failed to search products.");
                return View("Index", new List<SanPhamWebDtos>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"{_apiBaseUrl}/api/SanPham/getbyid/{id}");

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var sanPham = JsonConvert.DeserializeObject<SanPhamWebDtos>(data);
                return View(sanPham);
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Failed to load product details.");
                return RedirectToAction("Index");
            }
        }

        public async Task<IActionResult> _MenuPartial()
        {
            return PartialView();
        }
        public async Task<IActionResult> _SidebarPartial()
        {
            return PartialView();
        }
    }
}