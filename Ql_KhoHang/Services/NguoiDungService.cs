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
    }
}
