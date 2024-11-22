
namespace Ql_KhoHang.Dtos
{
	public class LoaiKhachHangWebDtos
	{
		public int MaLoai { get; set; }

		public string? TenLoai { get; set; }

		public decimal? ChietKhauXuatHang { get; set; }

		public decimal? ChiPhiVanChuyen { get; set; }
        public bool? Hide { get; set; }
    }
}
