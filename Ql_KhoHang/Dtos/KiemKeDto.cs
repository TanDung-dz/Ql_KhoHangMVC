namespace Ql_KhoHang.Dtos
{
	public class KiemKeDto
	{
		public int MaKiemKe { get; set; }
		public DateTime? NgayKiemKe { get; set; }
		public bool? Hide { get; set; }
		// Thông tin cơ bản về nhân viên kho
		public int MaNhanVienKho { get; set; }
		public string? TenNhanVienKho { get; set; } // Tên nhân viên kho từ NhanVienKho

        public List<ChiTietKiemKeDto> Details { get; set; } = new List<ChiTietKiemKeDto>();                                           // Danh sách chi tiết kiểm kê liên quan

    }
}
