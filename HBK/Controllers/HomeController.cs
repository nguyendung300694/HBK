using HBK.Models;
using HBK.Services;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System.Collections;
using System.Collections.Generic;
using System.Web.Mvc;
using System;
using Microsoft.AspNet.Identity;
using System.Web;

namespace HBK.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {

        private readonly IProductService _productService;
        private readonly IProductCategoryService _productCategoryService;


        public HomeController(IProductService productService,
            IProductCategoryService productCategoryService)
        {
            _productService = productService;
            _productCategoryService = productCategoryService;
        }

        [AllowAnonymous]
        public ActionResult Index()
        {
            var listCategory = _productCategoryService.GetAllProductCatrgory();
            var listHotProduct = _productService.GetFourHotProduct();
            ViewBag.ListHotProduct = listHotProduct;
            ViewBag.ListCategory = listCategory;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public JsonResult SaveProduct(List<HttpPostedFileBase> Files)
        {
            var count = 1;

            foreach (var item in Files)
            {
                Product product = new Product()
                {
                    ProductName = "San pham " + count++,
                    CreatedDate = DateTime.Now,
                    ProductCategoryId = 2
                };

                _productService.CreateProduct(product);

                var productId = product.ProductId;

                ProductExtend model = new ProductExtend()
                {
                    ProductId = productId,
                    FileName = item.FileName,
                    FileSize = item.ContentLength,
                    FilePath = Util.CreateProductImage(item, "KhiNenThuyLuc")
                };
                _productService.CreateProductExtend(model);
            }

            _productService.Save();

            return Json(new { success = false });
        }
    }
}
