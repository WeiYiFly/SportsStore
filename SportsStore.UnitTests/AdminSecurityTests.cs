using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SportsStore.WebUI.Infrastructure.Abstract;
using SportsStore.WebUI.Models;
using SportsStore.WebUI.Controllers;
using System.Web.Mvc;

namespace SportsStore.UnitTests
{
    [TestClass]
    public class AdminSecurityTests
    {
        [TestMethod]
        public void Can_Login_With_valid_Credentials()
        {
            //準備-創建一模仿認證提供器
            Mock<IAuthProvider> mock = new Mock<IAuthProvider>();
            mock.Setup(m => m.Authenticate("admin", "secret")).
                Returns(true);
            //準備-創建視圖模型
            LoginViewModel model = new LoginViewModel
            {
                UserName = "admin",
                Password = "secret"
            };
            //準備·--創建控制器
            AccountController target = new AccountController(mock.Object);
            //動作---使用合法的凭据进行认真
              ActionResult result=  target.Login(model, "/MyURL");
            //断言
            Assert.IsInstanceOfType(result, typeof(RedirectResult));
            Assert.AreEqual("/MyURL", ((RedirectResult)result).Url);
        }

        [TestMethod]
        public void Can_Login_With_Invalid_Credentials()
        {
            //準備-創建一模仿認證提供器
            Mock<IAuthProvider> mock = new Mock<IAuthProvider>();
            mock.Setup(m => m.Authenticate("admin", "secret")).Returns(true);
            //準備-創建視圖模型
            LoginViewModel model = new LoginViewModel
            {
                UserName = "badUser",
                Password = "badPass"
            };
            //準備---創建控制器
            AccountController target = new AccountController(mock.Object);
            //動作---使用非法凭证进行验证
            ActionResult result = target.Login(model, "/MyURL");     
            //断言
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            Assert.IsFalse(((ViewResult)result).ViewData.ModelState.IsValid);
        }

    }
}
