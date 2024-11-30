using Newtonsoft.Json;
using Ql_KhoHang.Dtos;
using System.Text;

namespace Ql_KhoHang.Services
{
    public class PhieuNhapHangService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _apiBaseUrl;

        public PhieuNhapHangService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _apiBaseUrl = configuration["ApiSettings:BaseUrl"];
        }

        public async Task<List<PhieuNhapHangDto>> GetAllAsync(string? keyword)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"{_apiBaseUrl}/api/PhieuNhapHang/Get?keyword={keyword}");

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<PhieuNhapHangDto>>(data);
            }

            return new List<PhieuNhapHangDto>();
        }

        public async Task<PhieuNhapHangDto> GetByIdAsync(int id)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"{_apiBaseUrl}/api/PhieuNhapHang/GetById/{id}");

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<PhieuNhapHangDto>(data);
            }

            return null;
        }

        public async Task<bool> CreateAsync(PhieuNhapHangDto newOrder)
        {
            var client = _httpClientFactory.CreateClient();

            // Gửi phiếu nhập hàng đến API
            var orderContent = new StringContent(JsonConvert.SerializeObject(newOrder), Encoding.UTF8, "application/json");
            var response = await client.PostAsync($"{_apiBaseUrl}/api/PhieuNhapHang/CreateImportOrder", orderContent);

            if (!response.IsSuccessStatusCode)
            {
                return false; // Dừng lại nếu lưu phiếu nhập hàng thất bại
            }

            // Lấy lại mã phiếu nhập hàng từ API
            var responseData = await response.Content.ReadAsStringAsync();
            var createdOrder = JsonConvert.DeserializeObject<PhieuNhapHangDto>(responseData);
            var createdOrderId = createdOrder.MaPhieuNhapHang;

            // Gửi dữ liệu chi tiết phiếu nhập hàng
            if (newOrder.Details != null && newOrder.Details.Any())
            {
                foreach (var detail in newOrder.Details)
                {
                    // Gắn mã phiếu nhập hàng cho từng chi tiết
                    detail.MaPhieuNhapHang = createdOrderId;

                    // Tạo nội dung multipart cho từng chi tiết
                    var detailContent = new MultipartFormDataContent();
                    detailContent.Add(new StringContent(detail.MaPhieuNhapHang.ToString()), "MaPhieuNhapHang");
                    detailContent.Add(new StringContent(detail.MaSanPham.ToString()), "MaSanPham");
                    detailContent.Add(new StringContent(detail.SoLuong.ToString()), "SoLuong");
                    detailContent.Add(new StringContent(detail.DonGiaNhap.ToString()), "DonGiaNhap");

                    // Xử lý ảnh nếu có
                    if (detail.Img != null && detail.Img.Length > 0)
                    {
                        var imageContent = new StreamContent(detail.Img.OpenReadStream());
                        imageContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(detail.Img.ContentType);
                        detailContent.Add(imageContent, "Img", detail.Img.FileName);
                    }

                    // Gửi chi tiết phiếu nhập hàng đến API
                    var detailResponse = await client.PostAsync($"{_apiBaseUrl}/api/ChiTietPhieuNhapHang/CreateDetailWithImage/uploadfile", detailContent);

                    if (!detailResponse.IsSuccessStatusCode)
                    {
                        return false; // Dừng lại nếu một chi tiết phiếu không được lưu
                    }
                }
            }

            return true; // Thành công nếu cả phiếu nhập và chi tiết phiếu được lưu
        }
        public async Task<bool> UpdateAsync(
    int id,
    PhieuNhapHangDto updatedOrder,
    List<ChiTietPhieuNhapHangDto> newDetails,
    List<ChiTietPhieuNhapHangDto> updatedDetails,
    List<ChiTietPhieuNhapHangDto> deletedDetails)
        {
            var client = _httpClientFactory.CreateClient();

            // Cập nhật thông tin phiếu nhập hàng
            var orderContent = new StringContent(JsonConvert.SerializeObject(updatedOrder), Encoding.UTF8, "application/json");
            var response = await client.PutAsync($"{_apiBaseUrl}/api/PhieuNhapHang/UpdateImportOrder/{id}", orderContent);
            if (!response.IsSuccessStatusCode)
            {
                return false; // Dừng lại nếu cập nhật phiếu nhập hàng thất bại
            }

            // Thêm mới các chi tiết phiếu nhập
            foreach (var detail in newDetails)
            {
                detail.MaPhieuNhapHang = id; // Gắn mã phiếu nhập cho từng chi tiết
                var success = await CreateDetailAsync(detail, $"{_apiBaseUrl}/api/ChiTietPhieuNhapHang/CreateDetailWithImage/uploadfile");
                if (!success)
                {
                    return false; // Dừng lại nếu thêm mới một chi tiết thất bại
                }
            }

            // Cập nhật các chi tiết phiếu nhập hiện có
            foreach (var detail in updatedDetails)
            {
                
                var success = await UpdateDetailAsync(detail, $"{_apiBaseUrl}/api/ChiTietPhieuNhapHang/UpdateDetail/{id}/{detail.MaSanPham}");
                if (!success)
                {
                    return false; // Dừng lại nếu cập nhật một chi tiết thất bại
                }
            }

            // Xóa các chi tiết phiếu nhập bị loại bỏ
            foreach (var detail in deletedDetails)
            {
                var deleteResponse = await client.DeleteAsync($"{_apiBaseUrl}/api/ChiTietPhieuNhapHang/DeleteDetail/{id}/{detail.MaSanPham}");
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
            var response = await client.DeleteAsync($"{_apiBaseUrl}/api/PhieuNhapHang/DeleteImportOrder/{id}");

            return response.IsSuccessStatusCode;
        }
        private async Task<bool> CreateDetailAsync(ChiTietPhieuNhapHangDto detail, string apiEndpoint)
        {
            var client = _httpClientFactory.CreateClient();
            var requestContent = new MultipartFormDataContent();

            // Đảm bảo mã phiếu nhập và mã sản phẩm được cung cấp
            if (detail.MaPhieuNhapHang <= 0 || detail.MaSanPham <= 0)
            {
                throw new InvalidOperationException("Mã phiếu nhập hàng hoặc mã sản phẩm không hợp lệ.");
            }

            // Thêm các trường dữ liệu chi tiết phiếu nhập
            requestContent.Add(new StringContent(detail.MaPhieuNhapHang.ToString()), "MaPhieuNhapHang");
            requestContent.Add(new StringContent(detail.MaSanPham.ToString()), "MaSanPham");
            requestContent.Add(new StringContent(detail.SoLuong.ToString()), "SoLuong");
            requestContent.Add(new StringContent(detail.DonGiaNhap.ToString()), "DonGiaNhap");
            requestContent.Add(new StringContent(detail.TrangThai.ToString() ?? ""), "TrangThai");

            // Thêm file ảnh (nếu có)
            if (detail.Img != null && detail.Img.Length > 0)
            {
                var imageContent = new StreamContent(detail.Img.OpenReadStream());
                imageContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(detail.Img.ContentType);
                requestContent.Add(imageContent, "Img", detail.Img.FileName);
            }

            // Gửi request đến API
            var response = await client.PostAsync(apiEndpoint, requestContent);
            return response.IsSuccessStatusCode;
        }
        private async Task<bool> UpdateDetailAsync(ChiTietPhieuNhapHangDto detail, string apiEndpoint)
        {
            var client = _httpClientFactory.CreateClient();
            var requestContent = new MultipartFormDataContent();

            // Đảm bảo mã phiếu nhập và mã sản phẩm được cung cấp
            if (detail.MaPhieuNhapHang <= 0 || detail.MaSanPham <= 0)
            {
                throw new InvalidOperationException("Mã phiếu nhập hàng hoặc mã sản phẩm không hợp lệ.");
            }

            // Thêm các trường dữ liệu chi tiết phiếu nhập
            requestContent.Add(new StringContent(detail.MaSanPham.ToString()), "MaSanPham");
            requestContent.Add(new StringContent(detail.SoLuong.ToString()), "SoLuong");
            requestContent.Add(new StringContent(detail.DonGiaNhap.ToString()), "DonGiaNhap");
            requestContent.Add(new StringContent(detail.TrangThai.ToString() ?? ""), "TrangThai");

            // Thêm file ảnh (nếu có)
            if (detail.Img != null && detail.Img.Length > 0)
            {
                var imageContent = new StreamContent(detail.Img.OpenReadStream());
                imageContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(detail.Img.ContentType);
                requestContent.Add(imageContent, "Img", detail.Img.FileName);
            }

            // Gửi request đến API
            var response = await client.PutAsync(apiEndpoint, requestContent);
            return response.IsSuccessStatusCode;
        }
    }
}
