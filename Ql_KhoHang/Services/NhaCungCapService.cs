using Newtonsoft.Json;
using Ql_KhoHang.Dtos;
using System.Text;

namespace Ql_KhoHang.Services
{
    public class NhaCungCapService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _apiBaseUrl;

        public NhaCungCapService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _apiBaseUrl = configuration["ApiSettings:BaseUrl"];
        }

        public async Task<List<NhacungcapDto>> GetAllAsync()
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"{_apiBaseUrl}/api/NhaCungCap/Get");

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var nhacc = JsonConvert.DeserializeObject<List<NhacungcapDto>>(data);
				return nhacc;

            }

            return new List<NhacungcapDto>();
        }

        public async Task<NhacungcapDto> GetByIdAsync(int id)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"{_apiBaseUrl}/api/NhaCungCap/GetById/{id}");

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var ncc = JsonConvert.DeserializeObject<NhacungcapDto>(data);
				return ncc;
            }

            return null;
        }

        public async Task<bool> CreateAsync(NhacungcapDto newSupplier, IFormFile Img)
        {
            var client = _httpClientFactory.CreateClient();
            var requestContent = new MultipartFormDataContent();

            // Thêm thông tin nhà cung cấp
            requestContent.Add(new StringContent(newSupplier.TenNhaCungCap ?? ""), "TenNhaCungCap");
            requestContent.Add(new StringContent(newSupplier.DiaChi ?? ""), "DiaChi");
            requestContent.Add(new StringContent(newSupplier.Email ?? ""), "Email");
            requestContent.Add(new StringContent(newSupplier.Sdt.ToString() ?? "0"), "Sdt");

            // Thêm file ảnh nếu có
            if (Img != null && Img.Length > 0)
            {
                var imageContent = new StreamContent(Img.OpenReadStream());
                imageContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(Img.ContentType);
                requestContent.Add(imageContent, "Img", Img.FileName);
            }

            var response = await client.PostAsync($"{_apiBaseUrl}/api/NhaCungCap/CreateSupplierWithImage/uploadfile", requestContent);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateAsync(int id, NhacungcapDto updatedSupplier, IFormFile Img)
        {
            var client = _httpClientFactory.CreateClient();
            var requestContent = new MultipartFormDataContent();

            // Thêm thông tin nhà cung cấp
            requestContent.Add(new StringContent(updatedSupplier.TenNhaCungCap ?? ""), "TenNhaCungCap");
            requestContent.Add(new StringContent(updatedSupplier.DiaChi ?? ""), "DiaChi");
            requestContent.Add(new StringContent(updatedSupplier.Email ?? ""), "Email");
            requestContent.Add(new StringContent(updatedSupplier.Sdt.ToString() ?? "0"), "Sdt");

            // Thêm file ảnh nếu có
            if (Img != null && Img.Length > 0)
            {
                var imageContent = new StreamContent(Img.OpenReadStream());
                imageContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(Img.ContentType);
                requestContent.Add(imageContent, "Img", Img.FileName);
            }

            var response = await client.PutAsync($"{_apiBaseUrl}/api/NhaCungCap/UpdateSupplier/{id}", requestContent);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.DeleteAsync($"{_apiBaseUrl}/api/NhaCungCap/DeleteSupplier/{id}");

            return response.IsSuccessStatusCode;
        }

        public async Task<List<NhacungcapDto>> SearchAsync(string keyword)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"{_apiBaseUrl}/api/NhaCungCap/Search/{keyword}");

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var nhacc = JsonConvert.DeserializeObject<List<NhacungcapDto>>(data);
                return nhacc;
            }

            return new List<NhacungcapDto>();
        }
    }
}
