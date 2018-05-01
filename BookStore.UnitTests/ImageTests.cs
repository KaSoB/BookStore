using BookStore.Domain.Abstract;
using BookStore.Domain.Entities;
using BookStore.WebUI.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Linq;
using System.Web.Mvc;

namespace BookStore.UnitTests {
    [TestClass]
    public class ImageTests {
        [TestMethod]
        public void CanRetrieveImageData() {

            Product book = new Product {
                ProductID = 2,
                Name = "Test",
                ImageData = new byte[] { },
                ImageMimeType = "image/png"
            };

            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] {
                new Product {ProductID = 1, Name = "P1"},
                book,
                new Product {ProductID = 3, Name = "P3"}
            }.AsQueryable());

            ProductController target = new ProductController(mock.Object);


            ActionResult result = target.GetImage(2);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(FileResult));
            Assert.AreEqual(book.ImageMimeType, ((FileResult) result).ContentType);
        }

        [TestMethod]
        public void CannotRetrieveImageDataForInvalidID() {
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] {
                new Product {ProductID = 1, Name = "P1"},
                new Product {ProductID = 2, Name = "P2"}
            }.AsQueryable());

            ProductController target = new ProductController(mock.Object);


            ActionResult result = target.GetImage(100);

            Assert.IsNull(result);
        }
    }
}
