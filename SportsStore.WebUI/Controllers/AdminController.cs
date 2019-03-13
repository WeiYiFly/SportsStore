﻿using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SportsStore.WebUI.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private IProductsRepository repository;
      
        public AdminController(IProductsRepository repo)
        {
            repository = repo;

        }
        // GET: Admin
        public ViewResult Index()
        {
            return View(repository.Products);
        }
        public ViewResult Edit(int productId)
        {
            Product product = repository.Products.FirstOrDefault(p => p.ProductID == productId);
            return View(product);            
        }

        public ViewResult Create()
        {
            return View("Edit", new Product());

        }

        #region ajax
        [HttpPost]
        public ActionResult Edit(Product product,HttpPostedFileBase image=null)
        {
            if (ModelState.IsValid)
            {
                if (image != null)
                {
                    product.ImageMimeType = image.ContentType;
                    product.ImageData = new byte[image.ContentLength];
                    image.InputStream.Read(product.ImageData,0, image.ContentLength);
                }
                repository.SaveProduct(product);
                TempData["message"] = string.Format("{0} has been saved", product.Name);
                return RedirectToAction("Index");
            }
            else
            {
                //數值有錯誤
                return View(product);
            }

        }

        [HttpPost]
        public ActionResult Delete(int productid)
        {
            Product deletedProduct = repository.DeleteProduct(productid);
            if (deletedProduct != null)
            {
                TempData["message"] = string.Format("{0} was deleted", deletedProduct.Name);
            }
            return RedirectToAction("Index");
        }
        #endregion

        //[HttpPost]
        //public ActionResult Edit(Product product)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        repository.SaveProduct(product);
        //        TempData["message"] = string.Format("{0} has been saved", product.Name);
        //        return RedirectToAction("Index");
        //    }
        //    else
        //    {
        //        //數值有錯誤
        //        return View(product);
        //    }
        //}


    }
}