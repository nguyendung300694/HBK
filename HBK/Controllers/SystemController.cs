using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HBK.Services;
using HBK.Models;
using Microsoft.AspNet.Identity;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System.Collections;

namespace HBK.Controllers
{
    [Authorize]
    public class SystemController : Controller
    {

        private readonly IMembershipService _MembershipService;
        private readonly ICommonService _CommonService;
        public SystemController(IMembershipService MembershipService, ICommonService CommonService)
        {
            _MembershipService = MembershipService;
            _CommonService = CommonService;
        }
        // GET: System
        public ActionResult Index()
        {
            ApplicationUser currentUser = _MembershipService.GetUserById(User.Identity.GetUserId());
            if (!CheckAdminPermisson(currentUser))
            {
                return RedirectToAction("Login", "Account");
            }
            return View();
        }

        public ActionResult EditingInline_Read([DataSourceRequest] DataSourceRequest request)
        {
            ApplicationUser currentUser = _MembershipService.GetUserById(User.Identity.GetUserId());
            if (CheckAdminPermisson(currentUser))
            {
                return Json(_MembershipService.GetInformationAllMembers(currentUser).ToDataSourceResult(request));
            }
            return Json(null);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EditingInline_Update([DataSourceRequest] DataSourceRequest request, MemberInformationViewModel member)
        {
            ApplicationUser currentUser = _MembershipService.GetUserById(User.Identity.GetUserId());
            if (CheckAdminPermisson(currentUser))
            {
                if (member != null && ModelState.IsValid)
                {
                    var TempUser = _MembershipService.GetUserById(member.Id);
                    TempUser.FirstName = member.FirstName;
                    TempUser.LastName = member.LastName;
                    if (member.IsChange)
                    {
                        TempUser.ExtendAspNetUser.SpecialtyType = member.SpecialtyType;
                    }else
                    {
                        TempUser.ExtendAspNetUser.SpecialtyType = member.SpecialtyTypeCode;
                    }
                    TempUser.ExtendAspNetUser.CareerInfo = member.Career;
                    TempUser.ExtendAspNetUser.CareerDuration = member.CareerDuration;
                    _MembershipService.Save();
                }
                return Json(new[] { member }.ToDataSourceResult(request, ModelState));
            }
            return Json(null);
        }

        public JsonResult SpecialType_Read(string specialTypeID)
        {
            ApplicationUser currentUser = _MembershipService.GetUserById(User.Identity.GetUserId());
            if (CheckAdminPermisson(currentUser))
            {
                var listSpecialType = _CommonService.getCommonById(specialTypeID).ParentCommom.ChildCommom.Select(c => new SpecialTypeItemViewModel { CommCode = c.ComCode, CommName = c.ComName }).ToList();
                return Json(listSpecialType, JsonRequestBehavior.AllowGet);
                //var listSpecialType = _CommonService.getCommonById(specialTypeID).ParentCommom.ChildCommom.Select(c => new SpecialTypeItemViewModel { CommCode = c.ComName, CommName = c.ComName }).ToList();
                //return Json(listSpecialType, JsonRequestBehavior.AllowGet);
            }
            return Json(null);
        }

        private bool CheckAdminPermisson(ApplicationUser currentUser)
        {
            return currentUser.ExtendAspNetUser != null && currentUser.ExtendAspNetUser.SystemAdmin == true;
        }
    }
}