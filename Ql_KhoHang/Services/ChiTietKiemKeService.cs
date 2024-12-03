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

                // Gắn URL đầy đủ cho ảnh
                foreach (var item in details)
                {
                    if (!string.IsNullOrEmpty(item.Anh))
                    {
                        item.Anh = $"{_apiBaseUrl}{item.Anh}";
                    }
                }

                return details;
            }

            return new List<ChiTietKiemKeDto>();
        }

        public async Task<bool> CreateDetailAsync(ChiTietKiemKeDto detail, IFormFile Img)
        {
            var client = _httpClientFactory.CreateClient();
            var requestContent = new MultipartFormDataContent();

            // Thêm các trường dữ liệu
            requestContent.Add(new StringContent(detail.MaKiemKe.ToString()), "MaKiemKe");
            requestContent.Add(new StringContent(detail.MaSanPham.ToString()), "MaSanPham");
            requestContent.Add(new StringContent(detail.SoLuongTon.ToString() ?? ""), "SoLuongTon");
            requestContent.Add(new StringContent(detail.SoLuongThucTe.ToString() ?? ""), "SoLuongThucTe");
            requestContent.Add(new StringContent(detail.TrangThai.ToString() ?? ""), "TrangThai");
            requestContent.Add(new StringContent(detail.NguyenNhan ?? ""), "NguyenNhan");

            // Thêm file ảnh
            if (Img != null && Img.Length > 0)
            {
                var imageContent = new StreamContent(Img.OpenReadStream());
                imageContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(Img.ContentType);
                requestContent.Add(imageContent, "Img", Img.FileName);
            }

            var response = await client.PostAsync($"{_apiBaseUrl}/api/ChiTietKiemKe/CreateDetail", requestContent);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateDetailAsync(ChiTietKiemKeDto detail, IFormFile Img)
        {
            var client = _httpClientFactory.CreateClient();
            var requestContent = new MultipartFormDataContent();

            // Thêm các trường dữ liệu
            requestContent.Add(new StringContent(detail.MaKiemKe.ToString()), "MaKiemKe");
            requestContent.Add(new StringContent(detail.MaSanPham.ToString()), "MaSanPham");
            requestContent.Add(new StringContent(detail.SoLuongTon.ToString() ?? ""), "SoLuongTon");
            requestContent.Add(new StringContent(detail.SoLuongThucTe.ToString() ?? ""), "SoLuongThucTe");
            requestContent.Add(new StringContent(detail.TrangThai.ToString() ?? ""), "TrangThai");
            requestContent.Add(new StringContent(detail.NguyenNhan ?? ""), "NguyenNhan");

            // Thêm file ảnh
            if (Img != null && Img.Length > 0)
            {
                var imageContent = new StreamContent(Img.OpenReadStream());
                imageContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(Img.ContentType);
                requestContent.Add(imageContent, "Img", Img.FileName);
            }

            var response = await client.PutAsync($"{_apiBaseUrl}/api/ChiTietKiemKe/UpdateDetail/{detail.MaKiemKe}/{detail.MaSanPham}", requestContent);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteDetailAsync(int id, int maSanPham)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.DeleteAsync($"{_apiBaseUrl}/api/ChiTietKiemKe/DeleteDetail/{id}/{maSanPham}");

            return response.IsSuccessStatusCode;
        }
    }
}
