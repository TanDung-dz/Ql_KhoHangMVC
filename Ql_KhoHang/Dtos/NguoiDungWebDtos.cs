

namespace Ql_KhoHang.Dtos
{
	public class NguoiDungWebDtos
	{
        public int MaNguoiDung { get; set; }

        public string? TenDangNhap { get; set; }

        public string? MatKhau { get; set; }

        public string? TenNguoiDung { get; set; }

        public string? Email { get; set; }

        public int? Sdt { get; set; }

        public DateTime? NgayDk { get; set; }
        public int? Quyen { get; set; }

        //public virtual ICollection<Blog> Blogs { get; set; } = new List<Blog>();

        //public virtual ICollection<Menu> Menus { get; set; } = new List<Menu>();

        //public virtual ICollection<PhieuNhapHang> PhieuNhapHangs { get; set; } = new List<PhieuNhapHang>();

        //public virtual ICollection<PhieuXuatHang> PhieuXuatHangs { get; set; } = new List<PhieuXuatHang>();
    }
}
