using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace HBK.Models
{
    [Table("tbProjectComment")]
    public class ProjectComment
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProjectCommentID { get; set; }

        [Required]
        [Column(TypeName = "ntext")]
        [MaxLength]
        public string ComentContent { get; set; }

        [Required]
        public DateTime ComentTime { get; set; }

        [Required]
        [MaxLength(128)]
        public string ComentUserID { get; set; }

        [ForeignKey("Project")]
        public int ProjectID { get; set; }

        public virtual Project Project { get; set; }

    }
}