using Newtonsoft.Json;
using Ql_KhoHang.Dtos;
using System.Text;

namespace Ql_KhoHang.Services
{
	public class KiemKeService
	{
		private readonly IHttpClientFactory _httpClientFactory;
		private readonly string _apiBaseUrl;

		public KiemKeService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
		{
			_httpClientFactory = httpClientFactory;
			_apiBaseUrl = configuration["ApiSettings:BaseUrl"];
		}

		// Gọi API từ KiemKeController
		public async Task<List<KiemKeDto>> GetKiemKeAsync()
		{
			var client = _httpClientFactory.CreateClient();
			var response = await client.GetAsync($"{_apiBaseUrl}/api/KiemKe/Get");
			if (response.IsSuccessStatusCode)
			{
				var data = await response.Content.ReadAsStringAsync();
				return JsonConvert.DeserializeObject<List<KiemKeDto>>(data);
			}
			return new List<KiemKeDto>();
		}

		public async Task<KiemKeDto> GetKiemKeByIdAsync(int id)
		{
			var client = _httpClientFactory.CreateClient();
			var response = await client.GetAsync($"{_apiBaseUrl}/api/KiemKe/GetById/{id}");
			if (response.IsSuccessStatusCode)
			{
				var data = await response.Content.ReadAsStringAsync();
				return JsonConvert.DeserializeObject<KiemKeDto>(data);
			}
			return null;
		}

		// Gọi API từ ChiTietKiemKeController
		public async Task<List<ChiTietKiemKeDto>> GetChiTietKiemKeAsync(int kiemKeId)
		{
			var client = _httpClientFactory.CreateClient();
			var response = await client.GetAsync($"{_apiBaseUrl}/api/ChiTietKiemKe/GetById/{kiemKeId}");
			if (response.IsSuccessStatusCode)
			{
				var data = await response.Content.ReadAsStringAsync();
				return JsonConvert.DeserializeObject<List<ChiTietKiemKeDto>>(data);
			}
			return new List<ChiTietKiemKeDto>();
		}
		public async Task<bool> CreateKiemKeAsync(KiemKeDto newKiemKe)
		{
			var client = _httpClientFactory.CreateClient();
			var jsonContent = new StringContent(JsonConvert.SerializeObject(newKiemKe), Encoding.UTF8, "application/json");
			var response = await client.PostAsync($"{_apiBaseUrl}/api/KiemKe/CreateInventoryCheck", jsonContent);
			return response.IsSuccessStatusCode;
		}

		public async Task<bool> UpdateKiemKeAsync(int id, KiemKeDto updatedKiemKe)
		{
			var client = _httpClientFactory.CreateClient();
			var jsonContent = new StringContent(JsonConvert.SerializeObject(updatedKiemKe), Encoding.UTF8, "application/json");
			var response = await client.PutAsync($"{_apiBaseUrl}/api/KiemKe/UpdateInventoryCheck/{id}", jsonContent);
			return response.IsSuccessStatusCode;
		}

		public async Task<bool> DeleteKiemKeAsync(int id)
		{
			var client = _httpClientFactory.CreateClient();
			var response = await client.DeleteAsync($"{_apiBaseUrl}/api/KiemKe/DeleteInventoryCheck/{id}");
			return response.IsSuccessStatusCode;
		}

		public async Task<bool> CreateChiTietKiemKeAsync(ChiTietKiemKeDto newChiTiet)
		{
			var client = _httpClientFactory.CreateClient();
			var jsonContent = new StringContent(JsonConvert.SerializeObject(newChiTiet), Encoding.UTF8, "application/json");
			var response = await client.PostAsync($"{_apiBaseUrl}/api/ChiTietKiemKe/CreateDetail", jsonContent);
			return response.IsSuccessStatusCode;
		}

		public async Task<bool> UpdateChiTietKiemKeAsync(int kiemKeId, ChiTietKiemKeDto updatedChiTiet)
		{
			var client = _httpClientFactory.CreateClient();
			var jsonContent = new StringContent(JsonConvert.SerializeObject(updatedChiTiet), Encoding.UTF8, "application/json");
			var response = await client.PutAsync($"{_apiBaseUrl}/api/ChiTietKiemKe/UpdateDetail/{kiemKeId}", jsonContent);
			return response.IsSuccessStatusCode;
		}

		public async Task<bool> DeleteChiTietKiemKeAsync(int kiemKeId, int sanPhamId)
		{
			var client = _httpClientFactory.CreateClient();
			var response = await client.DeleteAsync($"{_apiBaseUrl}/api/ChiTietKiemKe/DeleteDetail/{sanPhamId}");
			return response.IsSuccessStatusCode;
		}

	}

}
