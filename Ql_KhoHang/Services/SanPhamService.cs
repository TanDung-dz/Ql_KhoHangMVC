namespace Ql_KhoHang.Services
{
    using Newtonsoft.Json;
    using Ql_KhoHang.Dtos;
    using System.Text;

    public class SanPhamService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _apiBaseUrl;

        public SanPhamService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _apiBaseUrl = configuration["ApiSettings:BaseUrl"];
        }

        public async Task<List<SanPhamDto>> GetAllAsync()
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"{_apiBaseUrl}/api/SanPham/Get");

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var products = JsonConvert.DeserializeObject<List<SanPhamDto>>(data);
                var result = products?.OrderByDescending(p=>p.NgayTao).ToList();
                return result ?? new List<SanPhamDto>();
            }

            return new List<SanPhamDto>();
        }

        public async Task<SanPhamDto> GetByIdAsync(int id)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"{_apiBaseUrl}/api/SanPham/GetById/{id}");

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var product = JsonConvert.DeserializeObject<SanPhamDto>(data);
                return product;

            }

            return null;
        }

        public async Task<bool> CreateAsync(SanPhamDto newProduct, IFormFileCollection images)
        {
            var client = _httpClientFactory.CreateClient();
            var requestContent = new MultipartFormDataContent();

            AddProductDataToRequest(newProduct, requestContent);
            AddImagesToRequest(images, requestContent);

            // Gửi sản phẩm mới đến API
            var response = await client.PostAsync($"{_apiBaseUrl}/api/SanPham/CreateProduct/uploadfile", requestContent);

            if (!response.IsSuccessStatusCode)
                return false;

            // Lấy lại mã sản phẩm từ API
            var responseData = await response.Content.ReadAsStringAsync();
            var createdSp = JsonConvert.DeserializeObject<SanPhamDto>(responseData);
            var spId = createdSp.MaSanPham;

            // Gửi dữ liệu chi tiết vị trí
            if (newProduct.ViTriSanPhams != null && newProduct.ViTriSanPhams.Any())
            {
                foreach (var detail in newProduct.ViTriSanPhams)
                {
                    // Gắn mã sp cho từng chi tiết
                    detail.MaSanPham = spId;

                    // Gửi chi tiết vị trí đến API
                    var detailContent = new StringContent(JsonConvert.SerializeObject(detail), Encoding.UTF8, "application/json");
                    var detailResponse = await client.PostAsync($"{_apiBaseUrl}/api/SanPhamViTri/Create", detailContent);


                    if (!detailResponse.IsSuccessStatusCode)
                    {
                        return false; // Dừng lại nếu lưu chi tiết thất bại
                    }

                }

            }
            return true;
        }

        public async Task<bool> UpdateAsync(int id, SanPhamDto product, IFormFileCollection images,
            List<SanPhamViTriDto> newDetails,
            List<SanPhamViTriDto> updatedDetails,
            List<SanPhamViTriDto> deletedDetails)
        {
            var client = _httpClientFactory.CreateClient();
            var requestContent = new MultipartFormDataContent();

            AddProductDataToRequest(product, requestContent);
            AddImagesToRequest(images, requestContent);

            // Gửi sản phẩm cập nhật đến API
            var response = await client.PutAsync($"{_apiBaseUrl}/api/SanPham/UpdateProduct/{id}", requestContent);

            if (!response.IsSuccessStatusCode)
                return false;
            // Thêm mới các vị trí
            foreach (var detail in newDetails)
            {
                var success = await CreateDetailAsync(detail, $"{_apiBaseUrl}/api/SanPhamViTri/Create");
                if (!success)
                {
                    return false; // Dừng lại nếu thêm mới một chi tiết thất bại
                }
            }

            // Cập nhật các chi tiết hiện có
            foreach (var detail in updatedDetails)
            {
                var success = await UpdateDetailAsync(detail, $"{_apiBaseUrl}/api/SanPhamViTri/Update/{detail.MaViTri}/{detail.MaSanPham}");
                if (!success)
                {
                    return false; // Dừng lại nếu cập nhật một chi tiết thất bại
                }
            }

            // Xóa các chi tiết bị loại bỏ
            foreach (var detail in deletedDetails)
            {
                var deleteResponse = await client.DeleteAsync($"{_apiBaseUrl}/api/SanPhamViTri/Delete/{detail.MaViTri}/{detail.MaSanPham}");
                if (!deleteResponse.IsSuccessStatusCode)
                {
                    return false; // Dừng lại nếu không thể xóa một chi tiết
                }
            }
            return true;
        }


        public async Task<bool> DeleteAsync(int id)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.DeleteAsync($"{_apiBaseUrl}/api/SanPham/DeleteProduct/{id}");

            return response.IsSuccessStatusCode;
        }

        public async Task<List<SanPhamDto>> SearchAsync(string keyword)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"{_apiBaseUrl}/api/SanPham/Search/{keyword}");

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var products = JsonConvert.DeserializeObject<List<SanPhamDto>>(data);
                return products ?? new List<SanPhamDto>();
            }

            return new List<SanPhamDto>();
        }
        private async Task<bool> CreateDetailAsync(SanPhamViTriDto detail, string apiEndpoint)
        {
            var client = _httpClientFactory.CreateClient();
            var detailContent = new StringContent(JsonConvert.SerializeObject(detail), Encoding.UTF8, "application/json");
            var response = await client.PostAsync(apiEndpoint, detailContent);
            return response.IsSuccessStatusCode;
        }

        private async Task<bool> UpdateDetailAsync(SanPhamViTriDto detail, string apiEndpoint)
        {
            var client = _httpClientFactory.CreateClient();
            

            // Đảm bảo mã phiếu xuất và mã sản phẩm được cung cấp
            if (detail.MaViTri <= 0 || detail.MaSanPham <= 0)
            {
                throw new InvalidOperationException("Mã vị trí hoặc mã sản phẩm không hợp lệ.");
            }
            var detailContent = new StringContent(JsonConvert.SerializeObject(detail), Encoding.UTF8, "application/json");
            
            var response = await client.PutAsync(apiEndpoint, detailContent);
            return response.IsSuccessStatusCode;
        }
        private void AddProductDataToRequest(SanPhamDto product, MultipartFormDataContent requestContent)
        {
            requestContent.Add(new StringContent(product.TenSanPham ?? ""), "TenSanPham");
            requestContent.Add(new StringContent(product.Mota ?? ""), "Mota");
            requestContent.Add(new StringContent(product.SoLuong?.ToString() ?? "0"), "SoLuong");
            requestContent.Add(new StringContent(product.DonGia?.ToString() ?? "0"), "DonGia");
            requestContent.Add(new StringContent(product.KhoiLuong?.ToString() ?? "0"), "KhoiLuong");
            requestContent.Add(new StringContent(product.KichThuoc ?? ""), "KichThuoc");
            requestContent.Add(new StringContent(product.XuatXu ?? ""), "XuatXu");
            requestContent.Add(new StringContent(product.MaLoaiSanPham.ToString()), "MaLoaiSanPham");
            requestContent.Add(new StringContent(product.MaHangSanXuat.ToString()), "MaHangSanXuat");
            requestContent.Add(new StringContent(product.MaNhaCungCap.ToString()), "MaNhaCungCap");
        }

        private void AddImagesToRequest(IFormFileCollection images, MultipartFormDataContent requestContent)
        {
            if (images != null && images.Any())
            {
                // Xử lý ảnh đầu tiên cho `Image`
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
        public async Task<List<SanPhamDto>> GetByLoaiSanPhamAsync(string loaiSanPham)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"{_apiBaseUrl}/api/SanPham/Get");
           
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var products = JsonConvert.DeserializeObject<List<SanPhamDto>>(data);
                var result = products.Where(p=>p.TenLoaiSanPham==loaiSanPham).OrderByDescending(p=>p.NgayTao).ToList();
                return result ?? new List<SanPhamDto>();
            }

            return new List<SanPhamDto>();
        }

    }
}
