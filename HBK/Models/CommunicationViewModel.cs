using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace HBK.Models
{
    public class CommunicationDetailsViewModel
    {
        public int CommuId { get; set; }
        public string ImageURL { get; set; }
        public string Information { get; set; }
        public bool IsModifier { get; set; }

        public List<CommentViewModel> Commments { get; set; }
        public List<CommentAttachmentViewModel> CommentAttachments { get; set; }
        public List<CommunicationAttachmentViewModel> CommunicationAttachments { get; set; }

        public HttpPostedFileBase NewCommentAttachment { get; set; }

        [Required]
        public string NewComment { get; set; }

        public string AuthorId { get; set; }
    }

    public class CommentViewModel
    {
        public int CommeId { get; set; }
        public string Name { get; set; }
        public string Avatar { get; set; }
        public string Content { get; set; }
        public string Time { get; set; }
        public bool IsModifier { get; set; }
    }

    public class CommentAttachmentViewModel
    {
        public string DownloadUrl { get; set; }
        public string Filename { get; set; }
        public int FileID { get; set; }
    }

    public class CommunicationAttachmentViewModel
    {
        public string Url { get; set; }
        public string Filename { get; set; }
        public bool isFile { get; set; }
        public int FileSize { get; set; }
        public int FileID { get; set; }
    }

    public class EditCommunityViewModel
    {
        public int CommID { get; set; }
        public int ImageDisplayID { get; set; }
        public string ImageDisplay { get; set; }
        public string CommTitle { get; set; }
        public string CommContent { get; set; }
        public List<CommunicationAttachmentViewModel> Images { get; set; }
        public List<CommunicationAttachmentViewModel> Files { get; set; }
    }
}