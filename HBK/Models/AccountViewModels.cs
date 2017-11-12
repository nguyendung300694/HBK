using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace HBK.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string ReturnUrl { get; set; }
    }

    public class SendCodeViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
        public string ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
    }

    public class VerifyCodeViewModel
    {
        [Required]
        public string Provider { get; set; }

        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }
        public string ReturnUrl { get; set; }

        [Display(Name = "Remember this browser?")]
        public bool RememberBrowser { get; set; }

        public bool RememberMe { get; set; }
    }

    public class ForgotViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        public string Language { get; set; }
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        [Remote("DoesEmailExist", "Account", HttpMethod = "POST", ErrorMessage = "Email already exists. Please enter a different Email.")]
        public string Email { get; set; }

        //[StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        //[RegularExpression(@"^(?=.{6,})(?=.*[a-z])(?=.*[A-Z])(?=.*[@#$%^&+=]).*$")]
        [Required]
        [MembershipPassword(
            MinRequiredNonAlphanumericCharacters = 1,
            MinNonAlphanumericCharactersError = "Your password needs to contain at least one symbol (!, @, #, etc).",
            ErrorMessage = "Your password must be 6 characters long and contain at least one symbol (!, @, #, etc).",
            MinRequiredPasswordLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        //[RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Not a valid Phone number")]
        public string Tel { get; set; }

        public string Address { get; set; }
        [Required]
        public string Country { get; set; }

        [Required]
        public HttpPostedFileBase Photo { get; set; }

        [Required]
        public string MainSpecialtyType { get; set; }

        [Required]
        public string SpecialtyType { get; set; }

        public string SNSSite { get; set; }

        [Required]
        public string Recommender { get; set; }

        [Required]
        public string Introduction { get; set; }

        [Required]
        public string Career { get; set; }

        [Required]
        public int CareerDuration { get; set; }
    }

    public class MyInformationViewModel
    {
        public EditAccountViewModel EditAccount { get; set; }
        public ChangePasswordtViewModel ChangePassword { get; set; }
        public bool IsEditing { get; set; }// = true;
    }

    public class ChangePasswordtViewModel
    {
        [Required]
        [MembershipPassword(
           MinRequiredNonAlphanumericCharacters = 1,
           MinNonAlphanumericCharactersError = "Your password needs to contain at least one symbol (!, @, #, etc).",
           ErrorMessage = "Your password must be 6 characters long and contain at least one symbol (!, @, #, etc).",
           MinRequiredPasswordLength = 6)]
        public string OldPassword { get; set; }

        [Required]
        [MembershipPassword(
            MinRequiredNonAlphanumericCharacters = 1,
            MinNonAlphanumericCharactersError = "Your password needs to contain at least one symbol (!, @, #, etc).",
            ErrorMessage = "Your password must be 6 characters long and contain at least one symbol (!, @, #, etc).",
            MinRequiredPasswordLength = 6)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class EditAccountViewModel
    {
        public string Email { get; set; }

        //[Required]
        //[MembershipPassword(
        //    MinRequiredNonAlphanumericCharacters = 1,
        //    MinNonAlphanumericCharactersError = "Your password needs to contain at least one symbol (!, @, #, etc).",
        //    ErrorMessage = "Your password must be 6 characters long and contain at least one symbol (!, @, #, etc).",
        //    MinRequiredPasswordLength = 6)]
        //public string Password { get; set; }

        //[DataType(DataType.Password)]
        //[System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        //public string ConfirmPassword { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string Tel { get; set; }

        public string Address { get; set; }
        [Required]
        public string Country { get; set; }
        public string OldPhoto { get; set; }

        public HttpPostedFileBase Photo { get; set; }

        [Required]
        public string MainSpecialtyType { get; set; }

        [Required]
        public string SpecialtyType { get; set; }

        public string SNSSite { get; set; }

        [Required]
        public string Recommender { get; set; }

        [Required]
        public string Introduction { get; set; }

        [Required]
        public string Career { get; set; }

        [Required]
        public int CareerDuration { get; set; }

        public IEnumerable<dynamic> ListSpecialtyType { get; set; }
    }

    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}
