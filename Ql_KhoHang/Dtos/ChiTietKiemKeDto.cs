namespace Ql_KhoHang.Dtos
{
	public class ChiTietKiemKeDto
	{
		public int MaKiemKe { get; set; }
		public int MaSanPham { get; set; }
		public string? TenSanPham { get; set; }
		public int? SoLuongTon { get; set; }
		public int? SoLuongThucTe { get; set; }

		public int? TrangThai { get; set; }

		public string? NguyenNhan { get; set; }

		public string? Anh { get; set; }
	}
}
