namespace Ql_KhoHang.Dtos
{
	public class SanPhamDto
	{
		public int MaSanPham { get; set; }

        public int MaLoaiSanPham { get; set; }
        public string? TenLoaiSanPham { get; set; } // Lấy từ LoaiSanPham nếu cần
        public string? TenHangSanXuat { get; set; } // Lấy từ HangSanXuat nếu cần
        public int MaHangSanXuat { get; set; }

        public string? TenSanPham { get; set; }

        public string? Mota { get; set; }

        public int? SoLuong { get; set; }

        public decimal? DonGia { get; set; }

        public double? KhoiLuong { get; set; }

        public string? KichThuoc { get; set; }

        public string? XuatXu { get; set; }

        public string? Image { get; set; }

        public string? Image2 { get; set; }

        public string? Image3 { get; set; }

        public string? Image4 { get; set; }

        public string? Image5 { get; set; }

        public string? MaVach { get; set; }

        public bool? Hide { get; set; }

        public DateTime? NgayTao { get; set; }

        public DateTime? NgayCapNhat { get; set; }

        public bool? TrangThai { get; set; }

        public int MaNhaCungCap { get; set; }
        public string? TenNhaCungCap { get; set; }
		// Thêm danh sách vị trí của sản phẩm
        public List<SanPhamViTriDto>? ViTriSanPhams { get; set; }
    }
}
