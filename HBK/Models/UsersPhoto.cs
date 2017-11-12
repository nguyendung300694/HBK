using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace HBK.Models
{
    [Table("tblUsersPhoto")]
    public class UsersPhoto
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserPhtoID { get; set; }

        [Required]
        [MaxLength(300)]
        public string UserPhtoFileName { get; set; }

        [Required]
        public int UserPhtoFileSize { get; set; }

        [Required]
        [MaxLength(100)]
        public string UserPhtoFileType { get; set; }

        [Required]
        [MaxLength(1000)]
        public string UserPhtoFileLocationPath { get; set; }

        [ForeignKey("ExtendAspNetUser")]
        [MaxLength(128)]
        public string UserID { get; set; }

        public virtual ExtendAspNetUser ExtendAspNetUser { get; set; }

    }
}