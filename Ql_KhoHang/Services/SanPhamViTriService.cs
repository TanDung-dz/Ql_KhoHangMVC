using Newtonsoft.Json;
using Ql_KhoHang.Dtos;
using System.Text;

namespace Ql_KhoHang.Services
{
    public class SanPhamViTriService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _apiBaseUrl;

        public SanPhamViTriService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _apiBaseUrl = configuration["ApiSettings:BaseUrl"];
        }

        // Lấy tất cả các sản phẩm tại các vị trí
        public async Task<List<SanPhamViTriDto>> GetAllAsync()
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"{_apiBaseUrl}/api/sanphamvitri/Get");

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var productLocations = JsonConvert.DeserializeObject<List<SanPhamViTriDto>>(data);
                return productLocations;
            }

            return new List<SanPhamViTriDto>();
        }

        // Lấy thông tin chi tiết sản phẩm tại một vị trí
        public async Task<List<SanPhamViTriDto>> GetByViTriAsync(int maViTri)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"{_apiBaseUrl}/api/sanphamvitri/GetByViTri/{maViTri}");

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var productLocations = JsonConvert.DeserializeObject<List<SanPhamViTriDto>>(data);
                return productLocations;
            }

            return new List<SanPhamViTriDto>();
        }
        // Service method to get a list of products by product ID
        public async Task<List<SanPhamViTriDto>> GetBySanPhamAsync(int maSanPham)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"{_apiBaseUrl}/api/sanphamvitri/GetBySanPham/{maSanPham}");

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var sanPhams = JsonConvert.DeserializeObject<List<SanPhamViTriDto>>(data);
                return sanPhams;
            }

            // Return an empty list if the API response is not successful
            return new List<SanPhamViTriDto>();
        }

        // Tạo mới thông tin sản phẩm tại vị trí
        public async Task<bool> CreateAsync(SanPhamViTriDto newProductLocation)
        {
            var client = _httpClientFactory.CreateClient();
            var requestContent = new StringContent(
                JsonConvert.SerializeObject(newProductLocation),
                Encoding.UTF8,
                "application/json");

            var response = await client.PostAsync($"{_apiBaseUrl}/api/sanphamvitri/Create", requestContent);

            return response.IsSuccessStatusCode;
        }

        // Cập nhật thông tin sản phẩm tại vị trí
        public async Task<bool> UpdateAsync(SanPhamViTriDto updatedProductLocation)
        {
            var client = _httpClientFactory.CreateClient();
            var requestContent = new StringContent(
                JsonConvert.SerializeObject(updatedProductLocation),
                Encoding.UTF8,
                "application/json");

            var response = await client.PutAsync($"{_apiBaseUrl}/api/sanphamvitri/Update", requestContent);

            return response.IsSuccessStatusCode;
        }

        // Xóa thông tin sản phẩm tại vị trí 
        public async Task<bool> DeleteAsync(int maViTri, int maSanPham)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.DeleteAsync($"{_apiBaseUrl}/api/sanphamvitri/Delete/{maViTri}/{maSanPham}");

            return response.IsSuccessStatusCode;
        }

    }
}
