using BookStore.Domain.Abstract;
using BookStore.Domain.Entities;
using BookStore.WebUI.Controllers;
using BookStore.WebUI.HtmlHelpers;
using BookStore.WebUI.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Linq;
using System.Web.Mvc;

namespace BookStore.UnitTests {
    [TestClass]
    public class WebUITests {
        [TestMethod]
        public void CanPaginate() {
            // Arrange
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new[] {
                new Product() {ProductID = 1, Name = "P1"},
                new Product() {ProductID = 2, Name = "P2"},
                new Product() {ProductID = 3, Name = "P3"},
                new Product() {ProductID = 4, Name = "P4"},
                new Product() {ProductID = 5, Name = "P5"},
            });
            ProductController controller = new ProductController(mock.Object) {
                PageSize = 3
            };

            // Act
            ProductsListViewModel result = controller.List(null, 2).Model as ProductsListViewModel;


            // Assert
            Product[] prodArray = result.Products.ToArray();
            Assert.IsTrue(prodArray.Length == 2);
            Assert.AreEqual(prodArray[0].Name, "P4");
            Assert.AreEqual(prodArray[1].Name, "P5");
        }
        [TestMethod]
        public void CanGeneratePageLinks() {
            // Arrange
            HtmlHelper htmlHelper = null;

            //Act
            PagingInfo pagingInfo = new PagingInfo {
                CurrentPage = 2,
                TotalItems = 28,
                ItemsPerPage = 10
            };

            Func<int, string> pageUrlDelegate = i => $"Strona{i}";


            MvcHtmlString result = htmlHelper.PageLinks(pagingInfo, pageUrlDelegate);
            //Assert
            Assert.AreEqual(
                @"<a class=""btn btn-default"" href=""Strona1"">1</a>" +
                @"<a class=""btn btn-default btn-primary selected"" href=""Strona2"">2</a>" +
                @"<a class=""btn btn-default"" href=""Strona3"">3</a>", result.ToString());
        }
        [TestMethod]
        public void CanSendPaginationViewModel() {
            //przygotowanie
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products)
                .Returns(new[] {
                    new Product {ProductID = 1, Name = "P1"},
                    new Product {ProductID = 2, Name = "P2"},
                    new Product {ProductID = 3, Name = "P3"},
                    new Product {ProductID = 4, Name = "P4"},
                    new Product {ProductID = 5, Name = "P5"}
                });

            ProductController controller = new ProductController(mock.Object) {
                PageSize = 3
            };
            //// działanie
            ProductsListViewModel result = controller.List(null, 2).Model as ProductsListViewModel;

            // asercje 
            PagingInfo pageInfo = result.PagingInfo;
            Assert.AreEqual(pageInfo.CurrentPage, 2);
            Assert.AreEqual(pageInfo.ItemsPerPage, 3);
            Assert.AreEqual(pageInfo.TotalItems, 5);
            Assert.AreEqual(pageInfo.TotalPages, 2);
        }

        [TestMethod]
        public void CanFilterProducts() {
            //Arrange
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products)
                .Returns(new[] {
                    new Product {ProductID = 1, Name = "P1", Category = "Cat1"},
                    new Product {ProductID = 2, Name = "P2", Category = "Cat2"},
                    new Product {ProductID = 3, Name = "P3", Category = "Cat3"},
                    new Product {ProductID = 4, Name = "P4", Category = "Cat2"},
                    new Product {ProductID = 5, Name = "P5", Category = "Cat1"}
                });
            //Act
            ProductController controller = new ProductController(mock.Object) {
                PageSize = 3
            };

            //Assert
            Product[] result = (controller.List("Cat2", 1).Model as ProductsListViewModel).Products.ToArray();

            Assert.AreEqual(result.Length, 2);
            Assert.IsTrue(result[0].Name == "P2" && result[0].Category == "Cat2");
            Assert.IsTrue(result[1].Name == "P4" && result[1].Category == "Cat2");
        }

    }
}
