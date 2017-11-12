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
using System.Web;
using System.Web.Mvc;

namespace HBK.Controllers
{
    [Authorize]
    public class MemberController : Controller
    {
        private ApplicationUserManager _userManager;
        private readonly IMembershipService _MembershipService;
        private readonly ICommonService _CommonService;
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

        public MemberController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public MemberController(ICommonService CommonService, IMembershipService MembershipService, IUsersPhotoService UsersPhotoService)
        {
            _CommonService = CommonService;
            _MembershipService = MembershipService;
            _UsersPhotoService = UsersPhotoService;
        }

        // GET: Member
        public ActionResult Index()
        {
            ApplicationUser currentUser = _MembershipService.GetUserById(User.Identity.GetUserId());
            ViewBag.IsAdmin = currentUser.ExtendAspNetUser != null && currentUser.ExtendAspNetUser.SystemAdmin == true;
            ViewBag.ShowPopUp = false;
            InitDropDownlist();
            MyInformationViewModel model = new MyInformationViewModel();
            model.IsEditing = true;
            return View(model);
        }

        public ActionResult Members_Read([DataSourceRequest] DataSourceRequest request)
        {
            ApplicationUser currentUser = _MembershipService.GetUserById(User.Identity.GetUserId());
            return Json(_MembershipService.GetAllMembers(currentUser).ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ChangeLockOut(string Email)
        {
            ApplicationUser currentUser = _MembershipService.GetUserById(User.Identity.GetUserId());
            if (currentUser.ExtendAspNetUser != null && currentUser.ExtendAspNetUser.SystemAdmin)
            {
                _MembershipService.ChangeLockOut(Email);
                return Json(true);
            }
            return Json(false);
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

        [HttpPost]
        public JsonResult UserDetails(string Email)
        {
            ApplicationUser currentUser = _MembershipService.GetUserById(User.Identity.GetUserId());
            ApplicationUser EditUser = _MembershipService.GetUserByEmail(Email);
            if (CheckPermission(currentUser, EditUser.Id))
            {
                EditAccountViewModel model = new EditAccountViewModel();
                model.Email = EditUser.Email;
                model.FirstName = EditUser.FirstName;
                model.LastName = EditUser.LastName;
                model.Tel = EditUser.PhoneNumber;
                model.Address = EditUser.Address ?? "";
                model.Country = EditUser.Country;
                model.OldPhoto = "../" + getAvatar(currentUser).Substring(2);
                model.MainSpecialtyType = currentUser.ExtendAspNetUser != null ? currentUser.ExtendAspNetUser.Common.ParentCommom.ComCode : string.Empty;
                if (!String.IsNullOrEmpty(model.MainSpecialtyType))
                {
                    var SpecialtyTypes = _CommonService.getCommonById(model.MainSpecialtyType).ChildCommom;
                    if (SpecialtyTypes.Any())
                    {
                        model.ListSpecialtyType = SpecialtyTypes.Select(c => new { value = c.ComCode, text = c.ComName });
                    }
                }
                model.SpecialtyType = EditUser.ExtendAspNetUser != null ? EditUser.ExtendAspNetUser.SpecialtyType : string.Empty;
                model.SNSSite = EditUser.ExtendAspNetUser != null ? EditUser.ExtendAspNetUser.SnsSite : string.Empty;
                model.Recommender = EditUser.ExtendAspNetUser != null ? EditUser.ExtendAspNetUser.Recommender : string.Empty;
                model.Introduction = EditUser.ExtendAspNetUser != null ? EditUser.ExtendAspNetUser.SelfIntroduction : string.Empty;
                model.Career = EditUser.ExtendAspNetUser != null ? EditUser.ExtendAspNetUser.CareerInfo : string.Empty;
                model.CareerDuration = EditUser.ExtendAspNetUser != null ? EditUser.ExtendAspNetUser.CareerDuration : 0;
                return Json(new { success = true, model });
            }
            else
            {
                return Json(new { success = false });
            }
        }

        private string getAvatar(ApplicationUser writer)
        {
            string avatar = Util.DefaultAvatar();
            var Extend = writer.ExtendAspNetUser;
            if (Extend != null)
            {
                var UsersPhoto = Extend.UsersPhotos.LastOrDefault();
                if (UsersPhoto != null)
                {
                    avatar = UsersPhoto.UserPhtoFileLocationPath;
                }
            }
            return avatar;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(MyInformationViewModel model)
        {
            ApplicationUser currentUser = _MembershipService.GetUserById(User.Identity.GetUserId());
            ApplicationUser EditUser = _MembershipService.GetUserByEmail(model.EditAccount.Email);
            if (CheckPermission(currentUser, EditUser.Id))
            {
                //if (ModelState.IsValid)
                //{
                //    EditUser.FirstName = model.FirstName;
                //    EditUser.FirstName = model.FirstName;
                //    EditUser.LastName = model.LastName;
                //    EditUser.PhoneNumber = model.Tel;
                //    EditUser.Address = model.Address;
                //    EditUser.Country = model.Country;
                //    if (EditUser.ExtendAspNetUser != null)
                //    {
                //        EditUser.ExtendAspNetUser.KorName = model.FirstName + " " + model.LastName;
                //        EditUser.ExtendAspNetUser.EngName = model.FirstName + " " + model.LastName;
                //        EditUser.ExtendAspNetUser.SpecialtyType = model.SpecialtyType;
                //        EditUser.ExtendAspNetUser.SnsSite = model.SNSSite;
                //        EditUser.ExtendAspNetUser.CareerInfo = model.Career;
                //        EditUser.ExtendAspNetUser.SelfIntroduction = model.Introduction;
                //        EditUser.ExtendAspNetUser.Recommender = model.Recommender;
                //        EditUser.ExtendAspNetUser.CareerDuration = model.CareerDuration;
                //        _MembershipService.Save();
                //    }
                //    else
                //    {
                //        _MembershipService.AddExtendUser(new ExtendAspNetUser
                //        {
                //            UserID = EditUser.Id,
                //            KorName = model.FirstName + " " + model.LastName,
                //            EngName = model.FirstName + " " + model.LastName,
                //            SpecialtyType = model.SpecialtyType,
                //            SnsSite = model.SNSSite,
                //            CareerInfo = model.Career,
                //            CareerDuration = model.CareerDuration,
                //            SelfIntroduction = model.Introduction,
                //            Recommender = model.Recommender,
                //            RegistrationDate = DateTime.Now,
                //            LastLoginDate = DateTime.Now,
                //            SystemAdmin = false
                //        });
                //    }
                //    if (model.Photo != null)
                //    {
                //        _UsersPhotoService.AddUsersPhoto(new UsersPhoto
                //        {
                //            UserID = EditUser.Id,
                //            UserPhtoFileLocationPath = Util.CreateUPhoto(EditUser.Id, model.Photo),
                //            UserPhtoFileName = model.Photo.FileName,
                //            UserPhtoFileSize = model.Photo.ContentLength,
                //            UserPhtoFileType = model.Photo.ContentType
                //        });
                //    }
                //    return RedirectToAction("Index");
                //}
                //ViewBag.IsAdmin = currentUser.ExtendAspNetUser != null && currentUser.ExtendAspNetUser.SystemAdmin == true;
                //InitDropDownlist();
                //ViewBag.ShowPopUp = true;
                //return View(model);
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
                        return RedirectToAction("Index");
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
                        return RedirectToAction("Index", "Home");
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
                return RedirectToAction("Index", "Member");
            }  
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
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
    }
}