using Microsoft.AspNetCore.Mvc;
using Ql_KhoHang.Dtos;
using Ql_KhoHang.Services;

namespace Ql_KhoHang.Controllers
{
    public class ChiTietPhieuNhapHangController : Controller
    {
        private readonly ChiTietPhieuNhapHangService _chiTietService;

        public ChiTietPhieuNhapHangController(ChiTietPhieuNhapHangService chiTietService)
        {
            _chiTietService = chiTietService;
        }

        [HttpGet]
        public async Task<IActionResult> GetByImportOrderId(int id)
        {
            var details = await _chiTietService.GetByImportOrderIdAsync(id);
            return Json(details);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ChiTietPhieuNhapHangDto detail, IFormFile Img)
        {
            if (ModelState.IsValid)
            {
                var success = await _chiTietService.CreateDetailAsync(detail, Img);
                if (success)
                {
                    return Json(new { success = true, message = "Chi tiết phiếu nhập đã được thêm thành công." });
                }
            }
            return Json(new { success = false, message = "Không thể thêm chi tiết phiếu nhập." });
        }

        [HttpPost]
        public async Task<IActionResult> Update(ChiTietPhieuNhapHangDto detail, IFormFile Img)
        {
            if (ModelState.IsValid)
            {
                var success = await _chiTietService.UpdateDetailAsync(detail, Img);
                if (success)
                {
                    return Json(new { success = true, message = "Chi tiết phiếu nhập đã được cập nhật thành công." });
                }
            }
            return Json(new { success = false, message = "Không thể cập nhật chi tiết phiếu nhập." });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int maPhieuNhapHang, int maSanPham)
        {
            var success = await _chiTietService.DeleteDetailAsync(maPhieuNhapHang, maSanPham);
            if (success)
            {
                return Json(new { success = true, message = "Chi tiết phiếu nhập đã được xóa thành công." });
            }
            return Json(new { success = false, message = "Không thể xóa chi tiết phiếu nhập." });
        }
    }
}
