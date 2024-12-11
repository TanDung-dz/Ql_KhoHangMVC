using Newtonsoft.Json;
using Ql_KhoHang.Dtos;
using System.Text;

namespace Ql_KhoHang.Services
{
    public class ChiTietKiemKeService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _apiBaseUrl;

        public ChiTietKiemKeService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _apiBaseUrl = configuration["ApiSettings:BaseUrl"];
        }

        public async Task<List<ChiTietKiemKeDto>> GetByInventoryCheckIdAsync(int id)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"{_apiBaseUrl}/api/ChiTietKiemKe/GetById/{id}");

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var details = JsonConvert.DeserializeObject<List<ChiTietKiemKeDto>>(data);
                foreach (var item in details) {
                    if (!string.IsNullOrEmpty(item.Anh))
                    {
                        item.Anh = $"{_apiBaseUrl}{item.Anh}";
                        for (int i = 2; i <= 6; i++)
                        {
                            var imageProperty = typeof(ChiTietKiemKeDto).GetProperty($"Anh{i}");
                            if (imageProperty != null)
                            {
                                var imageValue = imageProperty.GetValue(item) as string;
                                if (!string.IsNullOrEmpty(imageValue))
                                {
                                    imageProperty.SetValue(item, $"{_apiBaseUrl}{imageValue}");
                                }
                            }
                        }
                    }
                }
                return details;
            }
            return new List<ChiTietKiemKeDto>();
        }
    }
}
