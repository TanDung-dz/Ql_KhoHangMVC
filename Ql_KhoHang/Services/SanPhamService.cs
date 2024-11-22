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

        public async Task<List<SanPhamWebDtos>> GetAllAsync()
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"{_apiBaseUrl}/api/SanPham/Get");

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var products = JsonConvert.DeserializeObject<List<SanPhamWebDtos>>(data);

                // Gắn URL đầy đủ cho ảnh
                foreach (var product in products)
                {
                    if (!string.IsNullOrEmpty(product.Image))
                    {
                        product.Image = $"{_apiBaseUrl}{product.Image}";
                    }
                    if (!string.IsNullOrEmpty(product.MaVach))
                    {
                        product.MaVach = $"{_apiBaseUrl}{product.MaVach}";
                    }
                }

                return products;
            }

            return new List<SanPhamWebDtos>();
        }

        public async Task<SanPhamWebDtos> GetByIdAsync(int id)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"{_apiBaseUrl}/api/SanPham/GetById/{id}");

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var product = JsonConvert.DeserializeObject<SanPhamWebDtos>(data);
				if (!string.IsNullOrEmpty(product.Image))
				{
					product.Image = $"{_apiBaseUrl}{product.Image}";
				}
                if (!string.IsNullOrEmpty(product.MaVach))
                {
                    product.MaVach = $"{_apiBaseUrl}{product.MaVach}";
                }
                return product;

            }

            return null;
        }

        public async Task<bool> CreateAsync(SanPhamWebDtos newProduct, IFormFile Img)
        {
            var client = _httpClientFactory.CreateClient();
            var requestContent = new MultipartFormDataContent();

            // Thêm thông tin sản phẩm
            requestContent.Add(new StringContent(newProduct.TenSanPham ?? ""), "TenSanPham");
            requestContent.Add(new StringContent(newProduct.Mota ?? ""), "Mota");
            requestContent.Add(new StringContent(newProduct.SoLuong.ToString() ?? "0"), "SoLuong");
            requestContent.Add(new StringContent(newProduct.DonGia.ToString() ?? "0"), "DonGia");
            requestContent.Add(new StringContent(newProduct.XuatXu ?? ""), "XuatXu");
            requestContent.Add(new StringContent(newProduct.MaLoaiSanPham.ToString()), "MaLoaiSanPham");
            requestContent.Add(new StringContent(newProduct.MaHangSanXuat.ToString()), "MaHangSanXuat");

            // Thêm file ảnh nếu có
            if (Img != null && Img.Length > 0)
            {
                var imageContent = new StreamContent(Img.OpenReadStream());
                imageContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(Img.ContentType);
                requestContent.Add(imageContent, "Img", Img.FileName);
            }

            var response = await client.PostAsync($"{_apiBaseUrl}/api/SanPham/CreateProduct/uploadfile", requestContent);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateAsync(int id, SanPhamWebDtos product, IFormFile Img)
        {
            var client = _httpClientFactory.CreateClient();
            var requestContent = new MultipartFormDataContent();

            // Thêm các thông tin sản phẩm vào request
            requestContent.Add(new StringContent(product.TenSanPham ?? ""), "TenSanPham");
            requestContent.Add(new StringContent(product.Mota ?? ""), "Mota");
            requestContent.Add(new StringContent(product.SoLuong.ToString() ?? "0"), "SoLuong");
            requestContent.Add(new StringContent(product.DonGia.ToString() ?? "0"), "DonGia");
            requestContent.Add(new StringContent(product.XuatXu ?? ""), "XuatXu");
            requestContent.Add(new StringContent(product.MaLoaiSanPham.ToString()), "MaLoaiSanPham");
            requestContent.Add(new StringContent(product.MaHangSanXuat.ToString()), "MaHangSanXuat");

            // Thêm file ảnh nếu có
            if (Img != null && Img.Length > 0)
            {
                var imageContent = new StreamContent(Img.OpenReadStream());
                imageContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(Img.ContentType);
                requestContent.Add(imageContent, "Img", Img.FileName);
            }

            // Gửi yêu cầu PUT đến API
            var response = await client.PutAsync($"{_apiBaseUrl}/api/SanPham/UpdateProduct/{id}", requestContent);

            return response.IsSuccessStatusCode;
        }


        public async Task<bool> DeleteAsync(int id)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.DeleteAsync($"{_apiBaseUrl}/api/SanPham/DeleteProduct/{id}");

            return response.IsSuccessStatusCode;
        }

        public async Task<List<SanPhamWebDtos>> SearchAsync(string keyword)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"{_apiBaseUrl}/api/SanPham/Search/{keyword}");

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var products = JsonConvert.DeserializeObject<List<SanPhamWebDtos>>(data);

                // Gắn URL đầy đủ cho ảnh
                foreach (var product in products)
                {
                    if (!string.IsNullOrEmpty(product.Image))
                    {
                        product.Image = $"{_apiBaseUrl}{product.Image}";
                    }
                }

                return products;
            }

            return new List<SanPhamWebDtos>();
        }
    }

}
