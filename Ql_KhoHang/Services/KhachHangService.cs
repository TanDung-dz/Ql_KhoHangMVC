using Newtonsoft.Json;
using Ql_KhoHang.Dtos;
using System.Text;

namespace Ql_KhoHang.Services
{
    public class KhachHangService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _apiBaseUrl;

        public KhachHangService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _apiBaseUrl = configuration["ApiSettings:BaseUrl"];
        }

        public async Task<List<KhachHangDto>> GetAllAsync()
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"{_apiBaseUrl}/api/khachhang/Get");

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var customers = JsonConvert.DeserializeObject<List<KhachHangDto>>(data);
                return customers;
            }

            return new List<KhachHangDto>();
        }

        public async Task<KhachHangDto> GetByIdAsync(int id)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"{_apiBaseUrl}/api/khachhang/GetById/{id}");

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var customer = JsonConvert.DeserializeObject<KhachHangDto>(data);
                return customer;
            }

            return null;
        }

        public async Task<bool> CreateAsync(KhachHangDto newCustomer)
        {
            var client = _httpClientFactory.CreateClient();
            var requestContent = new StringContent(
                JsonConvert.SerializeObject(newCustomer),
                Encoding.UTF8,
                "application/json");

            var response = await client.PostAsync($"{_apiBaseUrl}/api/khachhang/CreateCustomer", requestContent);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateAsync(int id, KhachHangDto updatedCustomer)
        {
            var client = _httpClientFactory.CreateClient();
            var requestContent = new StringContent(
                JsonConvert.SerializeObject(updatedCustomer),
                Encoding.UTF8,
                "application/json");

            var response = await client.PutAsync($"{_apiBaseUrl}/api/khachhang/UpdateCustomer/{id}", requestContent);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.DeleteAsync($"{_apiBaseUrl}/api/khachhang/DeleteCustomer/{id}");

            return response.IsSuccessStatusCode;
        }

        public async Task<List<KhachHangDto>> SearchAsync(string keyword)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"{_apiBaseUrl}/api/khachhang/Search/{keyword}");

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var customers = JsonConvert.DeserializeObject<List<KhachHangDto>>(data);
                return customers;
            }

            return new List<KhachHangDto>();
        }
    }
}
