using BookStore.Domain.Abstract;
using BookStore.Domain.Entities;
using System.Collections.Generic;

namespace BookStore.Domain.Concrete {
    public class EFProductRepository : IProductRepository {
        private readonly EFDbContext context = new EFDbContext();

        public IEnumerable<Product> Products => context.Products;

        public void SaveProduct(Product product) {
            if (product.ProductID == 0) {
                context.Products.Add(product);
            } else {
                Product dbEntry = context.Products.Find(product.ProductID);
                if (dbEntry != null) {
                    dbEntry.Name = product.Name;
                    dbEntry.Description = product.Description;
                    dbEntry.Price = product.Price;
                    dbEntry.Category = product.Category;
                }
            }
            context.SaveChanges();
        }
    }
}
