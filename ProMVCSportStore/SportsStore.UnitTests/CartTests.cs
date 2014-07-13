using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SportsStore.Domain.Entities;
using System.Linq;
using Moq;
using SportsStore.WebUI.Controllers;
using System.Web.Mvc;
using SportsStore.WebUI.Models;
using SportsStore.Domain.Abstract;

namespace SportsStore.UnitTests
{
    [TestClass]
    public class CartTests
    {
        [TestMethod]
        public void Can_Add_New_Lines()
        {
            //Arrange - create some test product
            Product p1 = new Product { ProductID = 1, Name = "P1" };
            Product p2 = new Product { ProductID = 2, Name = "P2" };

            //Arrange - create a new chart
            Cart target = new Cart();

            //Act
            target.AddItem(p1, 1);
            target.AddItem(p2, 1);
            CartLine[] result = target.Lines.ToArray();

            //Assert
            Assert.AreEqual(result.Length, 2);
            Assert.AreEqual(result[0].Product, p1);
            Assert.AreEqual(result[1].Product, p2);
        }

        [TestMethod]
        public void Can_Add_Quantity_For_Existing_Lines()
        {
            //Arrange - create some test product
            Product p1 = new Product { ProductID = 1, Name = "P1" };
            Product p2 = new Product { ProductID = 2, Name = "P2" };

            //Arrange - create a new chart
            Cart target = new Cart();

            //Act
            target.AddItem(p1, 1);
            target.AddItem(p2, 1);
            target.AddItem(p1, 10);
            CartLine[] result = target.Lines.OrderBy(p => p.Product.ProductID).ToArray();

            //Assert
            Assert.AreEqual(result.Length, 2);
            Assert.AreEqual(result[0].Quantity, 11);
            Assert.AreEqual(result[1].Quantity, 1);
        }

        [TestMethod]
        public void Can_Remove_Line()
        {
            //Arrange - Create some test products
            Product p1 = new Product { ProductID = 1, Name = "P1" };
            Product p2 = new Product { ProductID = 2, Name = "P2" };
            Product p3 = new Product { ProductID = 3, Name = "P3" };

            //Arrange - Create a new cart
            Cart target = new Cart();

            //Arrange - Add some products to the cart
            target.AddItem(p1, 1);
            target.AddItem(p2, 3);
            target.AddItem(p3, 5);
            target.AddItem(p3, 1);

            //Act
            target.RemoveLine(p2);


            //Assert
            Assert.AreEqual(target.Lines.Where(c => c.Product == p2).Count(), 0);
            Assert.AreEqual(target.Lines.Count(), 2);

        }

        [TestMethod]
        public void Calculate_Cart_Total()
        {
            // Arrange - create some test products
            Product p1 = new Product { ProductID = 1, Name = "P1", Price = 100M };
            Product p2 = new Product { ProductID = 2, Name = "P2", Price = 50M };

            Cart target = new Cart();

            // Act
            target.AddItem(p1, 1);
            target.AddItem(p2, 1);
            target.AddItem(p1, 3);

            decimal result = target.ComputeTotalValue();

            //Assert
            Assert.AreEqual(result, 450M);
        }

        [TestMethod]
        public void Can_Clear_Contents()
        {
            // Arrange - create some test products
            Product p1 = new Product
            {
                ProductID = 1,
                Name = "P1",
                Price = 100M
            };
            Product p2 = new Product { ProductID = 2, Name = "P2", Price = 50M };

            Cart target = new Cart();

            //Arrange - add some items
            target.AddItem(p1, 1);
            target.AddItem(p2, 1);

            //Act
            target.clear();

            //Assert
            Assert.AreEqual(target.Lines.Count(), 0);
        }

        [TestMethod]
        public void Can_Add_To_Cart()
        {
            //Arrange create the mock repository
            Mock<IProductsRepository> mock = new Mock<IProductsRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] {
            new Product {ProductID = 1, Name = "P1", Category = "Apples"},
            }.AsQueryable());

            //Arrange create a Cart
            Cart cart = new Cart();

