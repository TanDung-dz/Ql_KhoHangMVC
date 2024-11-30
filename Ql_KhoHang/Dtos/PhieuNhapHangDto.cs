namespace Ql_KhoHang.Dtos
{
    public class PhieuNhapHangDto
    {
        public int MaPhieuNhapHang { get; set; }
        public DateTime? NgayNhap { get; set; }
        public decimal? PhiVanChuyen { get; set; }
        public int? TrangThai { get; set; }
        public bool? Hide { get; set; }
        // Thông tin cơ bản về người dùng và nhà cung cấp
        public int MaNguoiDung { get; set; }
        public string? TenNguoiDung { get; set; } // Tên người dùng từ NguoiDung

        public int MaNhaCungCap { get; set; }
        public string? TenNhaCungCap { get; set; } // Tên nhà cung cấp từ NhaCungCap
        // Danh sách chi tiết phiếu nhập hàng
        public List<ChiTietPhieuNhapHangDto> Details { get; set; } = new List<ChiTietPhieuNhapHangDto>();
    }
}
