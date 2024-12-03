using Newtonsoft.Json;
using Ql_KhoHang.Dtos;
using System.Text;

namespace Ql_KhoHang.Services
{
    public class ChiTietPhieuXuatHangService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _apiBaseUrl;

        public ChiTietPhieuXuatHangService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _apiBaseUrl = configuration["ApiSettings:BaseUrl"];
        }

        public async Task<List<ChiTietPhieuXuatHangDto>> GetByExportOrderIdAsync(int id)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"{_apiBaseUrl}/api/ChiTietPhieuXuatHang/GetById/{id}");

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var details = JsonConvert.DeserializeObject<List<ChiTietPhieuXuatHangDto>>(data);

                // Gắn URL đầy đủ cho ảnh
                foreach (var item in details)
                {
                    if (!string.IsNullOrEmpty(item.Image))
                    {
                        item.Image = $"{_apiBaseUrl}{item.Image}";
                    }
                }

                return details;
            }

            return new List<ChiTietPhieuXuatHangDto>();
        }

        public async Task<bool> CreateDetailAsync(ChiTietPhieuXuatHangDto detail, IFormFile Img)
        {
            var client = _httpClientFactory.CreateClient();
            var requestContent = new MultipartFormDataContent();

            // Thêm các trường dữ liệu
            requestContent.Add(new StringContent(detail.MaPhieuXuatHang.ToString()), "MaPhieuXuatHang");
            requestContent.Add(new StringContent(detail.MaSanPham.ToString()), "MaSanPham");
            requestContent.Add(new StringContent(detail.SoLuong.ToString()), "SoLuong");
            requestContent.Add(new StringContent(detail.DonGiaXuat.ToString()), "DonGiaXuat");
            requestContent.Add(new StringContent(detail.TrangThai.ToString() ?? ""), "TrangThai");

            // Thêm file ảnh
            if (Img != null && Img.Length > 0)
            {
                var imageContent = new StreamContent(Img.OpenReadStream());
                imageContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(Img.ContentType);
                requestContent.Add(imageContent, "Img", Img.FileName);
            }

            var response = await client.PostAsync($"{_apiBaseUrl}/api/ChiTietPhieuXuatHang/CreateDetailWithImage/uploadfile", requestContent);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateDetailAsync(ChiTietPhieuXuatHangDto detail, IFormFile Img)
        {
            var client = _httpClientFactory.CreateClient();
            var requestContent = new MultipartFormDataContent();

            // Thêm các trường dữ liệu
            requestContent.Add(new StringContent(detail.MaPhieuXuatHang.ToString()), "MaPhieuXuatHang");
            requestContent.Add(new StringContent(detail.MaSanPham.ToString()), "MaSanPham");
            requestContent.Add(new StringContent(detail.SoLuong.ToString()), "SoLuong");
            requestContent.Add(new StringContent(detail.DonGiaXuat.ToString()), "DonGiaXuat");
            requestContent.Add(new StringContent(detail.TrangThai.ToString() ?? ""), "TrangThai");

            // Thêm file ảnh
            if (Img != null && Img.Length > 0)
            {
                var imageContent = new StreamContent(Img.OpenReadStream());
                imageContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(Img.ContentType);
                requestContent.Add(imageContent, "Img", Img.FileName);
            }

            var response = await client.PutAsync($"{_apiBaseUrl}/api/ChiTietPhieuXuatHang/UpdateDetail/{detail.MaPhieuXuatHang}/{detail.MaSanPham}", requestContent);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteDetailAsync(int maPhieuXuatHang, int maSanPham)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.DeleteAsync($"{_apiBaseUrl}/api/ChiTietPhieuXuatHang/DeleteDetail/{maPhieuXuatHang}/{maSanPham}");

            return response.IsSuccessStatusCode;
        }
    }
}
