using HBK.Models;
using HBK.Services;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System.Collections;
using System.Collections.Generic;
using System.Web.Mvc;
using System;
using Microsoft.AspNet.Identity;

namespace HBK.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ICommunityService _CommunityService;
        private readonly ICommunityAttachmentService _CommunityAttachmentService;
        public HomeController(ICommunityService CommunityService, ICommunityAttachmentService CommunityAttachmentService)
        {
            _CommunityService = CommunityService;
            _CommunityAttachmentService = CommunityAttachmentService;
        }

        [AllowAnonymous]
        public ActionResult Index()
        {
            return View();
        }

    }
}
