using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;
using SportsStore.WebUI.Controllers;
using System.Collections.Generic;
using System.Linq;
using SportsStore.WebUI.Models;

namespace SportsStore.UnitTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            //arrange
            Mock<IProductsRepository> mock = new Mock<IProductsRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] {
                new Product {ProductID = 1, Name = "P1"},
                new Product {ProductID = 2, Name = "P2"},
                new Product {ProductID = 3, Name = "P3"},
                new Product {ProductID = 4, Name = "P4"},
                new Product {ProductID = 5, Name = "P5"}
            });

            ProductController controller = new ProductController(mock.Object);
            controller.PageSize = 3;

            //Act
            ProductListViewModel result = (ProductListViewModel)controller.List(null,2).Model;

            //Assert
            Product[] prodArray = result.Products.ToArray();
            Assert.IsTrue(prodArray.Length == 2);
            Assert.AreEqual(prodArray[0].Name, "P4");
            Assert.AreEqual(prodArray[1].Name, "P5");

        }

        [TestMethod]
        public void Can_Filter_Products()
        {
            //Arrange
            //Create mock repository
            Mock<IProductsRepository> mock = new Mock<IProductsRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] {
                new Product {ProductID = 1, Name = "P1", Category = "Cat1"},
                new Product {ProductID = 2, Name = "P2", Category = "Cat2"},
                new Product {ProductID = 3, Name = "P3", Category = "Cat1"},
                new Product {ProductID = 4, Name = "P4", Category = "Cat2"},
                new Product {ProductID = 5, Name = "P5", Category = "Cat3"}
            });

            //Create controller to process the product
            ProductController controller = new ProductController(mock.Object);
            controller.PageSize = 3;

            //Action
            Product[] result = ((ProductListViewModel)controller.List("Cat2", 1).Model).Products.ToArray();

            //Assert
            Assert.AreEqual("P2", result[0].Name);
            Assert.AreEqual("P4", result[1].Name);
        }

        [TestMethod]
        public void Can_Create_Categories()
        {
            //Arrange
            //Create mock repository
            Mock<IProductsRepository> mock = new Mock<IProductsRepository>();
            mock.Setup(r => r.Products).Returns(new Product[] {
                new Product {ProductID = 1, Name = "P1", Category = "Apples"},
                new Product {ProductID = 2, Name = "P2", Category = "Apples"},
                new Product {ProductID = 3, Name = "P3", Category = "Plums"},
                new Product {ProductID = 4, Name = "P4", Category = "Oranges"},
            });
            NavController controller = new NavController(mock.Object);

            //Act
            string[] results = ((IEnumerable<string>)controller.Menu(null).Model).ToArray();

            //Assert
            Assert.AreEqual(results[0], "Apples");
            Assert.AreEqual(results[1], "Oranges");
            Assert.AreEqual(results[2], "Plums");
        }

        [TestMethod]
        public void Indicates_Selected_Category()
        {
            //Arrange
            Mock<IProductsRepository> repository = new Mock<IProductsRepository>();
            repository.Setup(r => r.Products).Returns(new Product[] {
                new Product {ProductID = 1, Name = "P1", Category = "Apples"},
                new Product {ProductID = 4, Name = "P2", Category = "Oranges"},
            });
            NavController controller = new NavController(repository.Object);

            //Arrage
            string selected = "Apples";

            //Action
            string result = controller.Menu(selected).ViewBag.SelectedCategory;

            //Assert
            Assert.AreEqual(selected, result);
        }

        [TestMethod]
        public void Generate_Category_Specific_Product_Count()
        {
            //Arrange
            Mock<IProductsRepository> mock = new Mock<IProductsRepository>();
            mock.Setup(r => r.Products).Returns(new Product[] {
            new Product {ProductID = 1, Name = "P1", Category = "Cat1"},
            new Product {ProductID = 2, Name = "P2", Category = "Cat2"},
            new Product {ProductID = 3, Name = "P3", Category = "Cat1"},
            new Product {ProductID = 4, Name = "P4", Category = "Cat2"},
            new Product {ProductID = 5, Name = "P5", Category = "Cat3"}
            });

            //Arrange
            ProductController controller = new ProductController(mock.Object);
            controller.PageSize = 3;

            //Action
            int result1 = ((ProductListViewModel)controller.List("Cat1").Model).PagingInfo.TotalItems;
            int result2 = ((ProductListViewModel)controller.List("Cat2").Model).PagingInfo.TotalItems;
            int result3 = ((ProductListViewModel)controller.List("Cat3").Model).PagingInfo.TotalItems;
            int resultAll = ((ProductListViewModel)controller.List(null).Model).PagingInfo.TotalItems;

            //Assert
            Assert.AreEqual(2, result1);
            Assert.AreEqual(2, result2);
            Assert.AreEqual(1, result3);
            Assert.AreEqual(5, resultAll);
        }
    }
}
