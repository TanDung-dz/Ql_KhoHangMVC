using Newtonsoft.Json;
using Ql_KhoHang.Dtos;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

        public async Task<NguoiDungDto> LoginAsync(string username, string password)
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
                var nguoidung = JsonConvert.DeserializeObject<NguoiDungDto>(data);
				return nguoidung;
            }

            return null;
        }
        // Lấy tất cả người dùng
        public async Task<List<NguoiDungDto>> GetAllAsync()
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"{_apiBaseUrl}/api/NguoiDung/Get");
            

			if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var nguoidungs = JsonConvert.DeserializeObject<List<NguoiDungDto>>(data);
				return nguoidungs;
            }

            return new List<NguoiDungDto>();
        }

        // Lấy người dùng theo ID
        public async Task<NguoiDungDto> GetByIdAsync(int id)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"{_apiBaseUrl}/api/NguoiDung/GetById/{id}");

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
				var nguoidung = JsonConvert.DeserializeObject<NguoiDungDto>(data);
				return nguoidung;
            }

            return null;
        }

        // Tìm kiếm người dùng theo từ khóa
        public async Task<List<NguoiDungDto>> SearchAsync(string keyword)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"{_apiBaseUrl}/api/NguoiDung/Search/{keyword}");

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var nguoidungs = JsonConvert.DeserializeObject<List<NguoiDungDto>>(data);
                foreach (var item in nguoidungs)
                {
                    if (!string.IsNullOrEmpty(item.Anh))
                    {
                        item.Anh = $"{_apiBaseUrl}{item.Anh}";
                    }

                }
                return nguoidungs;
            }

            return new List<NguoiDungDto>();
        }

		// Tạo người dùng mới
		public async Task<bool> CreateAsync(NguoiDungDto newUser, IFormFile Img)
		{
			var client = _httpClientFactory.CreateClient();
			var requestContent = new MultipartFormDataContent
			{
				{ new StringContent(newUser.TenDangNhap), "TenDangNhap" },
				{ new StringContent(newUser.MatKhau), "MatKhau" },
				{ new StringContent(newUser.TenNguoiDung), "TenNguoiDung" },
				{ new StringContent(newUser.Email ?? ""), "Email" },
				{ new StringContent(newUser.Sdt ?? ""), "Sdt" },
				{ new StringContent(newUser.Quyen.ToString() ?? ""), "Quyen" }
			};

			if (Img != null && Img.Length > 0)
			{
				var imageContent = new StreamContent(Img.OpenReadStream());
				imageContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(Img.ContentType);
				requestContent.Add(imageContent, "Img", Img.FileName);
			}

			var response = await client.PostAsync($"{_apiBaseUrl}/api/NguoiDung/CreateUser", requestContent);

			return response.IsSuccessStatusCode;
		}

		public async Task<bool> UpdateAsync(int id, NguoiDungDto updatedUser, IFormFile Img)
		{
			var client = _httpClientFactory.CreateClient();
			var requestContent = new MultipartFormDataContent
			{
				{ new StringContent(updatedUser.TenDangNhap), "TenDangNhap" },
				{ new StringContent(updatedUser.TenNguoiDung), "TenNguoiDung" },
				{ new StringContent(updatedUser.Email ?? ""), "Email" },
				{ new StringContent(updatedUser.Sdt ?? ""), "Sdt" },
				{ new StringContent(updatedUser.Quyen.ToString() ?? ""), "Quyen" }
			};

			if (!string.IsNullOrEmpty(updatedUser.MatKhau))
			{
				requestContent.Add(new StringContent(updatedUser.MatKhau), "MatKhau");
			}

			if (Img != null && Img.Length > 0)
			{
				var imageContent = new StreamContent(Img.OpenReadStream());
				imageContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(Img.ContentType);
				requestContent.Add(imageContent, "Img", Img.FileName);
			}

			var response = await client.PutAsync($"{_apiBaseUrl}/api/NguoiDung/UpdateUser/{id}", requestContent);

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
