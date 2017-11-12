using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HBK.Models
{
    [Table("tblCommentAttachment")]
    public class CommentAttachment
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ComentAttachID { get; set; }

        [Required]
        [MaxLength(300)]
        public string CommentFileName { get; set; }

        [Required]
        public int CommentFileSize { get; set; }

        [Required]
        [MaxLength(100)]
        public string CommentFileType { get; set; }

        [Required]
        [MaxLength(1000)]
        public string ComentFileLocationPath { get; set; }

        //0: images / 1: other file (doc,xml, xls, ppt…..)
        [Required]
        public bool CommentImgOrOther { get; set; }

        [ForeignKey("CommunityComment")]
        public int ComentID { get; set; }

        public virtual CommunityComment CommunityComment { get; set; }
    }
}