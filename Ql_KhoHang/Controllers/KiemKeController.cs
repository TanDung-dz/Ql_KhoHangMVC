using Microsoft.AspNetCore.Mvc;
using Ql_KhoHang.Dtos;
using Ql_KhoHang.Services;

namespace Ql_KhoHang.Controllers
{
	public class KiemKeController : Controller
	{
		private readonly KiemKeService _kiemKeService;

		public KiemKeController(KiemKeService kiemKeService)
		{
			_kiemKeService = kiemKeService;
		}

		[HttpGet]
		public async Task<IActionResult> Index()
		{
			var kiemKeList = await _kiemKeService.GetKiemKeAsync();
			return View(kiemKeList);
		}

		[HttpGet]
		public async Task<IActionResult> Details(int id)
		{
			var kiemKe = await _kiemKeService.GetKiemKeByIdAsync(id);
			if (kiemKe == null)
			{
				TempData["ErrorMessage"] = "Không tìm thấy phiếu kiểm kê.";
				return RedirectToAction("Index");
			}

			var chiTietList = await _kiemKeService.GetChiTietKiemKeAsync(id);
			ViewBag.ChiTietKiemKe = chiTietList;

			return View(kiemKe);
		}
		[HttpGet]
		public IActionResult CreateKiemKe()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> CreateKiemKe(KiemKeDto newKiemKe)
		{
			if (ModelState.IsValid)
			{
				var success = await _kiemKeService.CreateKiemKeAsync(newKiemKe);
				if (success)
				{
					TempData["SuccessMessage"] = "Tạo phiếu kiểm kê thành công!";
					return RedirectToAction("Index");
				}
				TempData["ErrorMessage"] = "Không thể tạo phiếu kiểm kê.";
			}
			return View(newKiemKe);
		}
		[HttpGet]
		public async Task<IActionResult> EditKiemKe(int id)
		{
			var kiemKe = await _kiemKeService.GetKiemKeByIdAsync(id);
			if (kiemKe == null)
			{
				TempData["ErrorMessage"] = "Không tìm thấy phiếu kiểm kê.";
				return RedirectToAction("Index");
			}
			return View(kiemKe);
		}

		[HttpPost]
		public async Task<IActionResult> EditKiemKe(int id, KiemKeDto updatedKiemKe)
		{
			if (ModelState.IsValid)
			{
				var success = await _kiemKeService.UpdateKiemKeAsync(id, updatedKiemKe);
				if (success)
				{
					TempData["SuccessMessage"] = "Cập nhật phiếu kiểm kê thành công!";
					return RedirectToAction("Index");
				}
				TempData["ErrorMessage"] = "Không thể cập nhật phiếu kiểm kê.";
			}
			return View(updatedKiemKe);
		}
		[HttpGet]
		//public async Task<IActionResult> EditChiTietKiemKe(int kiemKeId, int sanPhamId)
		//{
		//	var chiTiet = await _kiemKeService.GetChiTietKiemKeByIdAsync(kiemKeId, sanPhamId);
		//	if (chiTiet == null)
		//	{
		//		TempData["ErrorMessage"] = "Không tìm thấy chi tiết kiểm kê.";
		//		return RedirectToAction("Details", new { id = kiemKeId });
		//	}
		//	return View(chiTiet);
		//}

		[HttpPost]
		public async Task<IActionResult> EditChiTietKiemKe(int kiemKeId, ChiTietKiemKeDto updatedChiTiet)
		{
			if (ModelState.IsValid)
			{
				var success = await _kiemKeService.UpdateChiTietKiemKeAsync(kiemKeId, updatedChiTiet);
				if (success)
				{
					TempData["SuccessMessage"] = "Cập nhật chi tiết kiểm kê thành công!";
					return RedirectToAction("Details", new { id = kiemKeId });
				}
				TempData["ErrorMessage"] = "Không thể cập nhật chi tiết kiểm kê.";
			}
			return View(updatedChiTiet);
		}
		[HttpPost]
		public async Task<IActionResult> DeleteKiemKe(int id)
		{
			var success = await _kiemKeService.DeleteKiemKeAsync(id);
			if (success)
			{
				TempData["SuccessMessage"] = "Xóa phiếu kiểm kê thành công!";
			}
			else
			{
				TempData["ErrorMessage"] = "Không thể xóa phiếu kiểm kê.";
			}
			return RedirectToAction("Index");
		}

	}

}
