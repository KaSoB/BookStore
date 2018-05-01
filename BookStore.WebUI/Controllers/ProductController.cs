using BookStore.Domain.Abstract;
using BookStore.Domain.Entities;
using BookStore.WebUI.Models;
using System.Linq;
using System.Web.Mvc;

namespace BookStore.WebUI.Controllers {
    public class ProductController : Controller {
        private readonly IProductRepository repository;
        public int PageSize = 4;

        public ProductController(IProductRepository repository) {
            this.repository = repository;
        }
        // GET: Product
        public ViewResult List(string category, int page = 1) {
            ProductsListViewModel model = new ProductsListViewModel {
                Products = repository.Products
                .Where(p => category == null || p.Category == category)
                .OrderBy(p => p.ProductID)
                .Skip((page - 1) * PageSize)
                .Take(PageSize),
                PagingInfo = new PagingInfo {
                    CurrentPage = page,
                    ItemsPerPage = PageSize,
                    TotalItems = category == null ? repository.Products.Count() :
                    repository.Products.Where(e => e.Category == category).Count()
                },
                CurrentCategory = category
            };
            return View(model);
        }

        public FileContentResult GetImage(int productId) {
            Product prod = repository.Products.FirstOrDefault(p => p.ProductID == productId);
            if (prod != null) {
                return File(prod.ImageData, prod.ImageMimeType);
            } else {
                return null;
            }
        }

    }
}