namespace Ql_KhoHang.Dtos
{
    public class NhaCungCapWebDto
    {
        public string? TenNhaCungCap { get; set; }
        public string? DiaChi { get; set; }
        public string? Email { get; set; }
        public int? Sdt { get; set; }
        public string? Image { get; set; }
        public IFormFile? Img { get; set; } // Trường này để nhận file ảnh tải lên
    }
}
