using Microsoft.AspNetCore.Mvc;
using Ql_KhoHang.Dtos;
using Ql_KhoHang.Services;
using System.Security.Claims;

namespace Ql_KhoHang.Controllers
{
	public class NhanVienKhoController : Controller
	{
		private readonly NhanVienKhoService _nhanVienKhoService;

		public NhanVienKhoController(NhanVienKhoService nhanVienKhoService)
		{
			_nhanVienKhoService = nhanVienKhoService;
		}

		[HttpGet]
		public async Task<IActionResult> Index(string keyword, int pageNumber = 1, int pageSize = 9)
		{
			SetUserClaims();
			var allEmployees = await _nhanVienKhoService.GetAllAsync(keyword);

			// Tính toán dữ liệu phân trang
			var paginatedEmployees = allEmployees
				.Skip((pageNumber - 1) * pageSize)
				.Take(pageSize)
				.ToList();

			// Tính tổng số trang
			int totalPages = (int)Math.Ceiling(allEmployees.Count() / (double)pageSize);

			// Gửi thông tin phân trang và từ khóa vào View
			ViewBag.CurrentPage = pageNumber;
			ViewBag.PageSize = pageSize;
			ViewBag.TotalPages = totalPages;
			ViewBag.Keyword = keyword; // Để giữ lại từ khóa tìm kiếm

			return View(paginatedEmployees);
		}

		[HttpGet]
		public async Task<IActionResult> Details(int id)
		{
			SetUserClaims();
			var employee = await _nhanVienKhoService.GetByIdAsync(id);
			if (employee == null)
			{
				TempData["ErrorMessage"] = "Không tìm thấy nhân viên kho.";
				return RedirectToAction("Index");
			}
			return View(employee);
		}

		[HttpGet]
		public IActionResult Create()
		{
			SetUserClaims();
			return View(new NhanVienKhoDto());
		}

		[HttpPost]
		public async Task<IActionResult> Create(NhanVienKhoDto newEmployee, IFormFile Img)
		{
			if (ModelState.IsValid)
			{
				var success = await _nhanVienKhoService.CreateAsync(newEmployee, Img);
				if (success)
				{
					TempData["SuccessMessage"] = "Thêm mới nhân viên kho thành công!";
					return RedirectToAction("Index");
				}
				TempData["ErrorMessage"] = "Không thể thêm mới nhân viên kho.";
			}
			return View(newEmployee);
		}

		[HttpGet]
		public async Task<IActionResult> Edit(int id)
		{
			SetUserClaims();
			var employee = await _nhanVienKhoService.GetByIdAsync(id);
			if (employee == null)
			{
				TempData["ErrorMessage"] = "Không tìm thấy nhân viên kho.";
				return RedirectToAction("Index");
			}
			return View(employee);
		}

		[HttpPost]
		public async Task<IActionResult> Edit(int id, NhanVienKhoDto updatedEmployee, IFormFile Img)
		{
			if (ModelState.IsValid)
			{
				var success = await _nhanVienKhoService.UpdateAsync(id, updatedEmployee, Img);
				if (success)
				{
					TempData["SuccessMessage"] = "Cập nhật nhân viên kho thành công!";
					return RedirectToAction("Index");
				}
				TempData["ErrorMessage"] = "Không thể cập nhật nhân viên kho.";
			}
			return View(updatedEmployee);
		}

		[HttpPost]
		public async Task<IActionResult> Delete(int id)
		{
			var success = await _nhanVienKhoService.DeleteAsync(id);
			if (success)
			{
				TempData["SuccessMessage"] = "Xóa nhân viên kho thành công!";
			}
			else
			{
				TempData["ErrorMessage"] = "Không thể xóa nhân viên kho.";
			}
			return RedirectToAction("Index");
		}
		public async Task<IActionResult> _MenuPartial()
		{
			return PartialView();
		}

		public async Task<IActionResult> _SidebarPartial()
		{
			return PartialView();
		}
		private void SetUserClaims()
		{
			ViewBag.Username = User.Identity?.Name;
			ViewBag.Role = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            ViewBag.HinhAnh = User.Claims.FirstOrDefault(c => c.Type == "HinhAnh")?.Value;
        }
	}
}
