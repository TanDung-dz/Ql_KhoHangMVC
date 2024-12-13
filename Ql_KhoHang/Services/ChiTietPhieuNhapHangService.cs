using Newtonsoft.Json;
using Ql_KhoHang.Dtos;
using System.Text;

namespace Ql_KhoHang.Services
{
    public class ChiTietPhieuNhapHangService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _apiBaseUrl;
        private readonly PhieuNhapHangService _phieuNhapHangService;
        public ChiTietPhieuNhapHangService(IHttpClientFactory httpClientFactory, IConfiguration configuration, PhieuNhapHangService phieuNhapHangService)
        {
            _httpClientFactory = httpClientFactory;
            _apiBaseUrl = configuration["ApiSettings:BaseUrl"];
            _phieuNhapHangService = phieuNhapHangService;
        }

        public async Task<List<ChiTietPhieuNhapHangDto>> GetByImportOrderIdAsync(int id)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"{_apiBaseUrl}/api/ChiTietPhieuNhapHang/GetById/{id}");

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var details = JsonConvert.DeserializeObject<List<ChiTietPhieuNhapHangDto>>(data);
                return details.OrderByDescending(p=>p.SoLuong).ToList();
            }
            return new List<ChiTietPhieuNhapHangDto>();
        }
        public async Task<List<PhieuNhapHangDto>> GetByImportOrderProductIdAsync(int productId)
        {
            var client = _httpClientFactory.CreateClient();

            // Bước 1: Lấy danh sách chi tiết phiếu nhập theo sản phẩm
            var response = await client.GetAsync($"{_apiBaseUrl}/api/ChiTietPhieuNhapHang/GetByProductId/{productId}");
            if (!response.IsSuccessStatusCode)
            {
                return new List<PhieuNhapHangDto>(); // Trả về danh sách rỗng nếu không thành công
            }

            var data = await response.Content.ReadAsStringAsync();
            var details = JsonConvert.DeserializeObject<List<ChiTietPhieuNhapHangDto>>(data);

            // Nếu không có chi tiết phiếu nhập, trả về danh sách rỗng
            if (details == null || !details.Any())
            {
                return new List<PhieuNhapHangDto>();
            }

            // Bước 2: Lấy toàn bộ danh sách phiếu nhập
            var allPhieuNhap = await _phieuNhapHangService.GetAllAsync();

            // Bước 3: Lọc phiếu nhập dựa vào mã phiếu nhập trong danh sách chi tiết phiếu nhập
            var filteredPhieuNhap = allPhieuNhap
                .Where(p => details.Any(d => d.MaPhieuNhapHang == p.MaPhieuNhapHang))
                .OrderBy(p => p.NgayNhap) // Sắp xếp theo ngày nhập
                .ToList();

            return filteredPhieuNhap; // Trả về danh sách phiếu nhập liên quan
        }

        public async Task<List<ChiTietPhieuNhapHangDto>> GetAllAsync()
		{
			var client = _httpClientFactory.CreateClient();
			var response = await client.GetAsync($"{_apiBaseUrl}/api/ChiTietPhieuNhapHang/Get");

			if (response.IsSuccessStatusCode)
			{
				var data = await response.Content.ReadAsStringAsync();
				return JsonConvert.DeserializeObject<List<ChiTietPhieuNhapHangDto>>(data);
			}

			return new List<ChiTietPhieuNhapHangDto>();
		}
	}
}
