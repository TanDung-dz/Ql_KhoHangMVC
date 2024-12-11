using Newtonsoft.Json;
using Ql_KhoHang.Dtos;
using System.Text;

namespace Ql_KhoHang.Services
{
    public class ChiTietPhieuNhapHangService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _apiBaseUrl;

        public ChiTietPhieuNhapHangService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _apiBaseUrl = configuration["ApiSettings:BaseUrl"];
        }

        public async Task<List<ChiTietPhieuNhapHangDto>> GetByImportOrderIdAsync(int id)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"{_apiBaseUrl}/api/ChiTietPhieuNhapHang/GetById/{id}");

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var details = JsonConvert.DeserializeObject<List<ChiTietPhieuNhapHangDto>>(data);

                // Gắn URL đầy đủ cho ảnh
                foreach (var item in details)
                {
                    if (!string.IsNullOrEmpty(item.Image))
                    {
                        item.Image = $"{_apiBaseUrl}{item.Image}";
                        for (int i = 2; i <= 6; i++)
                        {
                            var imageProperty = typeof(ChiTietPhieuNhapHangDto).GetProperty($"Image{i}");
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
                return details.OrderByDescending(p=>p.SoLuong).ToList();
            }

            return new List<ChiTietPhieuNhapHangDto>();
        }
    }
}
