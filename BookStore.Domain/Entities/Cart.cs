using System.Collections.Generic;
using System.Linq;

namespace BookStore.Domain.Entities {
    public class Cart {
        private List<CartLine> lineCollection = new List<CartLine>();

        public void AddItem(Product product, int quantity) {
            CartLine line = lineCollection
                .Where(p => p.Product.ProductID == product.ProductID)
                .FirstOrDefault();

            if (line == null) {
                lineCollection.Add(new CartLine { Product = product, Quantity = quantity });
            } else {
                line.Quantity += quantity;
            }
        }
        public void Clear() {
            lineCollection.Clear();
        }
        public void RemoveLine(Product product) {
            lineCollection.RemoveAll(e => e.Product.ProductID == product.ProductID);
        }
        public decimal ComputeTotalValue() {
            return lineCollection.Sum(e => e.Product.Price * e.Quantity);
        }
        public IEnumerable<CartLine> Lines {
            get { return lineCollection; }
        }
    }
}
