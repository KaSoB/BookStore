using BookStore.Domain.Abstract;
using BookStore.Domain.Entities;
using System.Collections.Generic;

namespace BookStore.Domain.Concrete {
    public class EFProductRepository : IProductRepository {
        private readonly EFDbContext context = new EFDbContext();

        public IEnumerable<Product> Products => context.Products;

    }
}
