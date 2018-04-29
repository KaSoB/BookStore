using BookStore.Domain.Abstract;
using BookStore.Domain.Entities;
using BookStore.WebUI.Controllers;
using BookStore.WebUI.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Linq;
using System.Web.Mvc;

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
        [TestMethod]
        public void CanAddToCart() {
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] {
                new Product {ProductID = 1, Name = "P1", Category = "AB"}
            }.AsQueryable());

            Cart cart = new Cart();

            CartController target = new CartController(mock.Object, null);

            target.AddToCart(cart, 1, null);

            Assert.AreEqual(cart.Lines.Count(), 1);
            Assert.AreEqual(cart.Lines.ToArray()[0].Product.ProductID, 1);
        }
        [TestMethod]
        public void AddingProductToCartGoesToCartScreen() {
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] {
                new Product {ProductID = 1, Name = "P1", Category = "AB"}
            }.AsQueryable());

            Cart cart = new Cart();

            CartController target = new CartController(mock.Object, null);

            RedirectToRouteResult result = target.AddToCart(cart, 2, "myUrl");

            Assert.AreEqual(result.RouteValues["action"], "Index");
            Assert.AreEqual(result.RouteValues["returnURL"], "myUrl");

        }
        [TestMethod]
        public void CanViewCartContents() {
            Cart cart = new Cart();

            CartController target = new CartController(null, null);

            CartIndexViewModel result = (CartIndexViewModel) target.Index(cart, "myUrl").ViewData.Model;

            Assert.AreEqual(result.Cart, cart);
            Assert.AreEqual(result.ReturnUrl, "myUrl");
        }

        [TestMethod]
        public void CannotCheckoutEmptyCart() {
            Mock<IOrderProcessor> mock = new Mock<IOrderProcessor>();
            Cart cart = new Cart();
            ShippingDetails shippingDetails = new ShippingDetails();

            CartController target = new CartController(null, mock.Object);

            ViewResult result = target.Checkout(cart, shippingDetails);

            mock.Verify(m => m.ProcessOrder(It.IsAny<Cart>(), It.IsAny<ShippingDetails>()), Times.Never);

            Assert.AreEqual("", result.ViewName);
            Assert.AreEqual(false, result.ViewData.ModelState.IsValid);
        }

        [TestMethod]
        public void CannotCheckoutInvalidShippingDetails() {

            // przygotowanie - tworzenie imitacji procesora zamówień
            Mock<IOrderProcessor> mock = new Mock<IOrderProcessor>();
            // przygotowanie - tworzenie koszyka z produktem
            Cart cart = new Cart();
            cart.AddItem(new Product(), 1);

            // przygotowanie - tworzenie egzemplarza kontrolera
            CartController target = new CartController(null, mock.Object);
            // przygotowanie - dodanie błędu do modelu
            target.ModelState.AddModelError("error", "error");

            // działanie - próba zakończenia zamówienia
            ViewResult result = target.Checkout(cart, new ShippingDetails());

            // asercje - sprawdzenie, czy zamówienie zostało przekazane do procesora
            mock.Verify(m => m.ProcessOrder(It.IsAny<Cart>(), It.IsAny<ShippingDetails>()),
                Times.Never());
            // asercje - sprawdzenie, czy metoda zwraca domyślny widok
            Assert.AreEqual("", result.ViewName);
            // asercje - sprawdzenie, czy przekazujemy prawidłowy model do widoku
            Assert.AreEqual(false, result.ViewData.ModelState.IsValid);
        }
        [TestMethod]
        public void CanCheckoutAndSubmitOrder() {
            // przygotowanie - tworzenie imitacji procesora zamówień
            Mock<IOrderProcessor> mock = new Mock<IOrderProcessor>();
            // przygotowanie - tworzenie koszyka z produktem
            Cart cart = new Cart();
            cart.AddItem(new Product(), 1);
            // przygotowanie - tworzenie egzemplarza kontrolera
            CartController target = new CartController(null, mock.Object);

            // działanie - próba zakończenia zamówienia
            ViewResult result = target.Checkout(cart, new ShippingDetails());

            // asercje - sprawdzenie, czy zamówienie nie zostało przekazane do procesora
            mock.Verify(m => m.ProcessOrder(It.IsAny<Cart>(), It.IsAny<ShippingDetails>()),
                Times.Once());
            // asercje - sprawdzenie, czy metoda zwraca widok Completed
            Assert.AreEqual("Completed", result.ViewName);
            // asercje - sprawdzenie, czy przekazujemy prawidłowy model do widoku
            Assert.AreEqual(true, result.ViewData.ModelState.IsValid);
        }
    }
}
