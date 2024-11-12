
namespace Ql_KhoHang.Dtos
{
	public class MenuWebDtos
	{
		public int MenuId { get; set; }
		public string? Name { get; set; }
		public int? Order { get; set; }
		public string? Link { get; set; }
		public bool? Hide { get; set; }

		// Thông tin cơ bản về người dùng
		public int MaNguoiDung { get; set; }
		public string? TenNguoiDung { get; set; } // Tên người dùng từ NguoiDung
	}
}
