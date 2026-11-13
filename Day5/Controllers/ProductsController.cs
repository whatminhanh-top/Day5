using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
            new Product { ProductId = 1, ProductName = "Áo thun nam", ProductPrice = 150000, CategoryId = 1 },
            new Product { ProductId = 2, ProductName = "Quần jean nữ", ProductPrice = 350000, CategoryId = 1 },
            new Product { ProductId = 3, ProductName = "Giày thể thao", ProductPrice = 550000, CategoryId = 2 },
            new Product { ProductId = 4, ProductName = "Mũ lưỡi trai", ProductPrice = 90000, CategoryId = 2 },
            new Product { ProductId = 5, ProductName = "Áo khoác gió", ProductPrice = 420000, CategoryId = 1 },
            new Product { ProductId = 6, ProductName = "Túi đeo chéo", ProductPrice = 210000, CategoryId = 2 }
        };

        // 1. DANH SÁCH + TÌM KIẾM + PHÂN TRANG
        public IActionResult Index(string searchString, int page = 1)
        {
            int pageSize = 3;
            ViewBag.CurrentFilter = searchString;

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

        // 2. THÊM MỚI (GET)
        public IActionResult Create()
        {
            ViewBag.Categories = new SelectList(_categories, "CategoryId", "CategoryName");
            return View();
        }

        // 2. THÊM MỚI (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Product product)
        {
            // Bỏ qua kiểm tra navigation property Category
            ModelState.Remove("Category");

            if (ModelState.IsValid)
            {
                // Tự động tăng ProductId
                product.ProductId = _products.Any() ? _products.Max(p => p.ProductId) + 1 : 1;
                _products.Add(product);
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Categories = new SelectList(_categories, "CategoryId", "CategoryName", product.CategoryId);
            return View(product);
        }

        // 3. SỬA (GET)
        public IActionResult Edit(int id)
        {
            var product = _products.FirstOrDefault(p => p.ProductId == id);
            if (product == null) return NotFound();

            ViewBag.Categories = new SelectList(_categories, "CategoryId", "CategoryName", product.CategoryId);
            return View(product);
        }

        // 3. SỬA (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Product product)
        {
            if (id != product.ProductId) return NotFound();

            ModelState.Remove("Category");

            if (ModelState.IsValid)
            {
                var existing = _products.FirstOrDefault(p => p.ProductId == id);
                if (existing != null)
                {
                    existing.ProductName = product.ProductName;
                    existing.ProductPrice = product.ProductPrice;
                    existing.CategoryId = product.CategoryId;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Categories = new SelectList(_categories, "CategoryId", "CategoryName", product.CategoryId);
            return View(product);
        }

        // 4. XÓA
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var product = _products.FirstOrDefault(p => p.ProductId == id);
            if (product != null)
            {
                _products.Remove(product);
            }
            return RedirectToAction(nameof(Index));
        }
    }
}