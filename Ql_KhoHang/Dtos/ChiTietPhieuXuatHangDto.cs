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
        public string? Image2 { get; set; }

        public string? Image3 { get; set; }

        public string? Image4 { get; set; }

        public string? Image5 { get; set; }

        public string? Image6 { get; set; }
        public IFormFile? Img { get; set; } // Danh sách file ảnh tải lên
        public List<IFormFile>? Images { get; set; } // Để nhận file ảnh tải lên
    }
}
