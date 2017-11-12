using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace HBK.Models
{
    [Table("tblProjectAttachment")]
    public class ProjectAttachment
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required]
        [MaxLength(300)]
        public string FileName { get; set; }

        [Required]
        public int FileSize { get; set; }

        [Required]
        [MaxLength(100)]
        public string FileType { get; set; }


        [Required]
        [MaxLength(1000)]
        public string FileLocationPath { get; set; }

        [Required]//0: Not display / 1: Main Display
        public bool DisplayImgYn { get; set; }


        [Required]//0: images / 1: other file (doc,xml, xls, ppt…..)

        public bool ImgOrOther { get; set; }

        [MaxLength(500)]
        public string AttachComment { get; set; }

        [ForeignKey("Project")]
        public int ProjectID { get; set; }

        public virtual Project Project { get; set; }

    }
}