namespace Ql_KhoHang.Dtos
{
    public class ChiTietPhieuXuatHangWebDto
    {
        public int MaSanPham { get; set; }
        public int MaPhieuXuatHang { get; set; }
        public int? SoLuong { get; set; }
        public decimal? DonGiaXuat { get; set; }
        public decimal? TienMat { get; set; }
        public decimal? NganHang { get; set; }
        public int? TrangThai { get; set; }
        public string? Image { get; set; }
        public IFormFile? Img { get; set; } // Để nhận file ảnh tải lên
    }

}
