using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;
using SportsStore.WebUI.Controllers;
using SportsStore.WebUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SportsStore.UnitTests
{
    [TestClass]
    public class CartTests
    {
        [TestMethod]
        public void Can_Add_New_Lines()
        {
            //準備 創建測試產品
            Product p1 = new Product { ProductID = 1, Name = "P1" };
            Product p2 = new Product { ProductID = 2, Name = "P2" };

            //準備創建一個新的購物車
            Cart target = new Cart();
            target.AddItem(p1, 1);
            target.AddItem(p2, 1);
            CartLine[] results = target.Lines.ToArray();

            //斷言
            Assert.AreEqual(results.Length, 2);
            Assert.AreEqual(results[0].Product, p1);
            Assert.AreEqual(results[1].Product, p2);
        }

        [TestMethod]
        public void Can_Remove_Line()
        {
            //準備-創建測試產品
            Product p1 = new Product { ProductID = 1, Name = "P1" };
            Product p2 = new Product { ProductID = 2, Name = "P2" };
            Product p3 = new Product { ProductID = 3, Name = "P3" };

            //準備創建一個新的購物車
            Cart target = new Cart();
            //添加產品
            target.AddItem(p1, 1);
            target.AddItem(p2, 3);
            target.AddItem(p3, 5);
            target.AddItem(p2, 1);
            //動作
            target.RemoveLine(p2);

            //斷言
            Assert.AreEqual(target.Lines.Where(c => c.Product == p2).Count(), 0);
            Assert.AreEqual(target.Lines.Count(), 2);

        }

        [TestMethod]
        public void Can_Clear_Contents()
        {
            //準備-創建測試產品
            Product p1 = new Product { ProductID = 1, Name = "P1",Price=100M };
            Product p2 = new Product { ProductID = 2, Name = "P2",Price=50M };
            //準備創建一個新的購物車
            Cart target = new Cart();
            //添加產品s
            target.AddItem(p1, 1);
            target.AddItem(p2, 1);
            //動作 重置購物車
            target.Clear();
            //斷言
            Assert.AreEqual(target.Lines.Count(), 0);

        }

        [TestMethod]
        public void Can_Add_To_Cart()
        {
            //準備--創建模仿存儲庫
            Mock<IProductsRepository> mock = new Mock<IProductsRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] {
                new Product { ProductID=1,Name="p1",Category="Apples"},
            }.AsQueryable());
            Mock<IOrderProcessor> mock1 = new Mock<IOrderProcessor>();

            //準備--創建控制器
            CartController target = new CartController(mock.Object, mock1.Object);
            //準備--創建Cart
            Cart cart = new Cart();
            //動作--對cart添加一個產品
            target.AddToCart(cart, 1, null);
            //斷言
            Assert.AreEqual(cart.Lines.Count(), 1);
            Assert.AreEqual(cart.Lines.ToArray()[0].Product.ProductID, 1);
        }
        [TestMethod]
        public void Adding_Product_To_Cart_Goes_To_Cart_Screen()
        {
            //準備--創建模仿存儲庫
            Mock<IProductsRepository> mock = new Mock<IProductsRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] {
                new Product { ProductID=1,Name="p1",Category="Apples"},
            }.AsQueryable());
            Mock<IOrderProcessor> mock1 = new Mock<IOrderProcessor>();
            //準備--創建Cart
            Cart cart = new Cart();
            //準備--創建控制器
            CartController target = new CartController(mock.Object, mock1.Object);
          
            //動作--對cart添加一個產品
            RedirectToRouteResult result = target.AddToCart(cart, 2, "MyUrl");
            //斷言
            Assert.AreEqual(result.RouteValues["Action"], "Index");
            Assert.AreEqual(result.RouteValues["returnUrl"], "MyUrl");
        }
        [TestMethod]
        public void Can_View_Cart_Contents()
        {
            //準備--創建Cart
            Cart cart = new Cart();
            //準備--創建控制器
            CartController target = new CartController(null, null);

            //動作--對cart添加一個產品
            CartIndexViewModel result = (CartIndexViewModel)target.Index(cart, "MyUrl").ViewData.Model;
            //斷言
            Assert.AreSame(result.Cart, cart);
            Assert.AreEqual(result.ReturnUrl, "MyUrl");
        }

        [TestMethod]
        public void Cannot_Checkout_Empty_Cart()
        {
            Mock<IOrderProcessor> mock = new Mock<IOrderProcessor>();

            ShippingDetails shippingDetails = new ShippingDetails();
            //準備--創建Cart
            Cart cart = new Cart();
            //準備--創建控制器
            CartController target = new CartController(null, mock.Object);

            //動作
            ViewResult result = target.Checkout(cart, shippingDetails);

            //斷言--檢查,訂單商未傳遞給處理器
            mock.Verify(m => m.ProcessOrder(It.IsAny<Cart>(), It.IsAny<ShippingDetails>()), Times.Never());
            //該方法返回的實默認視圖
            Assert.AreEqual("", result.ViewName);
            Assert.AreEqual(false, result.ViewData.ModelState.IsValid);


        }

        [TestMethod]
        public void Can_Checkout_And_Submit_Order()
        {
            Mock<IOrderProcessor> mock = new Mock<IOrderProcessor>();

        
            //準備--創建Cart
            Cart cart = new Cart();
            cart.AddItem(new Product(), 1);

            //準備--創建控制器
            CartController target = new CartController(null, mock.Object);

            //動作
            ViewResult result = target.Checkout(cart, new ShippingDetails());

            //斷言--檢查,訂單已經傳遞給處理器
            mock.Verify(m => m.ProcessOrder(It.IsAny<Cart>(), It.IsAny<ShippingDetails>()), Times.Once());
            //該方法返回的實默認視圖
            Assert.AreEqual("ComPleteds", result.ViewName);
            Assert.AreEqual(true, result.ViewData.ModelState.IsValid);


        }



    }
}
