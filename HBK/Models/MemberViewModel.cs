using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HBK.Models
{
    public class MemberViewModel : RegisterViewModel
    {
        public bool IsModifier { get; set; }
        public int MyProperty { get; set; }
        public bool IsLocked { get; set; }
        public string Fullname { get; set; }
        public string Avatar { get; set; }
        public string Id { get; set; }
        public DateTime LastLoginDate { get; set; }
        public DateTime RegistrationDate { get; set; }
        public string ProjectRole { get; set; }
        public string ProjectPercent { get; set; }
        public string MemberId { get; set; }
    }

    public class MemberInformationViewModel
    {
        public string Id { get; set; }
        [Required]
        [EmailAddress]
        [Remote("DoesEmailExist", "Account", HttpMethod = "POST", ErrorMessage = "Email already exists. Please enter a different Email.")]
        public string Email { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public bool IsChange { get; set; }
        public string LastName { get; set; }
        public string SpecialtyTypeCode { get; set; }
        [Required]
        [UIHint("SpecialTypeEditor")]
        public string SpecialtyType { get; set; }
        [Required]
        public string Career { get; set; }
        [Required]
        public int CareerDuration { get; set; }
        [Required]
        public string Recommender { get; set; }
        public DateTime LastLoginDate { get; set; }
    }

    public class SpecialTypeItemViewModel
    {
        public string CommCode { get; set; }
        public string CommName { get; set; }
    }
}