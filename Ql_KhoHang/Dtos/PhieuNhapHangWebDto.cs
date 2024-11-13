namespace Ql_KhoHang.Dtos
{
    public class PhieuNhapHangWebDto
    {
        public int MaPhieuNhapHang { get; set; }
        public DateTime? NgayNhap { get; set; }
        public decimal? PhiVanChuyen { get; set; }
        public int? TrangThai { get; set; }

        // Thông tin cơ bản về người dùng và nhà cung cấp
        public int MaNguoiDung { get; set; }
        public string? TenNguoiDung { get; set; } // Tên người dùng từ NguoiDung

        public int MaNhaCungCap { get; set; }
        public string? TenNhaCungCap { get; set; } // Tên nhà cung cấp từ NhaCungCap
    }

}
