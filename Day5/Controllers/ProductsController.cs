using Microsoft.AspNetCore.Mvc;
using Day5.Models;

namespace Day5.Controllers
{
    public class ProductsController : Controller
    {
        private static List<Category> _categories = new List<Category>
        {
            new Category { CategoryId = 1, CategoryName = "Thời trang" },
            new Category { CategoryId = 2, CategoryName = "Phụ kiện" }
        };

        private static List<Product> _products = new List<Product>
        {
            new Product { ProductId = 1, ProductName = "Áo thun nam", ProductPrice = 123456, CategoryId = 1 },
            new Product { ProductId = 2, ProductName = "Quần jean nữ", ProductPrice = 234567, CategoryId = 1 },
            new Product { ProductId = 3, ProductName = "Giày thể thao", ProductPrice = 345678, CategoryId = 2 },
            new Product { ProductId = 4, ProductName = "Mũ lưỡi trai", ProductPrice = 363636, CategoryId = 2 },
            new Product { ProductId = 5, ProductName = "Áo khoác gió", ProductPrice = 360000, CategoryId = 1 },
            new Product { ProductId = 6, ProductName = "Túi đeo chéo", ProductPrice = 3600000, CategoryId = 2 }
        };

        public IActionResult Index(string searchString, int page = 1)
        {
            int pageSize = 3;
            ViewBag.CurrentFilter = searchString;

            // Chuyển từ Product sang ProductViewModel để khớp với View
            var query = from p in _products
                        join c in _categories on p.CategoryId equals c.CategoryId
                        select new ProductViewModel
                        {
                            ProductId = p.ProductId,
                            ProductName = p.ProductName,
                            ProductPrice = p.ProductPrice,
                            CategoryName = c.CategoryName
                        };

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(x => x.ProductName.Contains(searchString, StringComparison.OrdinalIgnoreCase)
                                      || x.CategoryName.Contains(searchString, StringComparison.OrdinalIgnoreCase));
            }

            int totalItems = query.Count();
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            var pagedData = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            return View(pagedData);
        }
    }
}