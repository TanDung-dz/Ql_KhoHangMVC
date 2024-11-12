namespace Ql_KhoHang.Dtos
{
	public class SanPhamWebDtos
	{
		public int MaSanPham { get; set; }
		public string? TenSanPham { get; set; }
		public string? Mota { get; set; }
		public int? SoLuong { get; set; }
		public decimal? DonGia { get; set; }
		public string? XuatXu { get; set; }
		public string? Image { get; set; }

		// Thông tin cơ bản về loại sản phẩm và hãng sản xuất
		public int MaLoaiSanPham { get; set; }
		public string? TenLoaiSanPham { get; set; } // Lấy từ LoaiSanPham nếu cần

		public int MaHangSanXuat { get; set; }
		public string? TenHangSanXuat { get; set; } // Lấy từ HangSanXuat nếu cần
	}
}
