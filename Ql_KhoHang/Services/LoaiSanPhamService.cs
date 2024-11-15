using Newtonsoft.Json;
using Ql_KhoHang.Dtos;

namespace Ql_KhoHang.Services
{
    using Newtonsoft.Json;
    using Ql_KhoHang.Dtos;
    using System.Text;

    public class LoaiSanPhamService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _apiBaseUrl;

        public LoaiSanPhamService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _apiBaseUrl = configuration["ApiSettings:BaseUrl"];
        }

        public async Task<List<LoaiSanPhamWebDtos>> GetAllAsync()
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"{_apiBaseUrl}/api/LoaiSanPham/Get");

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<LoaiSanPhamWebDtos>>(data);
            }

            return new List<LoaiSanPhamWebDtos>();
        }

        public async Task<LoaiSanPhamWebDtos> GetByIdAsync(int id)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"{_apiBaseUrl}/api/LoaiSanPham/GetById/{id}");

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<LoaiSanPhamWebDtos>(data);
            }

            return null;
        }

        public async Task<List<LoaiSanPhamWebDtos>> SearchAsync(string keyword)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"{_apiBaseUrl}/api/LoaiSanPham/Search/{keyword}");

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<LoaiSanPhamWebDtos>>(data);
            }

            return new List<LoaiSanPhamWebDtos>();
        }

        public async Task<bool> CreateAsync(LoaiSanPhamWebDtos newCategory)
        {
            var client = _httpClientFactory.CreateClient();
            var jsonContent = new StringContent(JsonConvert.SerializeObject(newCategory), Encoding.UTF8, "application/json");
            var response = await client.PostAsync($"{_apiBaseUrl}/api/LoaiSanPham/CreateProductType", jsonContent);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateAsync(int id, LoaiSanPhamWebDtos category)
        {
            var client = _httpClientFactory.CreateClient();
            var jsonContent = new StringContent(JsonConvert.SerializeObject(category), Encoding.UTF8, "application/json");
            var response = await client.PutAsync($"{_apiBaseUrl}/api/LoaiSanPham/UpdateProductType/{id}", jsonContent);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.DeleteAsync($"{_apiBaseUrl}/api/LoaiSanPham/DeleteProductType/{id}");

            return response.IsSuccessStatusCode;
        }
    }


}
