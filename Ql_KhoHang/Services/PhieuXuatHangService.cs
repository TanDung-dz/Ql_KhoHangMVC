using Newtonsoft.Json;
using Ql_KhoHang.Dtos;
using System.Text;

namespace Ql_KhoHang.Services
{
    public class PhieuXuatHangService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _apiBaseUrl;

        public PhieuXuatHangService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _apiBaseUrl = configuration["ApiSettings:BaseUrl"];
        }

        public async Task<List<PhieuXuatHangDto>> GetAllAsync(string? keyword)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"{_apiBaseUrl}/api/PhieuXuatHang/Get?keyword={keyword}");

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var phieu = JsonConvert.DeserializeObject<List<PhieuXuatHangDto>>(data);
                return phieu.OrderByDescending(p => p.NgayXuat).ToList();
            }

            return new List<PhieuXuatHangDto>();
        }

        public async Task<PhieuXuatHangDto> GetByIdAsync(int id)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"{_apiBaseUrl}/api/PhieuXuatHang/GetById/{id}");

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<PhieuXuatHangDto>(data);
            }

            return null;
        }

        public async Task<bool> CreateAsync(PhieuXuatHangDto newOrder)
        {
            var client = _httpClientFactory.CreateClient();

            // Gửi phiếu xuất hàng đến API
            var orderContent = new StringContent(JsonConvert.SerializeObject(newOrder), Encoding.UTF8, "application/json");
            var response = await client.PostAsync($"{_apiBaseUrl}/api/PhieuXuatHang/CreateExportOrder", orderContent);

            if (!response.IsSuccessStatusCode)
            {
                return false; // Dừng lại nếu lưu phiếu xuất hàng thất bại
            }

            // Lấy lại mã phiếu xuất hàng từ API
            var responseData = await response.Content.ReadAsStringAsync();
            var createdOrder = JsonConvert.DeserializeObject<PhieuXuatHangDto>(responseData);
            var createdOrderId = createdOrder.MaPhieuXuatHang;

            // Gửi dữ liệu chi tiết phiếu xuất hàng
            if (newOrder.Details != null && newOrder.Details.Any())
            {
                foreach (var detail in newOrder.Details)
                {
                    // Gắn mã phiếu xuất hàng cho từng chi tiết
                    detail.MaPhieuXuatHang = createdOrderId;

                    // Tạo nội dung multipart cho từng chi tiết
                    var detailContent = new MultipartFormDataContent();
                    AddDataToRequest(detail, detailContent);
                    AddImagesToRequest(detail.Images, detailContent);

                    // Gửi chi tiết phiếu xuất hàng đến API
                    var detailResponse = await client.PostAsync($"{_apiBaseUrl}/api/ChiTietPhieuXuatHang/CreateDetailWithImage/uploadfile", detailContent);

                    if (!detailResponse.IsSuccessStatusCode)
                    {
                        return false; // Dừng lại nếu một chi tiết phiếu không được lưu
                    }
                }
            }

            return true; // Thành công nếu cả phiếu xuất và chi tiết phiếu được lưu
        }

        public async Task<bool> UpdateAsync(
            int id,
            PhieuXuatHangDto updatedOrder,
            List<ChiTietPhieuXuatHangDto> newDetails,
            List<ChiTietPhieuXuatHangDto> updatedDetails,
            List<ChiTietPhieuXuatHangDto> deletedDetails)
        {
            var client = _httpClientFactory.CreateClient();

            // Cập nhật thông tin phiếu xuất hàng
            var orderContent = new StringContent(JsonConvert.SerializeObject(updatedOrder), Encoding.UTF8, "application/json");
            var response = await client.PutAsync($"{_apiBaseUrl}/api/PhieuXuatHang/UpdateExportOrder/{id}", orderContent);
            if (!response.IsSuccessStatusCode)
            {
                return false; // Dừng lại nếu cập nhật phiếu xuất hàng thất bại
            }

            // Thêm mới các chi tiết phiếu xuất
            foreach (var detail in newDetails)
            {
                detail.MaPhieuXuatHang = id; // Gắn mã phiếu xuất cho từng chi tiết
                var success = await CreateDetailAsync(detail, $"{_apiBaseUrl}/api/ChiTietPhieuXuatHang/CreateDetailWithImage/uploadfile");
                if (!success)
                {
                    return false; // Dừng lại nếu thêm mới một chi tiết thất bại
                }
            }

            // Cập nhật các chi tiết phiếu xuất hiện có
            foreach (var detail in updatedDetails)
            {
                var success = await UpdateDetailAsync(detail, $"{_apiBaseUrl}/api/ChiTietPhieuXuatHang/UpdateDetail/{id}/{detail.MaSanPham}");
                if (!success)
                {
                    return false; // Dừng lại nếu cập nhật một chi tiết thất bại
                }
            }

            // Xóa các chi tiết phiếu xuất bị loại bỏ
            foreach (var detail in deletedDetails)
            {
                var deleteResponse = await client.DeleteAsync($"{_apiBaseUrl}/api/ChiTietPhieuXuatHang/DeleteDetail/{id}/{detail.MaSanPham}");
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
            var response = await client.DeleteAsync($"{_apiBaseUrl}/api/PhieuXuatHang/DeleteExportOrder/{id}");
            return response.IsSuccessStatusCode;
        }

        private async Task<bool> CreateDetailAsync(ChiTietPhieuXuatHangDto detail, string apiEndpoint)
        {
            var client = _httpClientFactory.CreateClient();
            var requestContent = new MultipartFormDataContent();

            // Đảm bảo mã phiếu xuất và mã sản phẩm được cung cấp
            if (detail.MaPhieuXuatHang <= 0 || detail.MaSanPham <= 0)
            {
                throw new InvalidOperationException("Mã phiếu xuất hàng hoặc mã sản phẩm không hợp lệ.");
            }

            // Thêm các trường dữ liệu chi tiết phiếu xuất
            AddDataToRequest(detail, requestContent);
            AddImagesToRequest(detail.Images, requestContent);

            var response = await client.PostAsync(apiEndpoint, requestContent);
            return response.IsSuccessStatusCode;
        }

        private async Task<bool> UpdateDetailAsync(ChiTietPhieuXuatHangDto detail, string apiEndpoint)
        {
            var client = _httpClientFactory.CreateClient();
            var requestContent = new MultipartFormDataContent();

            // Đảm bảo mã phiếu xuất và mã sản phẩm được cung cấp
            if (detail.MaPhieuXuatHang <= 0 || detail.MaSanPham <= 0)
            {
                throw new InvalidOperationException("Mã phiếu xuất hàng hoặc mã sản phẩm không hợp lệ.");
            }

            // Thêm các trường dữ liệu chi tiết phiếu xuất
            AddDataToRequest(detail, requestContent);
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
        private void AddDataToRequest(ChiTietPhieuXuatHangDto detail, MultipartFormDataContent requestContent)
        {
            requestContent.Add(new StringContent(detail.MaPhieuXuatHang.ToString()), "MaPhieuXuatHang");
            requestContent.Add(new StringContent(detail.MaSanPham.ToString()), "MaSanPham");
            requestContent.Add(new StringContent(detail.SoLuong.ToString()), "SoLuong");
            requestContent.Add(new StringContent("0"), "DonGiaXuat");
            requestContent.Add(new StringContent("0"), "TienMat");
            requestContent.Add(new StringContent("0"), "NganHang");
            requestContent.Add(new StringContent("1"), "TrangThai");
        }
		public async Task<Dictionary<string, int>> GetStatisticsByMonthAsync()
		{
			var client = _httpClientFactory.CreateClient();
			var response = await client.GetAsync($"{_apiBaseUrl}/api/PhieuXuatHang/Get");

			if (response.IsSuccessStatusCode)
			{
				var data = await response.Content.ReadAsStringAsync();
				var phieuXuatList = JsonConvert.DeserializeObject<List<PhieuXuatHangDto>>(data);

				// Tổng hợp số lượng phiếu xuất theo tháng
				var statistics = phieuXuatList
					.GroupBy(p => p.NgayXuat.HasValue ? p.NgayXuat.Value.ToString("yyyy-MM") : "Unknown")
					.ToDictionary(g => g.Key, g => g.Count());

				return statistics;
			}

			return new Dictionary<string, int>();
		}

	}
}
