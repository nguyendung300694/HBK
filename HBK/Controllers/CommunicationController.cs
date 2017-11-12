using HBK.Models;
using HBK.Services;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace HBK.Controllers
{
    [Authorize]
    public class CommunicationController : Controller
    {
        private readonly ICommunityService _CommunityService;
        private readonly IMembershipService _MembershipService;
        private readonly ICommunityAttachmentService _CommunityAttachmentService;

        public CommunicationController(ICommunityService communityService, IMembershipService membershipService, ICommunityAttachmentService communityAttachmentService)
        {
            _CommunityService = communityService;
            _MembershipService = membershipService;
            _CommunityAttachmentService = communityAttachmentService;
        }
        // GET: Communication
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateCommunication(CommunityViewModel model)
        {
            if (ModelState.IsValid)
            {
                //CommunityViewModel a = new CommunityViewModel
                //{
                //    CommID = model.CommID,
                //    CommTitle = model.CommTitle,
                //    CommContent = model.CommContent,
                //    ImageDisPlayed = model.ImageDisPlayed,
                //    Images = model.Images,
                //    Files = model.Files
                //};

                Community comm = _CommunityService.findCommunityById(model.CommID);
                comm.CommTitle = model.CommTitle;
                comm.ComContent = model.CommContent;
                comm.EditDate = DateTime.Now;
                _CommunityService.SaveChange();

                if (model.ImageDisPlayed != null)
                {
                    var communityAttachment = _CommunityService.findCommunityAttachmentById(model.ImageDisplayID);
                    _CommunityService.DeleteCommunityAttachment(communityAttachment);

                    CommunityAttachment Attachment = new CommunityAttachment
                    {
                        FileName = model.ImageDisPlayed.FileName,
                        FileSize = model.ImageDisPlayed.ContentLength,
                        FileType = model.ImageDisPlayed.ContentType,
                        FileLocationPath = Util.CreateCommunityAttachment(User.Identity.GetUserId(), model.ImageDisPlayed),
                        DisplayImgYn = true,
                        ImgOrOther = false,
                        AttachComment = string.Empty,
                        CommID = comm.CommID
                    };
                    _CommunityAttachmentService.AddCommunityAttachment(Attachment);
                }
                if (model.Files != null && model.Files[0] != null)
                {
                    for (int i = 0; i < model.Files.Length; i++)
                    {
                        CommunityAttachment Attachment = new CommunityAttachment
                        {
                            FileName = model.Files[i].FileName,
                            FileSize = model.Files[i].ContentLength,
                            FileType = model.Files[i].ContentType,
                            FileLocationPath = Util.CreateCommunityAttachment(User.Identity.GetUserId(), model.Files[i]),
                            DisplayImgYn = false,
                            ImgOrOther = true,
                            AttachComment = string.Empty,
                            CommID = comm.CommID
                        };
                        _CommunityAttachmentService.AddCommunityAttachment(Attachment);
                    }
                }
                if (model.Images != null && model.Images[0] != null)
                {
                    for (int i = 0; i < model.Images.Length; i++)
                    {
                        CommunityAttachment Attachment = new CommunityAttachment
                        {
                            FileName = model.Images[i].FileName,
                            FileSize = model.Images[i].ContentLength,
                            FileType = model.Images[i].ContentType,
                            FileLocationPath = Util.CreateCommunityAttachment(User.Identity.GetUserId(), model.Images[i]),
                            DisplayImgYn = false,
                            ImgOrOther = false,
                            AttachComment = string.Empty,
                            CommID = comm.CommID
                        };
                        _CommunityAttachmentService.AddCommunityAttachment(Attachment);
                    }
                }
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        public ActionResult CommunicationDetails(int id, string From)
        {
            var Community = _CommunityService.increaseHits(id);
            ViewBag.From = From;
            if (Community != null)
            {
                ApplicationUser currentUser = _MembershipService.GetUserById(User.Identity.GetUserId());

                List<CommentViewModel> ListCommments = new List<CommentViewModel>();
                List<CommentAttachmentViewModel> ListCommentAttachments = new List<CommentAttachmentViewModel>();
                foreach (CommunityComment item in Community.CommunityComments)
                {
                    var writer = _MembershipService.GetUserById(item.ComentUserID);
                    ListCommments.Add(new CommentViewModel
                    {
                        CommeId = item.ComentID,
                        Avatar = getAvatar(writer),
                        Name = writer.FirstName + " " + writer.LastName,
                        Content = item.ComentContent,
                        Time = item.ComentTime.ToString("yyyy-MM-dd HH:mm:ss"),
                        IsModifier = CheckPermission(currentUser, item.ComentUserID),
                    });
                    var commentAttachment = item.CommentAttachments.FirstOrDefault();
                    if (commentAttachment != null)
                    {
                        ListCommentAttachments.Add(new CommentAttachmentViewModel
                        {
                            DownloadUrl = commentAttachment.ComentFileLocationPath,
                            Filename = commentAttachment.CommentFileName,
                            FileID = commentAttachment.ComentAttachID
                        });
                    }
                }
                CommunicationDetailsViewModel model = new CommunicationDetailsViewModel
                {
                    CommuId = Community.CommID,
                    ImageURL = GetMainImageURL(Community.CommunityAttachments),
                    Information = HttpUtility.HtmlDecode(Community.ComContent),
                    IsModifier = CheckPermission(currentUser, Community.AuthorUserID),
                    Commments = ListCommments,
                    CommentAttachments = ListCommentAttachments,
                    CommunicationAttachments = getCommunityAttachments(Community.CommunityAttachments)
                };
                return View(model);
            }
            return View();
        }

        [HttpPost]
        public JsonResult DeleteCommunityAttachment(int id)
        {
            try
            {
                var Community = _CommunityService.findCommunityByIdOfCommunityAttachment(id);
                var CommunityAttachment = _CommunityService.findCommunityAttachmentById(id);
                if (Community != null)
                {
                    ApplicationUser currentUser = _MembershipService.GetUserById(User.Identity.GetUserId());
                    if (CheckPermission(currentUser, Community.AuthorUserID))
                    {
                        Util.DeleteFileLocal(CommunityAttachment.FileLocationPath);
                        _CommunityService.DeleteCommunityAttachment(CommunityAttachment);
                        return Json(true);
                    }
                    else
                    {
                        return Json(false);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return Json(false);
        }

        public ActionResult Edit(int id)
        {
            var Community = _CommunityService.findCommunityById(id);
            if (Community != null)
            {
                ApplicationUser currentUser = _MembershipService.GetUserById(User.Identity.GetUserId());
                if (CheckPermission(currentUser, Community.AuthorUserID))
                {
                    List<CommunicationAttachmentViewModel> Images = new List<CommunicationAttachmentViewModel>();
                    List<CommunicationAttachmentViewModel> Files = new List<CommunicationAttachmentViewModel>();
                    string ImageDisplay = string.Empty;
                    int ImageDisplayID = 0;
                    foreach (var item in Community.CommunityAttachments)
                    {
                        if (item.DisplayImgYn == false && item.ImgOrOther == false)
                        {
                            Images.Add(new CommunicationAttachmentViewModel
                            {
                                FileID = item.ID,
                                Url = item.FileLocationPath,
                            });
                        }
                        else if (item.DisplayImgYn == false && item.ImgOrOther == true)
                        {
                            Files.Add(new CommunicationAttachmentViewModel
                            {
                                FileID = item.ID,
                                Url = item.FileLocationPath,
                                Filename = item.FileName,
                                FileSize = item.FileSize
                            });
                        }
                        else
                        {
                            ImageDisplayID = item.ID;
                            ImageDisplay = item.FileLocationPath;
                        }

                    }

                    EditCommunityViewModel model = new EditCommunityViewModel
                    {
                        CommID = Community.CommID,
                        ImageDisplay = ImageDisplay,
                        CommTitle = Community.CommTitle,
                        CommContent = HttpUtility.HtmlDecode(Community.ComContent),
                        Files = Files,
                        Images = Images,
                        ImageDisplayID = ImageDisplayID
                    };

                    return View(model);

                }
                else
                {
                    return View("Index");
                }
            }
            return View("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult WriteNewComment(CommunicationDetailsViewModel communication)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser currentUser = _MembershipService.GetUserById(User.Identity.GetUserId());
                communication.AuthorId = User.Identity.GetUserId();
                var NewComment = _CommunityService.WriteNewComment(communication);
                var writer = _MembershipService.GetUserById(NewComment.ComentUserID);
                CommentViewModel CommentModel = new CommentViewModel
                {
                    CommeId = NewComment.ComentID,
                    Avatar = getAvatar(writer),
                    Name = writer.FirstName + " " + writer.LastName,
                    Content = NewComment.ComentContent,
                    Time = NewComment.ComentTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    IsModifier = CheckPermission(currentUser, NewComment.ComentUserID),
                };

                CommentAttachmentViewModel CommentAttachModel = null;

                if (NewComment.CommentAttachments != null)
                {
                    var NewCommentAttach = NewComment.CommentAttachments.FirstOrDefault();
                    if (NewCommentAttach != null)
                    {
                        CommentAttachModel = new CommentAttachmentViewModel
                        {
                            DownloadUrl = NewCommentAttach.ComentFileLocationPath,
                            Filename = NewCommentAttach.CommentFileName,
                            FileID = NewCommentAttach.ComentAttachID
                        };
                    }
                }

                return Json(new
                {
                    Comment = RenderPartialViewToString(this, "_CommentLayout", CommentModel),
                    CommentAttach = CommentAttachModel
                });
            }
            return Json(null);
        }

        public static string RenderPartialViewToString(Controller thisController, string viewName, object model)
        {
            // assign the model of the controller from which this method was called to the instance of the passed controller (a new instance, by the way)
            thisController.ViewData.Model = model;

            // initialize a string builder
            using (StringWriter sw = new StringWriter())
            {
                // find and load the view or partial view, pass it through the controller factory
                ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(thisController.ControllerContext, viewName);
                ViewContext viewContext = new ViewContext(thisController.ControllerContext, viewResult.View, thisController.ViewData, thisController.TempData, sw);

                // render it
                viewResult.View.Render(viewContext, sw);

                //return the razorized view/partial-view as a string
                return sw.ToString();
            }
        }

        //public string RenderRazorViewToString(string viewName, object model)
        //{
        //    ViewData.Model = model;
        //    using (var sw = new StringWriter())
        //    {
        //        var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext,
        //                                                                 viewName);
        //        var viewContext = new ViewContext(ControllerContext, viewResult.View,
        //                                     ViewData, TempData, sw);
        //        viewResult.View.Render(viewContext, sw);
        //        viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);
        //        return sw.GetStringBuilder().ToString();
        //    }
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult WriteNewComment(CommunicationDetailsViewModel communication)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        communication.AuthorId = User.Identity.GetUserId();
        //        _CommunityService.WriteNewComment(communication);
        //        return RedirectToAction("CommunicationDetails", new { @id = communication.CommuId, @From = "Comment" });
        //    }
        //    return View("CommunicationDetails", communication);
        //}

        [HttpPost]
        public JsonResult Delete(int id)
        {
            try
            {
                Community Community = _CommunityService.findCommunityById(id);
                if (Community != null)
                {
                    ApplicationUser currentUser = _MembershipService.GetUserById(User.Identity.GetUserId());
                    if (CheckPermission(currentUser, Community.AuthorUserID))
                    {
                        foreach (var CommeAttach in Community.CommunityComments)
                        {
                            foreach (var item in CommeAttach.CommentAttachments)
                            {
                                Util.DeleteFileLocal(item.ComentFileLocationPath);
                            }

                        }
                        foreach (var CommuAttach in Community.CommunityAttachments)
                        {
                            Util.DeleteFileLocal(CommuAttach.FileLocationPath);
                        }
                        _CommunityService.Delete(Community);
                        return Json(true);
                    }
                    else
                    {
                        return Json(false);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return Json(false);
        }


        [HttpPost]
        public JsonResult EditComment(int id, string content)
        {
            try
            {
                CommunityComment Comment = _CommunityService.findCommentById(id);
                if (Comment != null)
                {
                    ApplicationUser currentUser = _MembershipService.GetUserById(User.Identity.GetUserId());
                    if (CheckPermission(currentUser, Comment.ComentUserID))
                    {
                        _CommunityService.EditComment(id, content);
                        return Json(true);
                    }
                    else
                    {
                        return Json(false);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return Json(false);
        }

        [HttpPost]
        public JsonResult DeleteComment(int id)
        {
            try
            {
                CommunityComment Comment = _CommunityService.findCommentById(id);
                if (Comment != null)
                {
                    ApplicationUser currentUser = _MembershipService.GetUserById(User.Identity.GetUserId());
                    if (CheckPermission(currentUser, Comment.ComentUserID))
                    {
                        CommentAttachmentViewModel CommentAttachModel = null;

                        if (Comment.CommentAttachments.Count > 0)
                        {
                            var CommentAttach = Comment.CommentAttachments.FirstOrDefault();
                            CommentAttachModel = new CommentAttachmentViewModel
                            {
                                FileID = CommentAttach.ComentAttachID
                            };
                        }

                        foreach (var Attachment in Comment.CommentAttachments)
                        {
                            Util.DeleteFileLocal(Attachment.ComentFileLocationPath);
                        }

                        _CommunityService.DeleteComment(Comment);

                        return Json(new { success = true, CommentAttach = CommentAttachModel });
                        //return Json(new { success = true, RedirectURL = Url.Action("CommunicationDetails", new { @id = Comment.CommID, @From = "Comment" }) });
                    }
                    else
                    {
                        return Json(new { success = false });
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return Json(new { success = false });
        }

        #region Communication Details Helper
        private string GetMainImageURL(ICollection<CommunityAttachment> firstQuery)
        {
            if (firstQuery.Any() && firstQuery.Count > 0)
            {
                var queryResult = firstQuery.Where(a => a.DisplayImgYn == true && a.ImgOrOther == false).FirstOrDefault();
                return queryResult != null ? queryResult.FileLocationPath : Util.DefaultImg();
            }
            else
            {
                return Util.DefaultImg();
            }
        }

        private bool CheckPermission(ApplicationUser currentUser, string writerId)
        {

            if (currentUser.Id.Equals(writerId))
            {
                return true;
            }
            else
            {
                return currentUser.ExtendAspNetUser != null && currentUser.ExtendAspNetUser.SystemAdmin;
            }
        }

        private List<CommunicationAttachmentViewModel> getCommunityAttachments(ICollection<CommunityAttachment> CommunityAttachments)
        {
            List<CommunicationAttachmentViewModel> ListCommunityAttachments = new List<CommunicationAttachmentViewModel>();
            foreach (var item in CommunityAttachments)
            {
                if (item.DisplayImgYn == false)
                {
                    ListCommunityAttachments.Add(new CommunicationAttachmentViewModel
                    {
                        Url = item.FileLocationPath,
                        Filename = item.FileName,
                        isFile = item.ImgOrOther,
                        FileSize = item.FileSize
                    });
                }
            }
            return ListCommunityAttachments;
        }

        private string getAvatar(ApplicationUser writer)
        {
            string avatar = Util.DefaultAvatar();
            var Extend = writer.ExtendAspNetUser;
            if (Extend != null)
            {
                var UsersPhoto = Extend.UsersPhotos.LastOrDefault();
                if (UsersPhoto != null)
                {
                    avatar = UsersPhoto.UserPhtoFileLocationPath;
                }
            }
            return avatar;
        }
        #endregion

    }

}