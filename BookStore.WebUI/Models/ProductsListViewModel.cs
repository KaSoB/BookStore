using BookStore.Domain.Entities;
using System.Collections.Generic;

namespace BookStore.WebUI.Models {
    public class ProductsListViewModel {
        public IEnumerable<Product> Products { get; set; }
        public PagingInfo PagingInfo { get; set; }
    }
}