//using Microsoft.AspNetCore.Mvc;
//using Ql_KhoHang.Dtos;
//using Ql_KhoHang.Services;
//using System.Security.Claims;

//namespace Ql_KhoHang.Controllers
//{
//    public class InventoryController : Controller
//    {
//        private readonly InventoryService _inventoryService;

//        public InventoryController(InventoryService inventoryService)
//        {
//            _inventoryService = inventoryService;
//        }

//        [HttpGet]
//        public async Task<IActionResult> Index(string? keyword, int pageNumber = 1, int pageSize = 10)
//        {
//            IEnumerable<KiemKeDto> allInventories;

//            if (!string.IsNullOrEmpty(keyword))
//            {
//                allInventories = await _inventoryService.SearchInventoriesAsync(keyword);
//                if (!allInventories.Any())
//                {
//                    TempData["ErrorMessage"] = "Không tìm thấy phiếu kiểm kê.";
//                }
//                else
//                {
//                    TempData["SuccessMessage"] = $"Tìm thấy {allInventories.Count()} kết quả.";
//                }
//            }
//            else
//            {
//                allInventories = await _inventoryService.GetAllInventoriesAsync();
//            }

//            var paginatedInventories = allInventories
//                .Skip((pageNumber - 1) * pageSize)
//                .Take(pageSize)
//                .ToList();

//            int totalPages = (int)Math.Ceiling(allInventories.Count() / (double)pageSize);

//            ViewBag.CurrentPage = pageNumber;
//            ViewBag.PageSize = pageSize;
//            ViewBag.TotalPages = totalPages;
//            ViewBag.Keyword = keyword;

//            return View(paginatedInventories);
//        }

//        [HttpGet]
//        public async Task<IActionResult> Details(int id)
//        {
//            var inventory = await _inventoryService.GetInventoryByIdAsync(id);
//            if (inventory == null)
//            {
//                TempData["ErrorMessage"] = "Không tìm thấy phiếu kiểm kê.";
//                return RedirectToAction("Index");
//            }

//            return View(inventory);
//        }

//        [HttpGet]
//        public IActionResult Create()
//        {
//            return View(new KiemKeDto());
//        }

//        [HttpPost]
//        public async Task<IActionResult> Create(KiemKeDto newInventory, List<ChiTietKiemKeDto> details, List<IFormFile> images)
//        {
//            if (ModelState.IsValid)
//            {
//                // Kết hợp chi tiết với ảnh tương ứng
//                for (int i = 0; i < details.Count; i++)
//                {
//                    if (i < images.Count && images[i] != null)
//                    {
//                        details[i].Img = images[i];
//                    }
//                }

//                var success = await _inventoryService.CreateInventoryAsync(newInventory, details);
//                if (success)
//                {
//                    TempData["SuccessMessage"] = "Thêm mới phiếu kiểm kê và chi tiết thành công!";
//                    return RedirectToAction("Index");
//                }
//                TempData["ErrorMessage"] = "Không thể thêm mới phiếu kiểm kê.";
//            }

//            return View(newInventory);
//        }


//        [HttpGet]
//        public async Task<IActionResult> Edit(int id)
//        {
//            var inventory = await _inventoryService.GetInventoryByIdAsync(id);
//            if (inventory == null)
//            {
//                TempData["ErrorMessage"] = "Không tìm thấy phiếu kiểm kê.";
//                return RedirectToAction("Index");
//            }

//            return View(inventory);
//        }

//        [HttpPost]
//        public async Task<IActionResult> Edit(int id, KiemKeDto updatedInventory, List<ChiTietKiemKeDto> details, List<IFormFile> images)
//        {
//            if (ModelState.IsValid)
//            {
//                // Kết hợp chi tiết với ảnh tương ứng
//                for (int i = 0; i < details.Count; i++)
//                {
//                    if (i < images.Count && images[i] != null)
//                    {
//                        details[i].Anh = images[i];
//                    }
//                }

//                var success = await _inventoryService.UpdateInventoryAsync(id, updatedInventory, details);
//                if (success)
//                {
//                    TempData["SuccessMessage"] = "Cập nhật phiếu kiểm kê thành công!";
//                    return RedirectToAction("Index");
//                }
//                TempData["ErrorMessage"] = "Không thể cập nhật phiếu kiểm kê.";
//            }

//            return View(updatedInventory);
//        }


//        [HttpPost]
//        public async Task<IActionResult> Delete(int id)
//        {
//            var success = await _inventoryService.DeleteInventoryAsync(id);
//            if (success)
//            {
//                TempData["SuccessMessage"] = "Xóa phiếu kiểm kê thành công!";
//            }
//            else
//            {
//                TempData["ErrorMessage"] = "Không thể xóa phiếu kiểm kê.";
//            }
//            return RedirectToAction("Index");
//        }
//    }
//}
