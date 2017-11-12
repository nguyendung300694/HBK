using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace HBK.Models
{
    [Table("tblProject")]
    public class Project
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProjectID { get; set; }

        [ForeignKey("Community")]
        public int CommID { get; set; }

        [Required]
        [MaxLength(300)]
        public string ProjectName { get; set; }

        //[Required]
        [MaxLength(6)]
        [ForeignKey("CategoryObj")]
        public string Category { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        //[Required]
        [MaxLength(6)]
        [ForeignKey("ProjectSizeObj")]
        public string ProjectSize { get; set; }


        //[Required]
        [MaxLength(6)]
        [ForeignKey("ProjectStatusObj")]
        public string ProjectStatus { get; set; }

        [ForeignKey("Company")]
        public int CompnayID { get; set; }

        [Required]
        [Column(TypeName = "ntext")]
        [MaxLength]
        public string ProjectContents { get; set; }

        public DateTime RegDate { get; set; }

        public DateTime? RegEdit { get; set; }

        [Required]
        [MaxLength(128)]
        public string RegUser { get; set; }

        public virtual Community Community { get; set; }
        public virtual Common CategoryObj { get; set; }
        public virtual Common ProjectSizeObj { get; set; }
        public virtual Common ProjectStatusObj { get; set; }
        public virtual Company Company { get; set; }

        public virtual ICollection<ProjectAttachment> ProjectAttachments { get; set; }
        public virtual ICollection<ProjectComment> ProjectComments { get; set; }
        public virtual ICollection<ProjectMember> ProjectMembers { get; set; }
    }
}