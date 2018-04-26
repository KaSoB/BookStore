using BookStore.Domain.Abstract;
using System.Web.Mvc;

namespace BookStore.WebUI.Controllers {
    public class ProductController : Controller {
        IProductRepository productRepository;

        public ProductController(IProductRepository productRepository) {
            this.productRepository = productRepository;
        }
        // GET: Product
        public ActionResult List() => View(productRepository.Products);
    }
}