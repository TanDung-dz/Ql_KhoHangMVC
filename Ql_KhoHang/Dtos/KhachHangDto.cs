namespace Ql_KhoHang.Dtos
{
    public class KhachHangDto
    {
        public int MaKhachHang { get; set; }
        public string? TenKhachHang { get; set; }

        // Thông tin cơ bản về loại khách hàng
        public int MaLoai { get; set; }
        public string? TenLoaiKhachHang { get; set; } // Tên loại khách hàng từ LoaiKhacHang
    }
}
