using BookStore.Domain.Abstract;
using BookStore.Domain.Entities;
using BookStore.WebUI.Models;
using System.Linq;
using System.Web.Mvc;

namespace BookStore.WebUI.Controllers {
    public class CartController : Controller {
        private readonly IProductRepository repository;
        private readonly IOrderProcessor orderProcessor;

        public CartController(IProductRepository repository, IOrderProcessor proc) {
            this.repository = repository;
            this.orderProcessor = proc;
        }

        public RedirectToRouteResult AddToCart(Cart cart, int productId, string returnUrl) {
            Product product = repository.Products.FirstOrDefault(p => p.ProductID == productId);
            if (product != null) {
                cart.AddItem(product, 1);
            }
            return RedirectToAction("Index", new { returnUrl });
        }

        public RedirectToRouteResult RemoveFromCart(Cart cart, int productId, string returnUrl) {
            Product product = repository.Products.FirstOrDefault(p => p.ProductID == productId);
            if (product != null) {
                cart.RemoveLine(product);
            }
            return RedirectToAction("Index", new { returnUrl });
        }

        public ViewResult Index(Cart cart, string returnUrl) {
            return View(new CartIndexViewModel {
                Cart = cart,
                ReturnUrl = returnUrl
            });
        }
        public PartialViewResult Summary(Cart cart) {
            return PartialView(cart);
        }

        [HttpPost]
        public ViewResult Checkout(Cart cart, ShippingDetails shippingDetails) {
            if (cart.Lines.Count() == 0) {
                ModelState.AddModelError("", "Koszyk jest pusty!");
            }
            if (ModelState.IsValid) {
                orderProcessor.ProcessOrder(cart, shippingDetails);
                cart.Clear();
                return View("Completed");
            } else {
                return View(shippingDetails);
            }
        }
    }
}