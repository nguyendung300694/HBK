using HBK.Models;
using HBK.Services;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace HBK.Controllers
{
    public class AccountManagementController : Controller
    {

        private readonly IMembershipService _MembershipService;
        private readonly ICommonService _CommonService;
        private ApplicationUserManager _userManager;
        private readonly IUsersPhotoService _UsersPhotoService;

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        private ApplicationSignInManager _signInManager;

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public AccountManagementController(IMembershipService MembershipService, ICommonService CommonService, IUsersPhotoService UsersPhotoService)
        {
            _MembershipService = MembershipService;
            _CommonService = CommonService;
            _UsersPhotoService = UsersPhotoService;
        }

        // GET: AccountManagement
        public ActionResult Index()
        {
            ApplicationUser currentUser = _MembershipService.GetUserById(User.Identity.GetUserId());
            if (!CheckAdminPermisson(currentUser))
            {
                return RedirectToAction("Login", "Account");
            }

            ViewBag.IsAdmin = currentUser.ExtendAspNetUser != null && currentUser.ExtendAspNetUser.SystemAdmin == true;
            ViewBag.ShowPopUp = false;
            InitDropDownlist();
            MyInformationViewModel model = new MyInformationViewModel();
            //model.IsEditing = true;
            return View(model);
        }

        private void InitDropDownlist()
        {
            var objDict = new Dictionary<string, string>();
            foreach (var cultureInfo in CultureInfo.GetCultures(CultureTypes.SpecificCultures))
            {
                var regionInfo = new RegionInfo(cultureInfo.Name);
                if (!objDict.ContainsKey(regionInfo.EnglishName))
                {
                    objDict.Add(regionInfo.EnglishName, regionInfo.TwoLetterISORegionName.ToLower());
                }
            }
            var obj = objDict.OrderBy(p => p.Key).ToArray();
            ViewBag.ListCountries = obj.Select(c => new SelectListItem
            {
                Text = c.Key,
                Value = c.Key
            });
            ViewBag.ListType = _CommonService.getAllSpecialtyType().ToList().Select(c => new SelectListItem
            {
                Text = c.ComName,
                Value = c.ComCode
            });
            //if (!string.IsNullOrEmpty(MainSpecialtyType))
            //{
            //    ViewBag.ListSubType = _CommonService.getCommonById(MainSpecialtyType).ChildCommom.Select(c => new SelectListItem
            //    {
            //        Text = c.ComName,
            //        Value = c.ComCode
            //    });
            //}
        }

        public ActionResult EditingInline_Read([DataSourceRequest] DataSourceRequest request)
        {
            ApplicationUser currentUser = _MembershipService.GetUserById(User.Identity.GetUserId());
            if (CheckAdminPermisson(currentUser))
            {
                return Json(_MembershipService.GetInformationAllAccounts(currentUser).ToDataSourceResult(request));
            }
            return Json(null);
        }

        //[AcceptVerbs(HttpVerbs.Post)]
        //public ActionResult EditingInline_Update([DataSourceRequest] DataSourceRequest request, MemberInformationViewModel member)
        //{
        //    ApplicationUser currentUser = _MembershipService.GetUserById(User.Identity.GetUserId());
        //    if (CheckAdminPermisson(currentUser))
        //    {
        //        if (member != null && ModelState.IsValid)
        //        {
        //            var TempUser = _MembershipService.GetUserById(member.Id);
        //            TempUser.FirstName = member.FirstName;
        //            TempUser.LastName = member.LastName;
        //            if (member.IsChange)
        //            {
        //                TempUser.ExtendAspNetUser.SpecialtyType = member.SpecialtyType;
        //            }
        //            else
        //            {
        //                TempUser.ExtendAspNetUser.SpecialtyType = member.SpecialtyTypeCode;
        //            }
        //            TempUser.ExtendAspNetUser.CareerInfo = member.Career;
        //            TempUser.ExtendAspNetUser.CareerDuration = member.CareerDuration;
        //            _MembershipService.Save();
        //        }
        //        return Json(new[] { member }.ToDataSourceResult(request, ModelState));
        //    }
        //    return Json(null);
        //}

        //public JsonResult SpecialType_Read(string specialTypeID)
        //{
        //    ApplicationUser currentUser = _MembershipService.GetUserById(User.Identity.GetUserId());
        //    if (CheckAdminPermisson(currentUser))
        //    {
        //        var listSpecialType = _CommonService.getCommonById(specialTypeID).ParentCommom.ChildCommom.Select(c => new SpecialTypeItemViewModel { CommCode = c.ComCode, CommName = c.ComName }).ToList();
        //        return Json(listSpecialType, JsonRequestBehavior.AllowGet);
        //        //var listSpecialType = _CommonService.getCommonById(specialTypeID).ParentCommom.ChildCommom.Select(c => new SpecialTypeItemViewModel { CommCode = c.ComName, CommName = c.ComName }).ToList();
        //        //return Json(listSpecialType, JsonRequestBehavior.AllowGet);
        //    }
        //    return Json(null);
        //}

        private bool CheckAdminPermisson(ApplicationUser currentUser)
        {
            return currentUser.ExtendAspNetUser != null && currentUser.ExtendAspNetUser.SystemAdmin == true;
        }

        [HttpPost]
        public async Task<JsonResult> CreateAccount(RegisterViewModel model)
        {
            ApplicationUser currentUser = _MembershipService.GetUserById(User.Identity.GetUserId());
            if (CheckAdminPermisson(currentUser))
            {
                if (ModelState.IsValid || User.Identity.IsAuthenticated)
                {
                    var user = new ApplicationUser
                    {
                        UserName = model.Email,
                        Email = model.Email,
                        PhoneNumber = model.Tel,
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Address = model.Address,
                        Country = model.Country
                    };
                    var result = await UserManager.CreateAsync(user, model.Password);
                    if (result.Succeeded)
                    {
                        //await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                        // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                        // Send an email with this link
                        // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                        // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                        // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");
                        _MembershipService.AddExtendUser(new ExtendAspNetUser
                        {
                            UserID = user.Id,
                            KorName = model.FirstName + " " + model.LastName,
                            EngName = model.FirstName + " " + model.LastName,
                            SpecialtyType = model.SpecialtyType,
                            SnsSite = model.SNSSite,
                            CareerInfo = model.Career,
                            CareerDuration = model.CareerDuration,
                            SelfIntroduction = model.Introduction,
                            Recommender = model.Recommender,
                            RegistrationDate = DateTime.Now,
                            LastLoginDate = DateTime.Now,
                            SystemAdmin = false
                        });
                        _UsersPhotoService.AddUsersPhoto(new UsersPhoto
                        {
                            UserID = user.Id,
                            UserPhtoFileLocationPath = Util.CreateUPhoto(user.Id, model.Photo),
                            UserPhtoFileName = model.Photo.FileName,
                            UserPhtoFileSize = model.Photo.ContentLength,
                            UserPhtoFileType = model.Photo.ContentType
                        });
                        return Json(new { success = true });
                    }
                    AddErrors(result);
                }
            }
            // If we got this far, something failed, redisplay form
            InitDropDownlist();
            return Json(new { success = false });
        }

        [HttpPost]
        public JsonResult DeleteAccount(string Id)
        {

            ApplicationUser currentUser = _MembershipService.GetUserById(User.Identity.GetUserId());
            if (currentUser.ExtendAspNetUser != null && currentUser.ExtendAspNetUser.SystemAdmin)
            {
                _MembershipService.DeleteAccount(Id);
                return Json(true);
            }
            return Json(false);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateAccount(MyInformationViewModel model)
        {
            ApplicationUser currentUser = _MembershipService.GetUserById(User.Identity.GetUserId());
            ApplicationUser EditUser = _MembershipService.GetUserByEmail(model.EditAccount.Email);
            if (CheckPermission(currentUser, EditUser.Id))
            {
                if (model.IsEditing)
                {
                    if (ModelState.ContainsKey("ChangePassword.OldPassword"))
                        ModelState["ChangePassword.OldPassword"].Errors.Clear();
                    if (ModelState.ContainsKey("ChangePassword.Password"))
                        ModelState["ChangePassword.Password"].Errors.Clear();
                    if (ModelState.IsValid)
                    {
                        EditUser.FirstName = model.EditAccount.FirstName;
                        EditUser.FirstName = model.EditAccount.FirstName;
                        EditUser.LastName = model.EditAccount.LastName;
                        EditUser.PhoneNumber = model.EditAccount.Tel;
                        EditUser.Address = model.EditAccount.Address;
                        EditUser.Country = model.EditAccount.Country;
                        if (EditUser.ExtendAspNetUser != null)
                        {
                            EditUser.ExtendAspNetUser.KorName = model.EditAccount.FirstName + " " + model.EditAccount.LastName;
                            EditUser.ExtendAspNetUser.EngName = model.EditAccount.FirstName + " " + model.EditAccount.LastName;
                            EditUser.ExtendAspNetUser.SpecialtyType = model.EditAccount.SpecialtyType;
                            EditUser.ExtendAspNetUser.SnsSite = model.EditAccount.SNSSite;
                            EditUser.ExtendAspNetUser.CareerInfo = model.EditAccount.Career;
                            EditUser.ExtendAspNetUser.SelfIntroduction = model.EditAccount.Introduction;
                            EditUser.ExtendAspNetUser.Recommender = model.EditAccount.Recommender;
                            EditUser.ExtendAspNetUser.CareerDuration = model.EditAccount.CareerDuration;
                            _MembershipService.Save();
                        }
                        else
                        {
                            _MembershipService.AddExtendUser(new ExtendAspNetUser
                            {
                                UserID = EditUser.Id,
                                KorName = model.EditAccount.FirstName + " " + model.EditAccount.LastName,
                                EngName = model.EditAccount.FirstName + " " + model.EditAccount.LastName,
                                SpecialtyType = model.EditAccount.SpecialtyType,
                                SnsSite = model.EditAccount.SNSSite,
                                CareerInfo = model.EditAccount.Career,
                                CareerDuration = model.EditAccount.CareerDuration,
                                SelfIntroduction = model.EditAccount.Introduction,
                                Recommender = model.EditAccount.Recommender,
                                RegistrationDate = DateTime.Now,
                                LastLoginDate = DateTime.Now,
                                SystemAdmin = false
                            });
                        }
                        if (model.EditAccount.Photo != null)
                        {
                            _UsersPhotoService.AddUsersPhoto(new UsersPhoto
                            {
                                UserID = EditUser.Id,
                                UserPhtoFileLocationPath = Util.CreateUPhoto(EditUser.Id, model.EditAccount.Photo),
                                UserPhtoFileName = model.EditAccount.Photo.FileName,
                                UserPhtoFileSize = model.EditAccount.Photo.ContentLength,
                                UserPhtoFileType = model.EditAccount.Photo.ContentType
                            });
                        }
                        return RedirectToAction("Index", "AccountManagement");
                    }
                }
                else
                {
                    var result = UserManager.ChangePassword(User.Identity.GetUserId(), model.ChangePassword.OldPassword, model.ChangePassword.Password);
                    if (result.Succeeded)
                    {
                        var user = UserManager.FindById(User.Identity.GetUserId());
                        if (user != null)
                        {
                            SignInManager.SignIn(user, isPersistent: false, rememberBrowser: false);
                        }
                        return RedirectToAction("Index", "AccountManagement");
                    }
                    AddErrors(result);
                }
                ViewBag.IsAdmin = currentUser.ExtendAspNetUser != null && currentUser.ExtendAspNetUser.SystemAdmin == true;
                InitDropDownlist();
                ViewBag.ShowPopUp = true;
                return View(model);
            }
            else
            {
                return RedirectToAction("Index", "AccountManagement");
            }
        }

        private bool CheckPermission(ApplicationUser currentUser, string writerId)
        {
            if (currentUser.Id.Equals(writerId))
            {
                return true;
            }
            else
            {
                return currentUser.ExtendAspNetUser != null && currentUser.ExtendAspNetUser.SystemAdmin;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }
    }
}