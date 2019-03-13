using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;
using SportsStore.WebUI.Controllers;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using SportsStore.WebUI.Models;
using SportsStore.WebUI.HtmlHelpers;

namespace SportsStore.UnitTests
{
    [TestClass]
    public class UnitTest1
    {

        [TestMethod]
        public void Can_Paginate()
        {
            //準備
            Mock<IProductsRepository> mock = new Mock<IProductsRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] {
            new Product { ProductID=1,Name="P1"},
            new Product { ProductID=2,Name="P2"},
            new Product { ProductID=3,Name="P3"},
            new Product { ProductID=4,Name="P4"},
            new Product { ProductID=5,Name="P5"},
   
            });
            ProductController controller = new ProductController(mock.Object);
            controller.PageSize = 3;

            //動作
            ProductsListViewModel result =(ProductsListViewModel)controller.List1(null,2);

            //斷言
            Product[] prodArray = result.Products.ToArray();
            Assert.IsTrue(prodArray.Length == 2);
            Assert.AreEqual(prodArray[0].Name, "P4");
            Assert.AreEqual(prodArray[1].Name, "P5");
        }
        [TestMethod]
        public void Can_Generate_Page_Links() {
            //準備 --定義一個HTML輔助器,這是必須的,目的是運用擴展方法
            HtmlHelper myHelper = null;

            //準備 創建PagingInfo數據
            PagingInfo pagingInfo = new PagingInfo
            {
                CurrentPage = 2,
                TotalItems = 28,
                ItemsPerPage = 10
            };

            //準備用lambda 表達式建立委託
            Func<int, string> pageUrlDelegate = i => "Page" + i;

            //動作
            MvcHtmlString result = myHelper.PageLinks(pagingInfo, pageUrlDelegate);

            //斷言
            Assert.AreEqual(@"<a class=""btn btn-default"" href=""Page1"">1<a/>"
                + @"<a class=""btn btn-default btn-primary selected"" href=""Page2"">2</a>"
                + @"<a class=""btn btn-default"" href=""Page3"">3</a>", result.ToString());


        }

        [TestMethod]
        public void Can_Send_Pagination_View_Model()
        {
            //準備
            Mock<IProductsRepository> mock = new Mock<IProductsRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] {
            new Product { ProductID=1,Name="P1"},
            new Product { ProductID=2,Name="P2"},
            new Product { ProductID=3,Name="P3"},
            new Product { ProductID=4,Name="P4"},
            new Product { ProductID=5,Name="P5"},

            });
            ProductController controller = new ProductController(mock.Object);
            controller.PageSize = 3;

            //動作
            ProductsListViewModel result = (ProductsListViewModel)controller.List1(null,2);

            //斷言
            PagingInfo pageInfo = result.PagingInfo;        
            Assert.AreEqual(pageInfo.CurrentPage, 2);
            Assert.AreEqual(pageInfo.ItemsPerPage, 3);
            Assert.AreEqual(pageInfo.TotalItems, 5);
            Assert.AreEqual(pageInfo.TotalPages, 2);
    
        }

        [TestMethod]
        public void Can_Filter_Products()
        {
            //準備
            Mock<IProductsRepository> mock = new Mock<IProductsRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] {
            new Product { ProductID=1,Name="P1",Category="Cat1"},
            new Product { ProductID=2,Name="P2",Category="Cat2"},
            new Product { ProductID=3,Name="P3",Category="Cat1"},
            new Product { ProductID=4,Name="P4",Category="Cat2"},
            new Product { ProductID=5,Name="P5",Category="Cat3"},

            });
            ProductController controller = new ProductController(mock.Object);
            controller.PageSize = 3;

            //動作
            ProductsListViewModel result = (ProductsListViewModel)controller.List1("Cat2", 2);

            //斷言
            Product[] prodArray = result.Products.ToArray();
            Assert.AreEqual(prodArray.Length, 2);
            Assert.AreEqual(prodArray[0].Name, "P2");
            Assert.AreEqual(prodArray[1].Name, "P4");
          

        }

        [TestMethod]
        public void Can_Create_Categories()
        {
            //準備 創建模仿
            Mock<IProductsRepository> mock = new Mock<IProductsRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] {
                new Product { ProductID=1,Name="P1",Category="Apples"},
                new Product { ProductID=2,Name="P2",Category="Apples"},
                new Product { ProductID=3,Name="P3",Category="Plums"},
                new Product { ProductID=4,Name="P4",Category="Oranges"},
            });

            //備註一創建控制器
            NavController target = new NavController(mock.Object);

            //動作一獲取分類集合
            string[] results = ((IEnumerable<string>)target.Menu().Model).ToArray();

            //斷言
            Assert.AreEqual(results.Length, 3);
            Assert.AreEqual(results[0],"Apples");
            Assert.AreEqual(results[1], "Oranges");
            Assert.AreEqual(results[2], "Plums");
        }

        [TestMethod]
        public void Indicates_Selected_Category()
        {
            //準備 創建模仿
            Mock<IProductsRepository> mock = new Mock<IProductsRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] {
                new Product { ProductID=1,Name="P1",Category="Apples"},
                new Product { ProductID=4,Name="P4",Category="Oranges"},
            });

            //準備一創建控制器
            NavController target = new NavController(mock.Object);

            //準備定義一個已選分類
            string categoryToSelect = "Apples";

            //動作
            string result = target.Menu(categoryToSelect).ViewBag.SelectedCategory;

            //斷言
            Assert.AreEqual(categoryToSelect, result);
        }

        [TestMethod]
        public void Generate_Category_Specific_Product_Count()
        {
            //準備 創建模仿
            Mock<IProductsRepository> mock = new Mock<IProductsRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] {
            new Product { ProductID = 1, Name = "P1", Category = "Cat1" },
            new Product { ProductID = 2, Name = "P2", Category = "Cat2" },
            new Product { ProductID = 3, Name = "P3", Category = "Cat1" },
            new Product { ProductID = 4, Name = "P4", Category = "Cat2" },
            new Product { ProductID = 5, Name = "P5", Category = "Cat3" }
        });
            //準備一個創建控制器並使用頁面容納3個物品
            ProductController target = new ProductController(mock.Object);
            target.PageSize = 3;

            //動作 測試不同分類的產品數
            int res1 = ((ProductsListViewModel)target.List("Cat1").Model).PagingInfo.TotalItems;
            int res2 = ((ProductsListViewModel)target.List("Cat2").Model).PagingInfo.TotalItems;
            int res3 = ((ProductsListViewModel)target.List("Cat3").Model).PagingInfo.TotalItems;
            int resAll = ((ProductsListViewModel)target.List(null).Model).PagingInfo.TotalItems;
            //斷言
            Assert.AreEqual(res1, 2);
            Assert.AreEqual(res2, 2);
            Assert.AreEqual(res3, 1);
            Assert.AreEqual(resAll, 5);
        }

    }
}
