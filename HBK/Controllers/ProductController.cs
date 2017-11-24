using HBK.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HBK.Controllers
{
    [Authorize]
    public class ProductController : Controller
    {

        private readonly IProductService _productService;
        private readonly IProductCategoryService _productCategoryService;

        public ProductController(IProductService productService,
          IProductCategoryService productCategoryService)
        {
            _productService = productService;
            _productCategoryService = productCategoryService;
        }

        [AllowAnonymous]
        // GET: Product
        public ActionResult Index(int? productCategoryId)
        {
            if (productCategoryId != null)
            {
                var listCategory = _productCategoryService.GetAllProductCatrgory();
                ViewBag.ListCategory = listCategory;
                var productCategory = _productCategoryService.GetProductCategoryById(productCategoryId);
                ViewBag.CategoryName = productCategory.ProductCategoryName;
                ViewBag.ListProduct = productCategory.Products;
            }
            return View();
        }
    }
}