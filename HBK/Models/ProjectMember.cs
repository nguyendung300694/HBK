using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace HBK.Models
{
    [Table("tblProjectMember")]
    public class ProjectMember
    {
        [Key, Column(Order = 0)]
        [MaxLength(128)]
        public string MemberID { get; set; }

        [Key, Column(Order = 1), ForeignKey("Project")]
        public int ProjectID { get; set; }

        [Required]
        [MaxLength(6)]
        [ForeignKey("ProjectRoleObj")]
        public string ProjectRole { get; set; }

        [Required]
        public decimal PercentInProject { get; set; }

        public virtual Common ProjectRoleObj { get; set; }
        public virtual Project Project { get; set; }
    }
}