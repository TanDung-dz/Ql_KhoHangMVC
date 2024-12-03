namespace Ql_KhoHang.Dtos
{
    public class ChiTietPhieuXuatHangDto
    {
        public int MaSanPham { get; set; }
        public string? TenSanPham { get; set; }
        public int MaPhieuXuatHang { get; set; }
        public int? SoLuong { get; set; }
        public decimal? DonGiaXuat { get; set; }
        public decimal? TienMat { get; set; }
        public decimal? NganHang { get; set; }
        public int? TrangThai { get; set; }
        public string? Image { get; set; }
        public IFormFile? Img { get; set; } // Tệp ảnh
    }
}
