using Newtonsoft.Json;
using Ql_KhoHang.Dtos;
using System.Text;

namespace Ql_KhoHang.Services
{
    public class KiemKeService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _apiBaseUrl;

        public KiemKeService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _apiBaseUrl = configuration["ApiSettings:BaseUrl"];
        }

        public async Task<List<KiemKeDto>> GetAllAsync(string? keyword)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"{_apiBaseUrl}/api/KiemKe/Get?keyword={keyword}");

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var kiemke = JsonConvert.DeserializeObject<List<KiemKeDto>>(data);
                return kiemke.OrderByDescending(p=>p.NgayKiemKe).ToList();
            }

            return new List<KiemKeDto>();
        }

        public async Task<KiemKeDto> GetByIdAsync(int id)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"{_apiBaseUrl}/api/KiemKe/GetById/{id}");

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<KiemKeDto>(data);
            }

            return null;
        }

        public async Task<bool> CreateAsync(KiemKeDto newInventoryCheck)
        {
            var client = _httpClientFactory.CreateClient();

            // Gửi phiếu kiểm kê đến API
            var inventoryCheckContent = new StringContent(JsonConvert.SerializeObject(newInventoryCheck), Encoding.UTF8, "application/json");
            var response = await client.PostAsync($"{_apiBaseUrl}/api/KiemKe/CreateInventoryCheck", inventoryCheckContent);

            if (!response.IsSuccessStatusCode)
            {
                return false; // Dừng lại nếu lưu phiếu kiểm kê thất bại
            }

            // Lấy lại mã phiếu kiểm kê từ API
            var responseData = await response.Content.ReadAsStringAsync();
            var createdInventoryCheck = JsonConvert.DeserializeObject<KiemKeDto>(responseData);
            var createdInventoryCheckId = createdInventoryCheck.MaKiemKe;

            // Gửi dữ liệu chi tiết phiếu kiểm kê
            if (newInventoryCheck.Details != null && newInventoryCheck.Details.Any())
            {
                foreach (var detail in newInventoryCheck.Details)
                {
                    // Gắn mã phiếu kiểm kê cho từng chi tiết
                    detail.MaKiemKe = createdInventoryCheckId;

                    // Tạo nội dung multipart cho từng chi tiết
                    var detailContent = new MultipartFormDataContent();
                    AddKiemkeDataToRequest(detail, detailContent);
                    AddImagesToRequest(detail.Images, detailContent);

                    // Gửi chi tiết phiếu kiểm kê đến API
                    var detailResponse = await client.PostAsync($"{_apiBaseUrl}/api/ChiTietKiemKe/CreateDetail", detailContent);

                    if (!detailResponse.IsSuccessStatusCode)
                    {
                        return false; // Dừng lại nếu lưu chi tiết thất bại
                    }
                }
            }

            return true;
        }

        public async Task<bool> UpdateAsync(
            int id,
            KiemKeDto updatedKiemke,
            List<ChiTietKiemKeDto> newDetails,
            List<ChiTietKiemKeDto> updatedDetails,
            List<ChiTietKiemKeDto> deletedDetails)
        {
            var client = _httpClientFactory.CreateClient();

            // Cập nhật thông tin 
            var kiemkeContent = new StringContent(JsonConvert.SerializeObject(updatedKiemke), Encoding.UTF8, "application/json");
            var response = await client.PutAsync($"{_apiBaseUrl}/api/KiemKe/UpdateInventoryCheck/{id}", kiemkeContent);
            if (!response.IsSuccessStatusCode)
            {
                return false; // Dừng lại nếu cập nhật  thất bại
            }

            // Thêm mới các chi tiết 
            foreach (var detail in newDetails)
            {
                detail.MaKiemKe = id; // Gắn mã cho từng chi tiết
                var success = await CreateDetailAsync(detail, $"{_apiBaseUrl}/api/ChiTietKiemKe/CreateDetail");
                if (!success)
                {
                    return false; // Dừng lại nếu thêm mới một chi tiết thất bại
                }
            }

            // Cập nhật các chi tiết phiếu xuất hiện có
            foreach (var detail in updatedDetails)
            {
                var success = await UpdateDetailAsync(detail, $"{_apiBaseUrl}/api/ChiTietKiemKe/UpdateDetail/{id}/{detail.MaSanPham}");
                if (!success)
                {
                    return false; // Dừng lại nếu cập nhật một chi tiết thất bại
                }
            }

            // Xóa các chi tiết phiếu xuất bị loại bỏ
            foreach (var detail in deletedDetails)
            {
                var deleteResponse = await client.DeleteAsync($"{_apiBaseUrl}/api/ChiTietKiemKe/DeleteDetail/{id}/{detail.MaSanPham}");
                if (!deleteResponse.IsSuccessStatusCode)
                {
                    return false; // Dừng lại nếu không thể xóa một chi tiết
                }
            }

            return true; // Thành công nếu không có lỗi xảy ra
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.DeleteAsync($"{_apiBaseUrl}/api/KiemKe/DeleteInventoryCheck/{id}");
            return response.IsSuccessStatusCode;
        }

        private async Task<bool> CreateDetailAsync(ChiTietKiemKeDto detail, string apiEndpoint)
        {
            var client = _httpClientFactory.CreateClient();
            var requestContent = new MultipartFormDataContent();

            // Gắn mã phiếu kiểm kê cho từng chi tiết
            detail.MaKiemKe = detail.MaKiemKe;

            // Tạo nội dung multipart cho từng chi tiết
            AddKiemkeDataToRequest(detail, requestContent);
            AddImagesToRequest(detail.Images, requestContent);
            var response = await client.PostAsync(apiEndpoint, requestContent);
            return response.IsSuccessStatusCode;
        }

        private async Task<bool> UpdateDetailAsync(ChiTietKiemKeDto detail, string apiEndpoint)
        {
            var client = _httpClientFactory.CreateClient();
            var requestContent = new MultipartFormDataContent();

            // Đảm bảo mã phiếu xuất và mã sản phẩm được cung cấp
            if (detail.MaKiemKe <= 0 || detail.MaSanPham <= 0)
            {
                throw new InvalidOperationException("Mã phiếu kiểm kê hoặc mã sản phẩm không hợp lệ.");
            }

            // Thêm các trường dữ liệu chi tiết phiếu xuất
            AddKiemkeDataToRequest(detail, requestContent);
            AddImagesToRequest(detail.Images, requestContent);
            var response = await client.PutAsync(apiEndpoint, requestContent);
            return response.IsSuccessStatusCode;
        }
        private void AddImagesToRequest(List<IFormFile>? images, MultipartFormDataContent requestContent)
        {
            if (images != null && images.Any())
            {
                // Xử lý ảnh đầu tiên
                var firstImage = images.FirstOrDefault();
                if (firstImage != null)
                {
                    var imageContent = new StreamContent(firstImage.OpenReadStream());
                    imageContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(firstImage.ContentType);
                    requestContent.Add(imageContent, "Images", firstImage.FileName);
                }
                // Xử lý các ảnh tiếp theo cho `Image2`, `Image3`, ...
                for (int i = 1; i < Math.Min(images.Count, 6); i++) // Đảm bảo chỉ xử lý đến `Image6`
                {
                    var image = images[i];
                    var imageContent = new StreamContent(image.OpenReadStream());
                    imageContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(image.ContentType);
                    requestContent.Add(imageContent, "Images", image.FileName); // Key là "Image2", "Image3", ...
                }
            }
        }
        private void AddKiemkeDataToRequest(ChiTietKiemKeDto detail, MultipartFormDataContent requestContent)
        {
            requestContent.Add(new StringContent(detail.MaKiemKe.ToString()), "MaKiemKe");
            requestContent.Add(new StringContent(detail.MaSanPham.ToString()), "MaSanPham");
            requestContent.Add(new StringContent(detail.SoLuongTon.ToString()), "SoLuongTon");
            requestContent.Add(new StringContent(detail.SoLuongThucTe.ToString()), "SoLuongThucTe");
            requestContent.Add(new StringContent(detail.TrangThai.ToString()), "TrangThai");
            requestContent.Add(new StringContent(detail.NguyenNhan ?? string.Empty), "NguyenNhan");
        }
    }
}
