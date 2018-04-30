using BookStore.Domain.Entities;
using System.Collections.Generic;

namespace BookStore.Domain.Abstract {
    public interface IProductRepository {
        IEnumerable<Product> Products { get; }
        void SaveProduct(Product product);
    }
}
