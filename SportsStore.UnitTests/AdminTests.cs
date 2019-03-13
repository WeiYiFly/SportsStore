
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;
using Moq;
using SportsStore.WebUI.Controllers;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace SportsStore.UnitTests
{
    /// <summary>
    /// AdminTests 的摘要说明
    /// </summary>
    [TestClass]
    public class AdminTests
    {
        
    
        [TestMethod]
        public void Index_Contains_All_Products()
        {
            //準備
            Mock<IProductsRepository> mock = new Mock<IProductsRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] {
            new Product { ProductID=1,Name="P1"},
            new Product { ProductID=2,Name="P2"},
            new Product { ProductID=3,Name="P3"},
            });

            //準備--創建控制器
            AdminController target = new AdminController(mock.Object);

            //動作
            Product[] result = ((IEnumerable<Product>)target.Index().ViewData.Model).ToArray();

            //斷言
            Assert.AreEqual(result.Length, 3);
            Assert.AreEqual("P1", result[0].Name);
            Assert.AreEqual("P2", result[2].Name);
            Assert.AreEqual("P3", result[3].Name);


        }

        [TestMethod]
        public void Can_Edit_Product()
        {
            //準備
            Mock<IProductsRepository> mock = new Mock<IProductsRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] {
            new Product { ProductID=1,Name="P1"},
            new Product { ProductID=2,Name="P2"},
            new Product { ProductID=3,Name="P3"},
            });

            //準備--創建控制器
            AdminController target = new AdminController(mock.Object);

            //動作
            Product p1 = target.Edit(1).ViewData.Model as Product;
            Product p2 = target.Edit(2).ViewData.Model as Product;
            Product p3 = target.Edit(3).ViewData.Model as Product;

            //斷言
            Assert.AreEqual(1, p1.ProductID);
            Assert.AreEqual(2, p2.ProductID);
            Assert.AreEqual(3, p3.ProductID);


        }

        [TestMethod]
        public void Can_Edit_Nonexistent_Product()
        {
            //準備
            Mock<IProductsRepository> mock = new Mock<IProductsRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] {
            new Product { ProductID=1,Name="P1"},
            new Product { ProductID=2,Name="P2"},
            new Product { ProductID=3,Name="P3"},
            });

            //準備--創建控制器
            AdminController target = new AdminController(mock.Object);

            //動作
            Product result = (Product)target.Edit(4).ViewData.Model;
         

            //斷言
            Assert.IsNull(result);
   
        }

        [TestMethod]
        public void Can_Save_Valid_Changes()
        {
            //準備
            Mock<IProductsRepository> mock = new Mock<IProductsRepository>();

            //準備--創建控制器
            AdminController target = new AdminController(mock.Object);

            //準備-一個產品
            Product product = new Product { Name = "Test" };
        
            //動作 ---保存這個產品
            ActionResult result = target.Edit(product);

            //斷言-檢查，調用了存儲庫
            mock.Verify(m => m.SaveProduct(product));

            //斷言--檢查方法的結構類型
            Assert.IsNotInstanceOfType(result,typeof(ViewResult));

        }

        [TestMethod]
        public void Can_Save_Invalid_Changes()
        {
            //準備
            Mock<IProductsRepository> mock = new Mock<IProductsRepository>();

            //準備--創建控制器
            AdminController target = new AdminController(mock.Object);

            //準備-一個產品
            Product product = new Product { Name = "Test" };
            //準備--把一個錯誤添加到模型狀態
            target.ModelState.AddModelError("error", "error");

            //動作 ---保存這個產品
            ActionResult result = target.Edit(product);

            //斷言-檢查，確認存儲庫沒有被調用
            mock.Verify(m => m.SaveProduct(It.IsAny<Product>()),Times.Never());

            //斷言--檢查方法的結構類型
            Assert.IsNotInstanceOfType(result, typeof(ViewResult));

        }

        [TestMethod]
        public void Can_Delete_Valid_Product()
        {
            //準備-一個產品
            Product prod = new Product { ProductID=2,Name = "Test" };
            //準備
            Mock<IProductsRepository> mock = new Mock<IProductsRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] {
            new Product { ProductID=1,Name="P1"},
            prod,
            new Product { ProductID=3,Name="P3"},
            });

            //準備--創建控制器
            AdminController target = new AdminController(mock.Object);

            //動作
             target.Delete(prod.ProductID);


            //斷言--確保存儲庫的刪除方法是針對正確的產品被調用的
            mock.Verify(m=>m.DeleteProduct(prod.ProductID));

        }


    }
}
