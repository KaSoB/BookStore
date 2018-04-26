using BookStore.Domain.Abstract;
using BookStore.Domain.Entities;
using Moq;
using Ninject;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace BookStore.WebUI.Infrastructure {
    public class NinjectDependencyResolver : IDependencyResolver {

        private IKernel kernel;

        public NinjectDependencyResolver(IKernel kernel) {
            this.kernel = kernel;
            AddBindings();
        }

        public object GetService(Type serviceType) {
            return kernel.TryGet(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType) {
            return kernel.GetAll(serviceType);
        }

        private void AddBindings() {
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new List<Product> {
                new Product {Name="Name1", Price=10 },
                new Product {Name="Name2", Price=20 },
                new Product {Name="Name3", Price=30 }
            });
            kernel.Bind<IProductRepository>().ToConstant(mock.Object);
        }

    }
}