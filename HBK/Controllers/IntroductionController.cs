using HBK.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HBK.Controllers
{
    [Authorize]
    public class IntroductionController : Controller
    {
        private readonly IProductService _productService;
        private readonly IProductCategoryService _productCategoryService;

        public IntroductionController(IProductService productService,
          IProductCategoryService productCategoryService)
        {
            _productService = productService;
            _productCategoryService = productCategoryService;
        }

        // GET: Introduction
        [AllowAnonymous]
        public ActionResult Index()
        {
            var listCategory = _productCategoryService.GetAllProductCatrgory();
            ViewBag.ListCategory = listCategory;
            return View();
        }
    }
}