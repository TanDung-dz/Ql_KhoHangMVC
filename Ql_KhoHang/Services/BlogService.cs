using Newtonsoft.Json;
using Ql_KhoHang.Dtos;
using System.Text;

namespace Ql_KhoHang.Services
{
    public class BlogService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _apiBaseUrl;

        public BlogService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _apiBaseUrl = configuration["ApiSettings:BaseUrl"];
        }

        public async Task<List<BlogDto>> GetAllAsync()
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"{_apiBaseUrl}/api/Blog/Get");

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var blogs = JsonConvert.DeserializeObject<List<BlogDto>>(data);
				foreach (var blog in blogs)
				{
					if (!string.IsNullOrEmpty(blog.Anh))
					{
						blog.Anh = $"{_apiBaseUrl}{blog.Anh}";
					}
				}
                return blogs;

            }

            return new List<BlogDto>();
        }

        public async Task<BlogDto> GetByIdAsync(int id)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"{_apiBaseUrl}/api/Blog/GetById/{id}");

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var blog = JsonConvert.DeserializeObject<BlogDto>(data);
				if (!string.IsNullOrEmpty(blog.Anh))
				{
					blog.Anh = $"{_apiBaseUrl}{blog.Anh}";
				}
				return blog;
            }

            return null;
        }

        public async Task<bool> CreateAsync(BlogDto newBlog, IFormFile Img)
        {
            var client = _httpClientFactory.CreateClient();
            var requestContent = new MultipartFormDataContent();

            // Thêm thông tin blog
            requestContent.Add(new StringContent(newBlog.Mota ?? ""), "Mota");
            requestContent.Add(new StringContent(newBlog.Link ?? ""), "Link");
            requestContent.Add(new StringContent(newBlog.Hide.ToString() ?? "false"), "Hide");
            requestContent.Add(new StringContent(newBlog.MaNguoiDung.ToString()), "MaNguoiDung");

            // Thêm file ảnh nếu có
            if (Img != null && Img.Length > 0)
            {
                var imageContent = new StreamContent(Img.OpenReadStream());
                imageContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(Img.ContentType);
                requestContent.Add(imageContent, "Image", Img.FileName);
            }

            var response = await client.PostAsync($"{_apiBaseUrl}/api/Blog/CreateWithImage/uploadfile", requestContent);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateAsync(int id, BlogDto updatedBlog, IFormFile Img)
        {
            var client = _httpClientFactory.CreateClient();
            var requestContent = new MultipartFormDataContent();

            // Thêm thông tin blog
            requestContent.Add(new StringContent(updatedBlog.Mota ?? ""), "Mota");
            requestContent.Add(new StringContent(updatedBlog.Link ?? ""), "Link");
            requestContent.Add(new StringContent(updatedBlog.Hide.ToString() ?? "false"), "Hide");
            requestContent.Add(new StringContent(updatedBlog.MaNguoiDung.ToString()), "MaNguoiDung");

            // Thêm file ảnh nếu có
            if (Img != null && Img.Length > 0)
            {
                var imageContent = new StreamContent(Img.OpenReadStream());
                imageContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(Img.ContentType);
                requestContent.Add(imageContent, "Image", Img.FileName);
            }

            var response = await client.PutAsync($"{_apiBaseUrl}/api/Blog/Update/{id}", requestContent);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.DeleteAsync($"{_apiBaseUrl}/api/Blog/Delete/{id}");

            return response.IsSuccessStatusCode;
        }

        public async Task<List<BlogDto>> SearchAsync(string keyword)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"{_apiBaseUrl}/api/Blog/Search/{keyword}");

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var blogs = JsonConvert.DeserializeObject<List<BlogDto>>(data);
                foreach (var blog in blogs)
                {
                    if (!string.IsNullOrEmpty(blog.Anh))
                    {
                        blog.Anh = $"{_apiBaseUrl}{blog.Anh}";
                    }
                }
                return blogs;
            }

            return new List<BlogDto>();
        }
    }
}
