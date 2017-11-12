using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HBK.Models
{
    public partial class ProjectViewModel
    {
        public int ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string PMName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Status { get; set; }
        public int MemberCount { get; set; }
        public string ImageURL { get; set; }
    }

    public class ProjectDetailsViewModel
    {
        public int PrjctId { get; set; }
        public string ImageURL { get; set; }
        public string ProjectName { get; set; }
        public string ProjectPeriod { get; set; }
        public string Category { get; set; }
        public string PMName { get; set; }
        public string ClientName { get; set; }
        public string RegisterDate { get; set; }
        public string EditDate { get; set; }
        public string ProjectContents { get; set; }
        public bool IsModifier { get; set; }

        public List<CommentViewModel> Commments { get; set; }
        public List<CommentAttachmentViewModel> CommentAttachments { get; set; }
        public List<ProjectAttachmentViewModel> ProjectAttachments { get; set; }

        public HttpPostedFileBase NewCommentAttachment { get; set; }

        [Required]
        public string NewComment { get; set; }

        public string AuthorId { get; set; }
    }

    public class ProjectAttachmentViewModel
    {
        public string Url { get; set; }
        public string Filename { get; set; }
        public bool isFile { get; set; }
        public int FileSize { get; set; }
    }

    public partial class CreateProjectViewModel
    {
        public int PrjctId { get; set; }        
        public int CompanyId { get; set; }

        [Required]
        public string ProjectName { get; set; }

        [Required]
        public string StartDate { get; set; }

        public string EndDate { get; set; }

        [Required]
        public string Category { get; set; }

        [Required]
        public string ProjectSize { get; set; }

        [Required]
        public string Client { get; set; }

        [Required]
        public string ProjectStatus { get; set; }

        [Required]
        public HttpPostedFileBase[] ProjecAttachments { get; set; }

        public string ProjectContent { get; set; }
    }
}