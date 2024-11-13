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

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            List<SanPhamWebDtos> products = new List<SanPhamWebDtos>();

            try
            {
                var client = _httpClientFactory.CreateClient();
                var response = await client.GetAsync($"{_apiBaseUrl}/api/SanPham/Get");

                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    products = JsonConvert.DeserializeObject<List<SanPhamWebDtos>>(data);
					foreach (var product in products)
					{
						if (!string.IsNullOrEmpty(product.Image))
						{
							product.Image = $"{_apiBaseUrl}{product.Image}";
							Console.WriteLine(product.Image); // In ra URL ảnh để kiểm tra
						}
					}
				}
                else
                {
                    ModelState.AddModelError(string.Empty, "Failed to load products from the API.");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"An error occurred: {ex.Message}");
            }
			

			return View(products);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            SanPhamWebDtos product = null;

            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"{_apiBaseUrl}/api/SanPham/GetById/{id}");

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                product = JsonConvert.DeserializeObject<SanPhamWebDtos>(data);
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Failed to load product details.");
            }

            return View(product);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(SanPhamWebDtos newProduct, IFormFile Img)
        {
            if (ModelState.IsValid)
            {
                var client = _httpClientFactory.CreateClient();
                var requestContent = new MultipartFormDataContent();

                // Thêm các trường thông tin sản phẩm vào request
                requestContent.Add(new StringContent(newProduct.TenSanPham ?? ""), "TenSanPham");
                requestContent.Add(new StringContent(newProduct.Mota ?? ""), "Mota");
                requestContent.Add(new StringContent(newProduct.SoLuong.ToString() ?? "0"), "SoLuong");
                requestContent.Add(new StringContent(newProduct.DonGia.ToString() ?? "0"), "DonGia");
                requestContent.Add(new StringContent(newProduct.XuatXu ?? ""), "XuatXu");
                requestContent.Add(new StringContent(newProduct.MaLoaiSanPham.ToString()), "MaLoaiSanPham");
                requestContent.Add(new StringContent(newProduct.MaHangSanXuat.ToString()), "MaHangSanXuat");

                // Thêm file ảnh vào request nếu có
                if (Img != null && Img.Length > 0)
                {
                    var imageContent = new StreamContent(Img.OpenReadStream());
                    imageContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(Img.ContentType);
                    requestContent.Add(imageContent, "Img", Img.FileName);
                }

                var response = await client.PostAsync($"{_apiBaseUrl}/api/SanPham/uploadfile", requestContent);

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
            SanPhamWebDtos product = null;

            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"{_apiBaseUrl}/api/SanPham/GetById/{id}");

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                product = JsonConvert.DeserializeObject<SanPhamWebDtos>(data);
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Failed to load product details.");
            }

            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, SanPhamWebDtos product)
        {
            if (ModelState.IsValid)
            {
                var client = _httpClientFactory.CreateClient();
                var jsonContent = new StringContent(JsonConvert.SerializeObject(product), Encoding.UTF8, "application/json");
                var response = await client.PutAsync($"{_apiBaseUrl}/api/SanPham/UpdateProduct/{id}", jsonContent);

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
            var response = await client.DeleteAsync($"{_apiBaseUrl}/api/SanPham/DeleteProduct/{id}");

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
            List<SanPhamWebDtos> products = new List<SanPhamWebDtos>();

            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"{_apiBaseUrl}/api/SanPham/Search/{keyword}");

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                products = JsonConvert.DeserializeObject<List<SanPhamWebDtos>>(data);
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Failed to search products.");
            }

            return View("Index", products);
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
