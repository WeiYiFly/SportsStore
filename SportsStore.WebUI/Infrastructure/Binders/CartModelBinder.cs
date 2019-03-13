using SportsStore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SportsStore.WebUI.Infrastructure.Binders
{
    public class CartModelBinder: IModelBinder
    {
        private const string sessionKey = "Cart";
        public object BindModel(ControllerContext controllContext, ModelBindingContext bindingContext)
        {
            //通過會話獲取Cart
             Cart cart = null;
        if (controllContext.HttpContext.Session != null)
        {
            cart = (Cart)controllContext.HttpContext.Session[sessionKey];
        }
        //若會話中沒有Cart，則創建一個
        if (cart == null)
        {
            cart = new Cart();
            if (controllContext.HttpContext.Session != null)
            {
                controllContext.HttpContext.Session[sessionKey] = cart;
            }
        }
        return cart;

        }
    }
}