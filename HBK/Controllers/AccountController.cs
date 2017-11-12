using HBK.Models;
using HBK.Services;
using Facebook;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace HBK.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        private readonly ICommonService _CommonService;
        private readonly IMembershipService _MembershipService;
        private readonly IUsersPhotoService _UsersPhotoService;

        public AccountController(ICommonService CommonService, IMembershipService MembershipService, IUsersPhotoService UsersPhotoService)
        {
            _CommonService = CommonService;
            _MembershipService = MembershipService;
            _UsersPhotoService = UsersPhotoService;
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

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

        [HttpPost]
        [AllowAnonymous]
        public JsonResult DoesEmailExist(string email)
        {
            var user = _MembershipService.GetUserByEmail(email);
            return Json(user == null);
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: true);
            switch (result)
            {
                case SignInStatus.Success:
                    _MembershipService.UpdateLastLogin(model.Email);
                    return RedirectToLocal(returnUrl, model.Language);

                case SignInStatus.LockedOut:
                    return View("Lockout");

                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });

                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", App_GlobalResources.Langues.InvalidLogin);
                    return View(model);
            }
        }

        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes.
            // If a user enters incorrect codes for a specified amount of time then the user account
            // will be locked out for a specified amount of time.
            // You can configure the account lockout settings in IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent: model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);

                case SignInStatus.LockedOut:
                    return View("Lockout");

                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            InitDropDownlist(null);
            return View();
        }

        private void InitDropDownlist(string MainSpecialtyType)
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
            if (!string.IsNullOrEmpty(MainSpecialtyType))
            {
                ViewBag.ListSubType = _CommonService.getCommonById(MainSpecialtyType).ChildCommom.Select(c => new SelectListItem
                {
                    Text = c.ComName,
                    Value = c.ComCode
                });
            }
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
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
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

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
                    return RedirectToAction("Index", "Home");
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            InitDropDownlist(null);
            return View(model);
        }

        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                // string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                // var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                // await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
                // return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {

            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            if (loginInfo.Login.LoginProvider == "Facebook")
            {
                var identity = AuthenticationManager.GetExternalIdentity(DefaultAuthenticationTypes.ExternalCookie);
                var access_token = identity.FindFirstValue("FacebookAccessToken");
                var fb = new FacebookClient(access_token);
                dynamic myInfo = fb.Get("/me?fields=email"); // specify the email field
                loginInfo.Email = myInfo.email;
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);

                case SignInStatus.LockedOut:
                    return View("Lockout");

                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });

                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        [AllowAnonymous]
        public JsonResult GetSpecialtyType(string MainSpecialtyType)
        {
            if (!string.IsNullOrEmpty(MainSpecialtyType))
            {
                var SpecialtyTypes = _CommonService.getCommonById(MainSpecialtyType).ChildCommom;
                if (SpecialtyTypes.Any())
                {
                    return Json(SpecialtyTypes.Select(c => new { value = c.ComCode, text = c.ComName }), JsonRequestBehavior.AllowGet);
                }
            }
            return Json(null, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        public ActionResult MyInfors()
        {
            ApplicationUser currentUser = _MembershipService.GetUserById(User.Identity.GetUserId());
            var model = new MyInformationViewModel
            {
                EditAccount = new EditAccountViewModel
                {
                    Email = currentUser.Email,
                    FirstName = currentUser.FirstName,
                    LastName = currentUser.LastName,
                    Tel = currentUser.PhoneNumber,
                    Address = currentUser.Address ?? "",
                    Country = currentUser.Country,
                    OldPhoto = getAvatar(currentUser),
                    MainSpecialtyType = currentUser.ExtendAspNetUser != null ? currentUser.ExtendAspNetUser.Common.ParentCommom.ComCode : string.Empty,
                    SpecialtyType = currentUser.ExtendAspNetUser != null ? currentUser.ExtendAspNetUser.SpecialtyType : string.Empty,
                    SNSSite = currentUser.ExtendAspNetUser != null ? currentUser.ExtendAspNetUser.SnsSite : string.Empty,
                    Recommender = currentUser.ExtendAspNetUser != null ? currentUser.ExtendAspNetUser.Recommender : string.Empty,
                    Introduction = currentUser.ExtendAspNetUser != null ? currentUser.ExtendAspNetUser.SelfIntroduction : string.Empty,
                    Career = currentUser.ExtendAspNetUser != null ? currentUser.ExtendAspNetUser.CareerInfo : string.Empty,
                    CareerDuration = currentUser.ExtendAspNetUser != null ? currentUser.ExtendAspNetUser.CareerDuration : 0,
                },
                IsEditing = true
            };
            InitDropDownlist(model.EditAccount.MainSpecialtyType);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MyInfors(MyInformationViewModel model)
        {
            if (model.IsEditing)
            {
                if (ModelState.ContainsKey("ChangePassword.OldPassword"))
                    ModelState["ChangePassword.OldPassword"].Errors.Clear();
                if (ModelState.ContainsKey("ChangePassword.Password"))
                    ModelState["ChangePassword.Password"].Errors.Clear();
                if (ModelState.IsValid)
                {
                    ApplicationUser EditUser = _MembershipService.GetUserById(User.Identity.GetUserId());
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
                    return RedirectToAction("Index", "Home");
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
            InitDropDownlist(model.EditAccount.MainSpecialtyType);
            return View(model);
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

        #region Helpers

        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        private ActionResult RedirectToLocal(string returnUrl, string language)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home", new { @lang = language });
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }

        #endregion Helpers
    }
}