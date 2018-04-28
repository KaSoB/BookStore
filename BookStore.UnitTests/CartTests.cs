using BookStore.Domain.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace BookStore.UnitTests {
    [TestClass]
    public class CartTests {
        [TestMethod]
        public void CanAddNewLines() {
            // Arrange
            var p1 = new Product() { ProductID = 1, Name = "P1" };
            var p2 = new Product() { ProductID = 2, Name = "P2" };

            Cart target = new Cart();

            // Act
            target.AddItem(p1, 1);
            target.AddItem(p2, 1);
            CartLine[] cartLines = target.Lines.ToArray();

            // Assert
            Assert.IsTrue(cartLines.Length == 2);
            Assert.AreEqual(cartLines[0].Product, p1);
            Assert.AreEqual(cartLines[1].Product, p2);
        }
        [TestMethod]
        public void CanAddQuantityForExistingLines() {
            // przygotowanie — tworzenie produktów testowych   
            var p1 = new Product() { ProductID = 1, Name = "P1" };
            var p2 = new Product() { ProductID = 2, Name = "P2" };
            // przygotowanie — utworzenie nowego koszyka   
            Cart target = new Cart();
            // działanie   
            target.AddItem(p1, 1);
            target.AddItem(p2, 1);
            target.AddItem(p1, 10);
            CartLine[] results = target.Lines.OrderBy(c => c.Product.ProductID).ToArray();
            // asercje   
            Assert.AreEqual(results.Length, 2);
            Assert.AreEqual(results[0].Quantity, 11);
            Assert.AreEqual(results[1].Quantity, 1);
        }
        [TestMethod]
        public void CanRemoveLine() {
            // przygotowanie — tworzenie produktów testowych   
            var p1 = new Product() { ProductID = 1, Name = "P1" };
            var p2 = new Product() { ProductID = 2, Name = "P2" };
            var p3 = new Product() { ProductID = 3, Name = "P3" };
            // przygotowanie — utworzenie nowego koszyka   
            Cart target = new Cart();
            // przygotowanie — dodanie kilku produktów do koszyka   
            target.AddItem(p1, 1);
            target.AddItem(p2, 3);
            target.AddItem(p3, 5);
            target.AddItem(p2, 1);
            // działanie   
            target.RemoveLine(p2);
            // asercje   
            Assert.AreEqual(target.Lines.Where(c => c.Product == p2).Count(), 0);
            Assert.AreEqual(target.Lines.Count(), 2);
        }
        [TestMethod]
        public void CalculateCartTotal() {
            // przygotowanie — tworzenie produktów testowych   
            var p1 = new Product { ProductID = 1, Name = "P1", Price = 100M };
            var p2 = new Product { ProductID = 2, Name = "P2", Price = 50M };
            // przygotowanie — utworzenie nowego koszyka   
            Cart target = new Cart();
            // działanie   
            target.AddItem(p1, 1);
            target.AddItem(p2, 1);
            target.AddItem(p1, 3);
            decimal result = target.ComputeTotalValue();
            // asercje 
            Assert.AreEqual(result, 450M);
        }
        [TestMethod]
        public void CanClearContents() {
            // przygotowanie — tworzenie produktów testowych   
            var p1 = new Product { ProductID = 1, Name = "P1", Price = 100M };
            var p2 = new Product { ProductID = 2, Name = "P2", Price = 50M };
            // przygotowanie — utworzenie nowego koszyka   
            Cart target = new Cart();
            // przygotowanie — dodanie kilku produktów do koszyka   
            target.AddItem(p1, 1);
            target.AddItem(p2, 1);
            // działanie — czyszczenie koszyka   
            target.Clear();
            // asercje   
            Assert.AreEqual(target.Lines.Count(), 0);
        }
    }
}
