using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HBK.Models
{
    [Table("tblCommunity")]
    public class Community
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CommID { get; set; }

        [Required]
        [MaxLength(1000)]
        public string CommTitle { get; set; }

        [Required]
        [Column(TypeName = "ntext")]
        [MaxLength]
        public string ComContent { get; set; }

        [Required]
        public DateTime InsertDate { get; set; }

        public DateTime? EditDate { get; set; }

        [Required]
        [MaxLength(128)]
        public string AuthorUserID { get; set; }

        [Required]
        [MaxLength(128)]
        public string AuthorUserName { get; set; }

        [Required]
        [DefaultValue(0)]
        public int Hits { get; set; }

        public virtual ICollection<CommunityAttachment> CommunityAttachments { get; set; }
        public virtual ICollection<CommunityComment> CommunityComments { get; set; }
    }
}