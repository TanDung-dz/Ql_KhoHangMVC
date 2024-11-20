using Newtonsoft.Json;
using Ql_KhoHang.Dtos;
using System.Text;

namespace Ql_KhoHang.Services
{
    public class NguoiDungService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _apiBaseUrl;

        public NguoiDungService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _apiBaseUrl = configuration["ApiSettings:BaseUrl"];
        }

        public async Task<NguoiDungWebDtos> LoginAsync(string username, string password)
        {
            var client = _httpClientFactory.CreateClient();
            var loginUrl = $"{_apiBaseUrl}/api/NguoiDung/Login/login?username=" + username + "&password=" + password;

            var loginData = new
            {
                username = username,
                password = password
            };

            var jsonContent = new StringContent(JsonConvert.SerializeObject(loginData), Encoding.UTF8, "application/json");
            var response = await client.PostAsync(loginUrl, jsonContent);

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<NguoiDungWebDtos>(data);
            }

            return null;
        }
        // Lấy tất cả người dùng
        public async Task<List<NguoiDungWebDtos>> GetAllAsync()
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"{_apiBaseUrl}/api/NguoiDung/Get");

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<NguoiDungWebDtos>>(data);
            }

            return new List<NguoiDungWebDtos>();
        }

        // Lấy người dùng theo ID
        public async Task<NguoiDungWebDtos> GetByIdAsync(int id)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"{_apiBaseUrl}/api/NguoiDung/GetById/{id}");

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<NguoiDungWebDtos>(data);
            }

            return null;
        }

        // Tìm kiếm người dùng theo từ khóa
        public async Task<List<NguoiDungWebDtos>> SearchAsync(string keyword)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"{_apiBaseUrl}/api/NguoiDung/Search/{keyword}");

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<NguoiDungWebDtos>>(data);
            }

            return new List<NguoiDungWebDtos>();
        }

        // Tạo người dùng mới
        public async Task<bool> CreateAsync(NguoiDungWebDtos newUser)
        {
            var client = _httpClientFactory.CreateClient();
            var jsonContent = new StringContent(JsonConvert.SerializeObject(newUser), Encoding.UTF8, "application/json");
            var response = await client.PostAsync($"{_apiBaseUrl}/api/NguoiDung/CreateUser", jsonContent);

            return response.IsSuccessStatusCode;
        }

        // Cập nhật người dùng
        public async Task<bool> UpdateAsync(int id, NguoiDungWebDtos updatedUser)
        {
            var client = _httpClientFactory.CreateClient();
            var jsonContent = new StringContent(JsonConvert.SerializeObject(updatedUser), Encoding.UTF8, "application/json");
            var response = await client.PutAsync($"{_apiBaseUrl}/api/NguoiDung/UpdateUser/{id}", jsonContent);

            return response.IsSuccessStatusCode;
        }

        // Xóa người dùng
        public async Task<bool> DeleteAsync(int id)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.DeleteAsync($"{_apiBaseUrl}/api/NguoiDung/DeleteUser/{id}");

            return response.IsSuccessStatusCode;
        }
    }
}
