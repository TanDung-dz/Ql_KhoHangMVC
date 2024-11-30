//using Newtonsoft.Json;
//using Ql_KhoHang.Dtos;
//using System.Text;

//namespace Ql_KhoHang.Services
//{
//    public class InventoryService
//    {
//        private readonly IHttpClientFactory _httpClientFactory;
//        private readonly string _apiBaseUrl;

//        public InventoryService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
//        {
//            _httpClientFactory = httpClientFactory;
//            _apiBaseUrl = configuration["ApiSettings:BaseUrl"];
//        }

//        public async Task<List<KiemKeDto>> GetAllInventoriesAsync()
//        {
//            var client = _httpClientFactory.CreateClient();
//            var response = await client.GetAsync($"{_apiBaseUrl}/api/KiemKe/Get");

//            if (response.IsSuccessStatusCode)
//            {
//                var data = await response.Content.ReadAsStringAsync();
//                return JsonConvert.DeserializeObject<List<KiemKeDto>>(data);
//            }

//            return new List<KiemKeDto>();
//        }

//        public async Task<KiemKeDto> GetInventoryByIdAsync(int id)
//        {
//            var client = _httpClientFactory.CreateClient();
//            var response = await client.GetAsync($"{_apiBaseUrl}/api/KiemKe/GetById/{id}");

//            if (response.IsSuccessStatusCode)
//            {
//                var data = await response.Content.ReadAsStringAsync();
//                return JsonConvert.DeserializeObject<KiemKeDto>(data);
//            }

//            return null;
//        }

//        public async Task<bool> CreateInventoryAsync(KiemKeDto newInventory, List<ChiTietKiemKeDto> details)
//        {
//            var client = _httpClientFactory.CreateClient();
//            var inventoryContent = new StringContent(JsonConvert.SerializeObject(newInventory), Encoding.UTF8, "application/json");

//            var response = await client.PostAsync($"{_apiBaseUrl}/api/KiemKe/Create", inventoryContent);

//            if (!response.IsSuccessStatusCode) return false;

//            foreach (var detail in details)
//            {
//                var detailContent = new MultipartFormDataContent();
//                detailContent.Add(new StringContent(detail.MaSanPham.ToString()), "MaSanPham");
//                detailContent.Add(new StringContent(detail.SoLuongTon.ToString()), "SoLuongTon");
//                detailContent.Add(new StringContent(detail.SoLuongThucTe.ToString()), "SoLuongThucTe");
//                detailContent.Add(new StringContent(detail.TrangThai.ToString() ?? ""), "TrangThai");
//                detailContent.Add(new StringContent(detail.NguyenNhan ?? ""), "NguyenNhan");

//                if (detail.Img != null && detail.Img.Length > 0)
//                {
//                    var imageContent = new StreamContent(detail..OpenReadStream());
//                    imageContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(detail.Img.ContentType);
//                    detailContent.Add(imageContent, "Img", detail.Img.FileName);
//                }

//                await client.PostAsync($"{_apiBaseUrl}/api/ChiTietKiemKe/Create", detailContent);
//            }

//            return true;
//        }


//        public async Task<bool> UpdateInventoryAsync(int id, KiemKeDto updatedInventory, List<ChiTietKiemKeDto> details)
//        {
//            var client = _httpClientFactory.CreateClient();
//            var inventoryContent = new StringContent(JsonConvert.SerializeObject(updatedInventory), Encoding.UTF8, "application/json");

//            var response = await client.PutAsync($"{_apiBaseUrl}/api/KiemKe/Update/{id}", inventoryContent);

//            if (!response.IsSuccessStatusCode) return false;

//            foreach (var detail in details)
//            {
//                var detailContent = new MultipartFormDataContent();
//                detailContent.Add(new StringContent(detail.MaSanPham.ToString()), "MaSanPham");
//                detailContent.Add(new StringContent(detail.SoLuongTon.ToString()), "SoLuongTon");
//                detailContent.Add(new StringContent(detail.SoLuongThucTe.ToString()), "SoLuongThucTe");
//                detailContent.Add(new StringContent(detail.TrangThai.ToString() ?? ""), "TrangThai");
//                detailContent.Add(new StringContent(detail.NguyenNhan ?? ""), "NguyenNhan");

//                if (detail.Img != null && detail.Img.Length > 0)
//                {
//                    var imageContent = new StreamContent(detail.Img.OpenReadStream());
//                    imageContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(detail.Img.ContentType);
//                    detailContent.Add(imageContent, "Img", detail.Img.FileName);
//                }

//                await client.PutAsync($"{_apiBaseUrl}/api/ChiTietKiemKe/Update/{detail.MaKiemKe}", detailContent);
//            }

//            return true;
//        }


//        public async Task<bool> DeleteInventoryAsync(int id)
//        {
//            var client = _httpClientFactory.CreateClient();
//            var response = await client.DeleteAsync($"{_apiBaseUrl}/api/KiemKe/Delete/{id}");

//            return response.IsSuccessStatusCode;
//        }

//        public async Task<List<KiemKeDto>> SearchInventoriesAsync(string keyword)
//        {
//            var client = _httpClientFactory.CreateClient();
//            var response = await client.GetAsync($"{_apiBaseUrl}/api/KiemKe/Search/{keyword}");

//            if (response.IsSuccessStatusCode)
//            {
//                var data = await response.Content.ReadAsStringAsync();
//                return JsonConvert.DeserializeObject<List<KiemKeDto>>(data);
//            }

//            return new List<KiemKeDto>();
//        }
//    }
//}
