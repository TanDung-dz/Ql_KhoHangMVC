namespace Ql_KhoHang.Dtos
{
    public class KhachHangDto
    {
        public int MaKhachHang { get; set; }

        public string? TenKhachHang { get; set; }

        public string? SoDt { get; set; }

        public string? Diachi { get; set; }

        public string? Email { get; set; }

        public int MaLoai { get; set; }

        public bool? Hide { get; set; }
        public string? TenLoaiKhachHang { get; set; } // Tên loại khách hàng từ LoaiKhacHang
    }
}
