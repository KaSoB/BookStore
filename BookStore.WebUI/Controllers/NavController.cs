using BookStore.Domain.Abstract;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace BookStore.WebUI.Controllers {
    public class NavController : Controller {
        private readonly IProductRepository repository;

        public NavController(IProductRepository repository) {
            this.repository = repository;
        }

        public PartialViewResult Menu(string category = null) {
            ViewBag.SelectedCategory = category;
            IEnumerable<string> categories = repository.Products
                .Select(x => x.Category)
                .Distinct()
                .OrderBy(x => x);

            return PartialView("FlexMenu", categories);
        }
    }
}