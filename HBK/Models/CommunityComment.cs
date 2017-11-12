using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HBK.Models
{
    [Table("tblCommunityComment")]
    public class CommunityComment
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ComentID { get; set; }

        [Required]
        [Column(TypeName = "ntext")]
        [MaxLength]
        public string ComentContent { get; set; }

        [Required]
        public DateTime ComentTime { get; set; }

        [Required]
        [MaxLength(128)]
        public string ComentUserID { get; set; }

        [ForeignKey("Community")]
        public int CommID { get; set; }

        public virtual Community Community { get; set; }

        public virtual ICollection<CommentAttachment> CommentAttachments { get; set; }
    }
}