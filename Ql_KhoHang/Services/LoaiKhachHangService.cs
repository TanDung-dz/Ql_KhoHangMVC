using Newtonsoft.Json;
using Ql_KhoHang.Dtos;
using System.Text;

namespace Ql_KhoHang.Services
{
	public class LoaiKhachHangService
	{
		private readonly IHttpClientFactory _httpClientFactory;
		private readonly string _apiBaseUrl;

		public LoaiKhachHangService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
		{
			_httpClientFactory = httpClientFactory;
			_apiBaseUrl = configuration["ApiSettings:BaseUrl"];
		}

		public async Task<List<LoaiKhachHangWebDtos>> GetAllAsync()
		{
			var client = _httpClientFactory.CreateClient();
			var response = await client.GetAsync($"{_apiBaseUrl}/api/LoaiKhachHang/Get");

			if (response.IsSuccessStatusCode)
			{
				var data = await response.Content.ReadAsStringAsync();
				return JsonConvert.DeserializeObject<List<LoaiKhachHangWebDtos>>(data);
			}

			return new List<LoaiKhachHangWebDtos>();
		}

		public async Task<LoaiKhachHangWebDtos> GetByIdAsync(int id)
		{
			var client = _httpClientFactory.CreateClient();
			var response = await client.GetAsync($"{_apiBaseUrl}/api/LoaiKhachHang/GetById/{id}");

			if (response.IsSuccessStatusCode)
			{
				var data = await response.Content.ReadAsStringAsync();
				return JsonConvert.DeserializeObject<LoaiKhachHangWebDtos>(data);
			}

			return null;
		}

		public async Task<List<LoaiKhachHangWebDtos>> SearchAsync(string keyword)
		{
			var client = _httpClientFactory.CreateClient();
			var response = await client.GetAsync($"{_apiBaseUrl}/api/LoaiKhachHang/Search/{keyword}");

			if (response.IsSuccessStatusCode)
			{
				var data = await response.Content.ReadAsStringAsync();
				return JsonConvert.DeserializeObject<List<LoaiKhachHangWebDtos>>(data);
			}

			return new List<LoaiKhachHangWebDtos>();
		}

		public async Task<bool> CreateAsync(LoaiKhachHangWebDtos newCustomerType)
		{
			var client = _httpClientFactory.CreateClient();
			var jsonContent = new StringContent(JsonConvert.SerializeObject(newCustomerType), Encoding.UTF8, "application/json");
			var response = await client.PostAsync($"{_apiBaseUrl}/api/LoaiKhachHang/CreateCustomerType", jsonContent);

			return response.IsSuccessStatusCode;
		}

		public async Task<bool> UpdateAsync(int id, LoaiKhachHangWebDtos customerType)
		{
			var client = _httpClientFactory.CreateClient();
			var jsonContent = new StringContent(JsonConvert.SerializeObject(customerType), Encoding.UTF8, "application/json");
			var response = await client.PutAsync($"{_apiBaseUrl}/api/LoaiKhachHang/UpdateCustomerType/{id}", jsonContent);

			return response.IsSuccessStatusCode;
		}

		public async Task<bool> DeleteAsync(int id)
		{
			var client = _httpClientFactory.CreateClient();
			var response = await client.DeleteAsync($"{_apiBaseUrl}/api/LoaiKhachHang/DeleteCustomerType/{id}");

			return response.IsSuccessStatusCode;
		}
	}
}
