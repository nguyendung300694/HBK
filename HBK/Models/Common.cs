using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace HBK.Models
{
    [Table("tblCommon")]
    public class Common
    {
        [Key]
        [MaxLength(6)]
        public string ComCode { get; set; }

        [MaxLength(6)]
        public string ComSubCode { get; set; }

        [Required]
        [DefaultValue(0)]
        public int CommonType { get; set; }

        [Required]
        [MaxLength(50)]
        public string ComName { get; set; }

        [Required]
        [MaxLength(50)]
        public string ComName2 { get; set; }


        [ForeignKey("ComSubCode")]
        public virtual Common ParentCommom { get; set; }
        public virtual ICollection<Common> ChildCommom { get; set; }
    }

    public class CommonModelView
    {
        [Required]
        [MaxLength(6)]
        public string ComCode{ get; set; }
        [UIHint("ComSubCode")]
        public string ComSubCode { get; set; }
        public bool IsChange { get; set; }
        public int CommonType { get; set; }
        public string TempComSubCode { get; set; }
        [Required]
        [MaxLength(50)]
        public string ComName { get; set; }
        [Required]
        [MaxLength(50)]
        public string ComName2 { get; set; }
    }
}