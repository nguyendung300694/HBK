using HBK.Models;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HBK.Services;
using Microsoft.AspNet.Identity;
using Kendo.Mvc.Extensions;

namespace HBK.Controllers
{
    [Authorize]
    public class CommonCodeController : Controller
    {
        private readonly ICommonService _CommonService;
        private readonly IMembershipService _MembershipService;

        public CommonCodeController(ICommonService CommonService, IMembershipService MembershipService)
        {
            _CommonService = CommonService;
            _MembershipService = MembershipService;
        }
        // GET: Test
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult EditingInline_Read([DataSourceRequest] DataSourceRequest request)
        {
            ApplicationUser currentUser = _MembershipService.GetUserById(User.Identity.GetUserId());
            if (CheckAdminPermisson(currentUser))
            {
                return Json(_CommonService.getAllCommon().ToDataSourceResult(request));
            }
            else return Json(null);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EditingInline_Create([DataSourceRequest] DataSourceRequest request, CommonModelView model)
        {
            ApplicationUser currentUser = _MembershipService.GetUserById(User.Identity.GetUserId());
            if (CheckAdminPermisson(currentUser))
            {
                if (model != null && ModelState.IsValid)
                {
                    Common common = new Common
                    {
                        ComCode = model.ComCode,
                        ComSubCode = model.ComSubCode == null ? null : model.ComSubCode,
                        ComName = model.ComName,
                        ComName2 = model.ComName2,
                        CommonType = model.CommonType
                    };
                    _CommonService.Create(common);
                }
                return Json(new[] { model }.ToDataSourceResult(request, ModelState));
                //return Json(ModelState.IsValid ? new object() : ModelState.ToDataSourceResult());
            }
            return Json(null);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EditingInline_Update([DataSourceRequest] DataSourceRequest request, CommonModelView model)
        {
            ApplicationUser currentUser = _MembershipService.GetUserById(User.Identity.GetUserId());
            if (CheckAdminPermisson(currentUser))
            {
                if (model != null && ModelState.IsValid)
                {
                    Common comm = _CommonService.getCommonById(model.ComCode);
                    if (model.IsChange)
                    {
                        comm.ComSubCode = model.ComSubCode;
                    }
                    else
                    {
                        comm.ComSubCode = model.TempComSubCode;
                    }
                    //comm.ComSubCode = model.ComSubCode == null ? null : model.ComSubCode;
                    comm.ComName = model.ComName;
                    comm.ComName2 = model.ComName2;
                    comm.CommonType = model.CommonType;
                    _CommonService.Save();
                }
                return Json(new[] { model }.ToDataSourceResult(request, ModelState));
            }
            return Json(null);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EditingInline_Destroy([DataSourceRequest] DataSourceRequest request, CommonModelView model)
        {
            ApplicationUser currentUser = _MembershipService.GetUserById(User.Identity.GetUserId());
            if (CheckAdminPermisson(currentUser))
            {
                if (model != null)
                {
                    Common common = _CommonService.getCommonById(model.ComCode);
                    _CommonService.Destroy(common);
                }
                return Json(new[] { model }.ToDataSourceResult(request, ModelState));
            }
            return Json(null);
        }

        public JsonResult SpecialType_Read()
        {
            ApplicationUser currentUser = _MembershipService.GetUserById(User.Identity.GetUserId());
            if (CheckAdminPermisson(currentUser))
            {
                var listSpecialType = new List<SpecialTypeItemViewModel>();
                var listSpecialTypeModel = _CommonService.getAllSpecialtyType().Select(c => new SpecialTypeItemViewModel { CommCode = c.ComCode, CommName = c.ComCode }).ToList();
                listSpecialType.Add(new SpecialTypeItemViewModel { CommCode = null, CommName = App_GlobalResources.Langues.Root });
                listSpecialType.AddRange(listSpecialTypeModel);
                return Json(listSpecialType, JsonRequestBehavior.AllowGet);
            }
            return Json(null);
        }

        private bool CheckAdminPermisson(ApplicationUser currentUser)
        {
            return currentUser.ExtendAspNetUser != null && currentUser.ExtendAspNetUser.SystemAdmin == true;
        }
    }
}