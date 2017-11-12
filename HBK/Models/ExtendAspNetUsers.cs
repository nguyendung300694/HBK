using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HBK.Models
{
    [Table("ExtendAspNetUsers")]
    public class ExtendAspNetUser
    {
        [Key, ForeignKey("ApplicationUser")]
        [MaxLength(128)]
        public string UserID { get; set; }

        [Required]
        [MaxLength(150)]
        public string KorName { get; set; }

        [Required]
        [MaxLength(200)]
        public string EngName { get; set; }

        [Required]
        [MaxLength(6)]
        [ForeignKey("Common")]
        public string SpecialtyType { get; set; }

        [MaxLength(200)]
        public string SnsSite { get; set; }

        [Required]
        [MaxLength(200)]
        public string CareerInfo { get; set; }

        [Required]
        public int CareerDuration { get; set; }

        [Required]
        [Column(TypeName = "ntext")]
        [MaxLength]
        public string SelfIntroduction { get; set; }

        [Required]
        [MaxLength(128)]
        public string Recommender { get; set; }

        [Required]
        public DateTime RegistrationDate { get; set; }

        [Required]
        public DateTime LastLoginDate { get; set; }

        [Required]//0: User, 1: Administrator
        public bool SystemAdmin { get; set; }

        public virtual Common Common { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }
        public virtual ICollection<UsersPhoto> UsersPhotos { get; set; }
    }
}