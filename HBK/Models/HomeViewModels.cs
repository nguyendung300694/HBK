using System;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;

namespace HBK.Models
{
    public class HomeViewModels
    {
    }

    public partial class HomeSilde
    {
        public string Description { get; set; }

        public string ImageURL { get; set; }
        public string Title { get; set; }
    }

    public partial class CommunicationViewModel
    {
        public int CommunicationId { get; set; }
        public string Title { get; set; }

        public string Writer { get; set; }

        public string ImageURL { get; set; }
        public DateTime Date { get; set; }

        public int CommentCount { get; set; }
        public int ViewCount { get; set; }
    }

    
    public class CommunityViewModel
    {
        public int ImageDisplayID { get; set; }
        public int CommID { get; set; }

        [Required]
        public string CommTitle { get; set; }

        [Required]
        public string CommContent { get; set; }

        public HttpPostedFileBase[] Images { get; set; }
        public HttpPostedFileBase[] Files { get; set; }
        public HttpPostedFileBase ImageDisPlayed { get; set; }
    }
}