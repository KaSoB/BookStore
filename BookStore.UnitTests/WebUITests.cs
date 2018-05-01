using BookStore.Domain.Abstract;
using BookStore.Domain.Entities;
using BookStore.WebUI.Controllers;
using BookStore.WebUI.HtmlHelpers;
using BookStore.WebUI.Infrastructure.Abstract;
using BookStore.WebUI.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
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
        [TestMethod]
        public void CanCreateCategories() {
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
            NavController target = new NavController(mock.Object);

            string[] results = ((IEnumerable<string>) target.Menu().Model).ToArray();

            //Assert
            Assert.AreEqual(results.Length, 3);
            Assert.AreEqual(results[0], "Cat1");
            Assert.AreEqual(results[1], "Cat2");
            Assert.AreEqual(results[2], "Cat3");

        }

        [TestMethod]
        public void IndicatesSelectedCategory() {
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new[] {
                new Product{ProductID=1, Name="P1", Category="Cat1" },
                new Product{ProductID=4, Name="P2", Category="Cat2" }
            });
            NavController target = new NavController(mock.Object);

            string categoryToSelect = "Cat1";

            string result = target.Menu(categoryToSelect).ViewBag.SelectedCategory;

            Assert.AreEqual(categoryToSelect, result);
        }

        [TestMethod]
        public void GenerateCategorySpecificProductCount() {
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new[] {
                   new Product {ProductID = 1, Name = "P1", Category = "Cat1"},
                    new Product {ProductID = 2, Name = "P2", Category = "Cat2"},
                    new Product {ProductID = 3, Name = "P3", Category = "Cat3"},
                    new Product {ProductID = 4, Name = "P4", Category = "Cat2"},
                    new Product {ProductID = 5, Name = "P5", Category = "Cat1"}
            });
            ProductController target = new ProductController(mock.Object) {
                PageSize = 3
            };

            int res1 = (target.List("Cat1").Model as ProductsListViewModel).PagingInfo.TotalItems;
            int res2 = (target.List("Cat2").Model as ProductsListViewModel).PagingInfo.TotalItems;
            int res3 = (target.List("Cat3").Model as ProductsListViewModel).PagingInfo.TotalItems;
            int resALL = (target.List(null).Model as ProductsListViewModel).PagingInfo.TotalItems;


            Assert.AreEqual(res1, 2);
            Assert.AreEqual(res2, 2);
            Assert.AreEqual(res3, 1);
            Assert.AreEqual(resALL, 5);
        }

        [TestMethod]
        public void CanSaveValidChanges() {
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            AdminController target = new AdminController(mock.Object);
            Product product = new Product { Name = "Test" };

            ActionResult result = target.Edit(product);

            mock.Verify(m => m.SaveProduct(product));

            Assert.IsNotInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public void CannotSaveIndalidChanges() {
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            AdminController target = new AdminController(mock.Object);
            Product product = new Product { Name = "Test" };

            target.ModelState.AddModelError("error", "error");

            ActionResult result = target.Edit(product);

            mock.Verify(m => m.SaveProduct(It.IsAny<Product>()), Times.Never());

            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public void CanDeleteValidProducts() {
            Product prod = new Product { ProductID = 2, Name = "Test" };
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] {
                new Product { ProductID= 1, Name="P1"},
                prod,
                new Product {ProductID = 3, Name="P3"}
            });

            AdminController target = new AdminController(mock.Object);

            target.Delete(prod.ProductID);


            mock.Verify(m => m.DeleteProduct(prod.ProductID));
        }

        [TestMethod]
        public void CanLoginWithValidCredentials() {

            Mock<IAuthProvider> mock = new Mock<IAuthProvider>();
            mock.Setup(m => m.Authenticate("admin", "secret")).Returns(true);


            LoginViewModel model = new LoginViewModel() {
                UserName = "admin",
                Password = "secret"
            };

            AccountController target = new AccountController(mock.Object);

            ActionResult result = target.Login(model, "/MyURL");

            Assert.IsInstanceOfType(result, typeof(RedirectResult));
            Assert.AreEqual("/MyURL", ((RedirectResult) result).Url);
        }

        [TestMethod]
        public void CannotLoginWithInvalidCredentials() {

            Mock<IAuthProvider> mock = new Mock<IAuthProvider>();
            mock.Setup(m => m.Authenticate("user", "invalidPassword")).Returns(false);


            LoginViewModel model = new LoginViewModel() {
                UserName = "user",
                Password = "invalidPassword"
            };

            AccountController target = new AccountController(mock.Object);

            ActionResult result = target.Login(model, "/MyURL");

            Assert.IsInstanceOfType(result, typeof(ViewResult));
            Assert.IsFalse(((ViewResult) result).ViewData.ModelState.IsValid);
        }
    }
}
