using Newtonsoft.Json;
using Ql_KhoHang.Dtos;
using System.Net.Http.Headers;
using System.Text;

namespace Ql_KhoHang.Services
{
	public class NhanVienKhoService
	{
		private readonly IHttpClientFactory _httpClientFactory;
		private readonly string _apiBaseUrl;

		public NhanVienKhoService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
		{
			_httpClientFactory = httpClientFactory;
			_apiBaseUrl = configuration["ApiSettings:BaseUrl"];
		}

		// Lấy tất cả nhân viên kho hoặc tìm kiếm nếu có từ khóa
		public async Task<List<NhanVienKhoDto>> GetAllAsync(string? keyword)
		{
			var client = _httpClientFactory.CreateClient();
			string url = string.IsNullOrEmpty(keyword)
				? $"{_apiBaseUrl}/api/NhanVienKho/Get"  // Lấy tất cả nhân viên kho
				: $"{_apiBaseUrl}/api/NhanVienKho/Search/{keyword}";  // Tìm kiếm nhân viên kho theo từ khóa

			var response = await client.GetAsync(url);

			if (response.IsSuccessStatusCode)
			{
				var data = await response.Content.ReadAsStringAsync();
				var employees = JsonConvert.DeserializeObject<List<NhanVienKhoDto>>(data);

				foreach (var item in employees)
				{
					if (!string.IsNullOrEmpty(item.Hinhanh))
					{
						item.Hinhanh = $"{_apiBaseUrl}{item.Hinhanh}";  // Gắn URL đầy đủ cho ảnh
					}
				}

				return employees;
			}

			return new List<NhanVienKhoDto>();
		}

		// Lấy thông tin nhân viên kho theo ID
		public async Task<NhanVienKhoDto> GetByIdAsync(int id)
		{
			var client = _httpClientFactory.CreateClient();
			var response = await client.GetAsync($"{_apiBaseUrl}/api/NhanVienKho/GetById/{id}");

			if (response.IsSuccessStatusCode)
			{
				var data = await response.Content.ReadAsStringAsync();
				var employee = JsonConvert.DeserializeObject<NhanVienKhoDto>(data);

				if (!string.IsNullOrEmpty(employee.Hinhanh))
				{
					employee.Hinhanh = $"{_apiBaseUrl}{employee.Hinhanh}";  // Gắn URL đầy đủ cho ảnh
				}

				return employee;
			}

			return null;
		}

		// Thêm mới nhân viên kho
		public async Task<bool> CreateAsync(NhanVienKhoDto newEmployee, IFormFile Img)
		{
			var client = _httpClientFactory.CreateClient();
			var requestContent = new MultipartFormDataContent();

			// Thêm thông tin nhân viên kho
			requestContent.Add(new StringContent(newEmployee.TenNhanVien ?? ""), "TenNhanVien");
			requestContent.Add(new StringContent(newEmployee.Email ?? ""), "Email");
			requestContent.Add(new StringContent(newEmployee.Sdt.ToString() ?? "0"), "Sdt");
			requestContent.Add(new StringContent(newEmployee.NamSinh?.ToString("yyyy-MM-dd") ?? ""), "NamSinh");

			// Thêm ảnh nếu có
			if (Img != null && Img.Length > 0)
			{
				var imageContent = new StreamContent(Img.OpenReadStream());
				imageContent.Headers.ContentType = new MediaTypeHeaderValue(Img.ContentType);
				requestContent.Add(imageContent, "Img", Img.FileName);
			}

			var response = await client.PostAsync($"{_apiBaseUrl}/api/NhanVienKho/Create/uploadfile", requestContent);

			return response.IsSuccessStatusCode;
		}

		// Cập nhật nhân viên kho
		public async Task<bool> UpdateAsync(int id, NhanVienKhoDto updatedEmployee, IFormFile Img)
		{
			var client = _httpClientFactory.CreateClient();
			var requestContent = new MultipartFormDataContent();

			// Thêm thông tin nhân viên kho
			requestContent.Add(new StringContent(updatedEmployee.TenNhanVien ?? ""), "TenNhanVien");
			requestContent.Add(new StringContent(updatedEmployee.Email ?? ""), "Email");
			requestContent.Add(new StringContent(updatedEmployee.Sdt.ToString() ?? "0"), "Sdt");
			requestContent.Add(new StringContent(updatedEmployee.NamSinh?.ToString("yyyy-MM-dd") ?? ""), "NamSinh");

			// Thêm ảnh nếu có
			if (Img != null && Img.Length > 0)
			{
				var imageContent = new StreamContent(Img.OpenReadStream());
				imageContent.Headers.ContentType = new MediaTypeHeaderValue(Img.ContentType);
				requestContent.Add(imageContent, "Img", Img.FileName);
			}

			var response = await client.PutAsync($"{_apiBaseUrl}/api/NhanVienKho/Update/{id}", requestContent);

			return response.IsSuccessStatusCode;
		}

		// Xóa nhân viên kho
		public async Task<bool> DeleteAsync(int id)
		{
			var client = _httpClientFactory.CreateClient();
			var response = await client.DeleteAsync($"{_apiBaseUrl}/api/NhanVienKho/Delete/{id}");

			return response.IsSuccessStatusCode;
		}
	}
}
