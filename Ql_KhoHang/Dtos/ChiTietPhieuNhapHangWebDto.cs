namespace Ql_KhoHang.Dtos
{
    public class ChiTietPhieuNhapHangWebDto
    {
        public int MaPhieuNhapHang { get; set; }
        public int MaSanPham { get; set; }
        public int? SoLuong { get; set; }
        public decimal? DonGiaNhap { get; set; }
        public int? TrangThai { get; set; }
        public string? Image { get; set; }
        public IFormFile? Img { get; set; } // Để nhận file ảnh tải lên
    }

}