            //Arrange create the controller
            CartController target = new CartController(mock.Object, null);

            //Act
            target.AddToCart(cart, 1, null);

            //Assert
            Assert.AreEqual(1, cart.Lines.Count());
            Assert.AreEqual(1, cart.Lines.ToArray()[0].Product.ProductID);
        }

        [TestMethod]
        public void Adding_Product_To_Cart_Goes_To_Cart_Sceen()
        {
            // Arrange - create the mock repository
            Mock<IProductsRepository> mock = new Mock<IProductsRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] {
            new Product {ProductID = 1, Name = "P1", Category = "Apples"},
            }.AsQueryable());

            // Arrange - create a Cart
            Cart cart = new Cart();
            // Arrange - create the controller
            CartController target = new CartController(mock.Object, null);

            //Act add a product to the cart
            RedirectToRouteResult result = target.AddToCart(cart, 1, "myURL");

            //Assert
            Assert.AreEqual("Index", result.RouteValues["action"], "Index");
            Assert.AreEqual("myURL", result.RouteValues["returnUrl"]);
        }

        [TestMethod]
        public void Can_View_Contents()
        {
            //Arrange create a Cart
            Cart cart = new Cart();

            //Arrage create the controller
            CartController target = new CartController(null, null);

            //Act
            CartIndexViewModel result = (CartIndexViewModel)target.Index(cart, "myUrl").ViewData.Model;

            //Assert
            Assert.AreEqual(cart, result.Cart);
            Assert.AreEqual("myUrl", result.ReturnUrl);
        }

        [TestMethod]
        public void Cannot_Checkout_Empty_Cart()
        {
            //Arrange create a mock orfer processor
            Mock<IOrderProcessor> mock = new Mock<IOrderProcessor>();
            //Arrange create an empty cart
            Cart cart = new Cart();
            //Arrange create shipping details
            ShippingDetails shippingDetails = new ShippingDetails();
            //Arrange create an instance of the controller
            CartController target = new CartController(null, mock.Object);

            //Act
            ViewResult result = target.Checkout(cart, shippingDetails);

            //Assert
            mock.Verify(m => m.ProcessOrder(It.IsAny<Cart>(), It.IsAny<ShippingDetails>()), Times.Never());
            Assert.AreEqual("", result.ViewName);
            Assert.AreEqual(false, result.ViewData.ModelState.IsValid);
        }

        [TestMethod]
        public void Cannot_Checkout_Invalid_ShippingDetails()
        {
            //Arrange create a mock order processor
            Mock<IOrderProcessor> mock = new Mock<IOrderProcessor>();

            //arrange create a cart with an item
            Cart cart = new Cart();
            cart.AddItem(new Product(), 1);

            //arrange create an instance of the controller
            CartController target = new CartController(null, mock.Object);

            //arrange add an error to the model
            target.ModelState.AddModelError("error", "error");

            //Act try to checkout
            ViewResult result = target.Checkout(cart, new ShippingDetails());

            //Assert
            mock.Verify(m => m.ProcessOrder(It.IsAny<Cart>(), It.IsAny<ShippingDetails>()), Times.Never());
            Assert.AreEqual("", result.ViewName);
            Assert.AreEqual(false, result.ViewData.ModelState.IsValid);
        }


        [TestMethod]
        public void Can_Checkout_And_Submit_Order()
        {
            //Arrange create a mock order processor
            Mock<IOrderProcessor> mock = new Mock<IOrderProcessor>();

            //arrange create a cart with an item
            Cart cart = new Cart();
            cart.AddItem(new Product(), 1);

            //arrange create an instance of the controller
            CartController target = new CartController(null, mock.Object);

             //Act try to checkout
            ViewResult result = target.Checkout(cart, new ShippingDetails());

            //Assert
            mock.Verify(m => m.ProcessOrder(It.IsAny<Cart>(), It.IsAny<ShippingDetails>()), Times.Once());
            Assert.AreEqual("Completed", result.ViewName);
            Assert.AreEqual(true, result.ViewData.ModelState.IsValid);
        }
    }
}
