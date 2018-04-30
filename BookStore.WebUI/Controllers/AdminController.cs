using BookStore.Domain.Abstract;
using BookStore.Domain.Entities;
using System.Linq;
using System.Web.Mvc;

namespace BookStore.WebUI.Controllers {
    public class AdminController : Controller {
        private readonly IProductRepository repository;

        public AdminController(IProductRepository repository) {
            this.repository = repository;
        }

        public ViewResult Index() {
            return View(repository.Products);
        }

        public ViewResult Edit(int productId) {
            Product product = repository.Products
                .FirstOrDefault(p => p.ProductID == productId);
            return View(product);
        }

        [HttpPost]
        public ActionResult Edit(Product product) {
            if (ModelState.IsValid) {
                repository.SaveProduct(product);
                TempData["message"] = string.Format("Zapisano {0} ", product.Name);
                return RedirectToAction("Index");
            } else {
                return View(product);
            }
        }
    }
}