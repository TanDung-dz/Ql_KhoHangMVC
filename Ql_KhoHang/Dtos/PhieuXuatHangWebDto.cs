namespace Ql_KhoHang.Dtos
{
    public class PhieuXuatHangWebDto
    {
        public int MaPhieuXuatHang { get; set; }
        public DateTime? NgayXuat { get; set; }
        public string? HinhThucThanhToan { get; set; }
        public string? PhiVanChuyen { get; set; }
        public int? TrangThai { get; set; }

        // Thông tin cơ bản về người dùng và khách hàng
        public int MaNguoiDung { get; set; }
        public string? TenNguoiDung { get; set; } // Tên người dùng từ NguoiDung

        public int MaKhachHang { get; set; }
        public string? TenKhachHang { get; set; } // Tên khách hàng từ KhachHang
    }

}
