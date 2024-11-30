using Microsoft.AspNetCore.Mvc;
using Ql_KhoHang.Dtos;
using Ql_KhoHang.Services;
using System.Security.Claims;

namespace Ql_KhoHang.Controllers
{
    public class BlogController : Controller
    {
        private readonly BlogService _blogService;

        public BlogController(BlogService blogService)
        {
            _blogService = blogService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? keyword, int pageNumber = 1, int pageSize = 10)
        {
            SetUserClaims();
            IEnumerable<BlogDto> allBlogs;

            if (!string.IsNullOrEmpty(keyword))
            {
                allBlogs = await _blogService.SearchAsync(keyword);
                if (!allBlogs.Any())
                {
                    TempData["ErrorMessage"] = "Không tìm thấy blog nào.";
                }
                else
                {
                    TempData["SuccessMessage"] = $"Tìm thấy {allBlogs.Count()} kết quả.";
                }
            }
            else
            {
                allBlogs = await _blogService.GetAllAsync();
            }

            // Tính toán dữ liệu phân trang
            var paginatedBlogs = allBlogs
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Tính tổng số trang
            int totalPages = (int)Math.Ceiling(allBlogs.Count() / (double)pageSize);

            // Gửi thông tin phân trang tới View
            ViewBag.CurrentPage = pageNumber;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = totalPages;
            ViewBag.Keyword = keyword; // Giữ từ khóa tìm kiếm

            return View(paginatedBlogs);
        }


        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            SetUserClaims();
            var blog = await _blogService.GetByIdAsync(id);
            if (blog == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy blog.";
                return RedirectToAction("Index");
            }
            return View(blog);
        }

        [HttpGet]
        public IActionResult Create()
        {
            SetUserClaims();
            return View(new BlogDto());
        }

        [HttpPost]
        public async Task<IActionResult> Create(BlogDto newBlog, IFormFile Img)
        {
            // Lấy thông tin người dùng từ Claims
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "MaNguoiDung")?.Value;

            if (int.TryParse(userIdClaim, out var userId))
            {
                newBlog.MaNguoiDung = userId;
            }
            else
            {
                TempData["ErrorMessage"] = "Không thể lấy thông tin người dùng.";
                return RedirectToAction("Index");
            }

            if (ModelState.IsValid)
            {
                var success = await _blogService.CreateAsync(newBlog, Img);
                if (success)
                {
                    TempData["SuccessMessage"] = "Thêm mới blog thành công!";
                    return RedirectToAction("Index");
                }
                TempData["ErrorMessage"] = "Không thể thêm mới blog.";
            }

            return View(newBlog);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            SetUserClaims();
            var blog = await _blogService.GetByIdAsync(id);
            if (blog == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy blog.";
                return RedirectToAction("Index");
            }
            return View(blog);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, BlogDto updatedBlog, IFormFile Img)
        {
            if (ModelState.IsValid)
            {
                var success = await _blogService.UpdateAsync(id, updatedBlog, Img);
                if (success)
                {
                    TempData["SuccessMessage"] = "Cập nhật blog thành công!";
                    return RedirectToAction("Index");
                }
                TempData["ErrorMessage"] = "Không thể cập nhật blog.";
            }
            return View(updatedBlog);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _blogService.DeleteAsync(id);
            if (success)
            {
                TempData["SuccessMessage"] = "Xóa blog thành công!";
            }
            else
            {
                TempData["ErrorMessage"] = "Không thể xóa blog.";
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Search(string keyword)
        {
            var blogs = await _blogService.SearchAsync(keyword);

            if (blogs == null || !blogs.Any())
            {
                TempData["ErrorMessage"] = "Không tìm thấy blog nào.";
            }
            else
            {
                TempData["SuccessMessage"] = $"Tìm thấy {blogs.Count()} kết quả.";
            }

            return View("Index", blogs);
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
