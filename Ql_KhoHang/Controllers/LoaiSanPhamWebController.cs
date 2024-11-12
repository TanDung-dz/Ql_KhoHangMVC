using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Ql_KhoHang.Dtos;
using System.Security.Claims;
using System.Text;

namespace Ql_KhoHang.Controllers
{
	public class LoaiSanPhamWebController : Controller
	{
		private readonly IHttpClientFactory _httpClientFactory;
		private readonly string _apiBaseUrl;

		public LoaiSanPhamWebController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
		{
			_httpClientFactory = httpClientFactory;
			_apiBaseUrl = configuration["ApiSettings:BaseUrl"];
		}

		[HttpGet]
		public async Task<IActionResult> Index()
		{
			List<LoaiSanPhamWebDtos> categories = new List<LoaiSanPhamWebDtos>();

			try
			{
				var client = _httpClientFactory.CreateClient();
				var response = await client.GetAsync($"{_apiBaseUrl}/api/Menu/Get");

				if (response.IsSuccessStatusCode)
				{
					string data = await response.Content.ReadAsStringAsync();
					categories = JsonConvert.DeserializeObject<List<LoaiSanPhamWebDtos>>(data);
				}
				else
				{
					ModelState.AddModelError(string.Empty, "Failed to load categories from the API.");
				}
			}
			catch (Exception ex)
			{
				ModelState.AddModelError(string.Empty, $"An error occurred: {ex.Message}");
			}
			// Truy xuất thông tin người dùng từ Claims
			var manguoidung = User.Claims.FirstOrDefault(c => c.Type == "MaNguoiDung")?.Value;
			var name = User.Identity?.Name;
			var quyen = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
			ViewBag.Username = name;
			ViewBag.Role = quyen;

			return View(categories);
		}

		// Action lấy danh mục theo ID
		[HttpGet]
		public async Task<IActionResult> Details(int id)
		{
			LoaiSanPhamWebDtos category = null;

			var client = _httpClientFactory.CreateClient();
			var response = await client.GetAsync($"{_apiBaseUrl}/api/LoaiSanPham/GetById/{id}");

			if (response.IsSuccessStatusCode)
			{
				var data = await response.Content.ReadAsStringAsync();
				category = JsonConvert.DeserializeObject<LoaiSanPhamWebDtos>(data);
			}
			else
			{
				ModelState.AddModelError(string.Empty, "Failed to load category details.");
			}

			return View(category);
		}

		// Action tìm kiếm danh mục
		[HttpGet]
		public async Task<IActionResult> Search(string keyword)
		{
			List<LoaiSanPhamWebDtos> categories = new List<LoaiSanPhamWebDtos>();

			var client = _httpClientFactory.CreateClient();
			var response = await client.GetAsync($"{_apiBaseUrl}/api/LoaiSanPham/Search/{keyword}");

			if (response.IsSuccessStatusCode)
			{
				var data = await response.Content.ReadAsStringAsync();
				categories = JsonConvert.DeserializeObject<List<LoaiSanPhamWebDtos>>(data);
			}

			return View("Index", categories);
		}

		// Action lấy danh mục theo phân trang
		[HttpGet]
		public async Task<IActionResult> GetPaged(int pageNumber = 1, int pageSize = 5)
		{
			List<LoaiSanPhamWebDtos> categories = new List<LoaiSanPhamWebDtos>();

			var client = _httpClientFactory.CreateClient();
			var response = await client.GetAsync($"{_apiBaseUrl}/api/LoaiSanPham/paged?pageNumber={pageNumber}&pageSize={pageSize}");

			if (response.IsSuccessStatusCode)
			{
				var data = await response.Content.ReadAsStringAsync();
				categories = JsonConvert.DeserializeObject<List<LoaiSanPhamWebDtos>>(data);
			}

			return View("Index", categories);
		}

		// Action tạo mới danh mục
		[HttpPost]
		public async Task<IActionResult> Create(LoaiSanPhamWebDtos newCategory)
		{
			if (ModelState.IsValid)
			{
				var client = _httpClientFactory.CreateClient();
				var jsonContent = new StringContent(JsonConvert.SerializeObject(newCategory), Encoding.UTF8, "application/json");
				var response = await client.PostAsync($"{_apiBaseUrl}/api/LoaiSanPham", jsonContent);

				if (response.IsSuccessStatusCode)
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

		// Action cập nhật danh mục
		[HttpPost]
		public async Task<IActionResult> Edit(int id, LoaiSanPhamWebDtos category)
		{
			if (ModelState.IsValid)
			{
				var client = _httpClientFactory.CreateClient();
				var jsonContent = new StringContent(JsonConvert.SerializeObject(category), Encoding.UTF8, "application/json");
				var response = await client.PutAsync($"{_apiBaseUrl}/api/LoaiSanPham/{id}", jsonContent);

				if (response.IsSuccessStatusCode)
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

		// Action xóa danh mục (ẩn danh mục)
		[HttpPost]
		public async Task<IActionResult> Delete(int id)
		{
			var client = _httpClientFactory.CreateClient();
			var response = await client.DeleteAsync($"{_apiBaseUrl}/api/LoaiSanPham/{id}");

			if (response.IsSuccessStatusCode)
			{
				return RedirectToAction("Index");
			}
			else
			{
				ModelState.AddModelError(string.Empty, "Failed to delete category.");
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

	}

}
