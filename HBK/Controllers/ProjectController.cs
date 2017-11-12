using HBK.Models;
using HBK.Services;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HBK.Controllers
{
    [Authorize]
    public class ProjectController : Controller
    {
        private readonly IProjectService _ProjectService;
        private readonly ICommonService _CommonService;
        private readonly IMembershipService _MembershipService;

        public ProjectController(IProjectService ProjectService, ICommonService CommonService, IMembershipService MembershipService)
        {
            _ProjectService = ProjectService;
            _CommonService = CommonService;
            _MembershipService = MembershipService;
        }

        public ActionResult Index()
        {
            ViewBag.ListProjectStatus = _CommonService.getCommonById("ProSta").ChildCommom.Select(c => new SelectListItem
            {
                Text = c.ComName,
                Value = c.ComCode
            });
            //ViewBag.selectedProjectStatus = "Sta_Pl";
            return View();
        }

        public ActionResult GetProjects([DataSourceRequest] DataSourceRequest request)
        {
            return Json(_ProjectService.getAllProjects().ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public ActionResult ProjectDetails(int id, string From)
        {
            var Project = _ProjectService.findProjectById(id);
            ViewBag.From = From;
            if (Project != null)
            {
                ApplicationUser currentUser = _MembershipService.GetUserById(User.Identity.GetUserId());

                List<CommentViewModel> ListCommments = new List<CommentViewModel>();
                List<CommentAttachmentViewModel> ListCommentAttachments = new List<CommentAttachmentViewModel>();
                foreach (var item in Project.ProjectComments)
                {
                    var writer = _MembershipService.GetUserById(item.ComentUserID);
                    ListCommments.Add(new CommentViewModel
                    {
                        CommeId = item.ProjectCommentID,
                        Avatar = getAvatar(writer),
                        Name = writer.FirstName + " " + writer.LastName,
                        Content = item.ComentContent,
                        Time = item.ComentTime.ToString("yyyy-MM-dd HH:mm:ss"),
                        IsModifier = CheckPermission(currentUser, item.ComentUserID),
                    });
                    //var commentAttachment = item..FirstOrDefault();
                    //if (commentAttachment != null)
                    //{
                    //    ListCommentAttachments.Add(new CommentAttachmentViewModel
                    //    {
                    //        DownloadUrl = commentAttachment.ComentFileLocationPath,
                    //        Filename = commentAttachment.CommentFileName
                    //    });
                    //}
                }
                ProjectDetailsViewModel model = new ProjectDetailsViewModel
                {
                    PrjctId = Project.ProjectID,
                    ImageURL = GetMainImageURL(Project.ProjectAttachments),
                    ProjectName = Project.ProjectName,
                    ProjectPeriod = (Project.StartDate.HasValue ? Project.StartDate.Value.ToString("yyyy-MM-dd") : string.Empty) + " ~ " + (Project.EndDate.HasValue ? Project.EndDate.Value.ToString("yyyy-MM-dd") : string.Empty),
                    Category = Project.CategoryObj.ComName,
                    PMName = GetPMName(Project.ProjectMembers),
                    ClientName = Project.Company.CompanyName,
                    RegisterDate = Project.RegDate.ToString("yyyy-MM-dd"),
                    EditDate = Project.RegEdit.HasValue ? Project.RegEdit.Value.ToString("yyyy-MM-dd") : string.Empty,
                    ProjectContents = Project.ProjectContents,
                    IsModifier = CheckPermissionForProject(currentUser, Project.ProjectMembers),
                    Commments = ListCommments,
                    CommentAttachments = ListCommentAttachments,
                    ProjectAttachments = getProjectAttachments(Project.ProjectAttachments)
                };
                ViewBag.From = "Infor";
                ViewBag.IsAdmin = currentUser.ExtendAspNetUser != null && currentUser.ExtendAspNetUser.SystemAdmin == true;
                InitDropDownlist();
                return View(model);
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult WriteNewComment(ProjectDetailsViewModel ProjectComment)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser currentUser = _MembershipService.GetUserById(User.Identity.GetUserId());
                ProjectComment.AuthorId = User.Identity.GetUserId();
                var NewProjectComment = _ProjectService.WriteNewComment(ProjectComment);
                var writer = _MembershipService.GetUserById(NewProjectComment.ComentUserID);
                CommentViewModel CommentModel = new CommentViewModel
                {
                    CommeId = NewProjectComment.ProjectCommentID,
                    Avatar = getAvatar(writer),
                    Name = writer.FirstName + " " + writer.LastName,
                    Content = NewProjectComment.ComentContent,
                    Time = NewProjectComment.ComentTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    IsModifier = CheckPermission(currentUser, NewProjectComment.ComentUserID),
                };
                return Json(new { Comment = RenderPartialViewToString(this, "_ProjectCommentLayout", CommentModel) });
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

        public ActionResult Edit(int id)
        {
            var project = _ProjectService.findProjectById(id);
            if (project != null)
            {
                ApplicationUser currentUser = _MembershipService.GetUserById(User.Identity.GetUserId());
                if (CheckPermissionForProject(currentUser, project.ProjectMembers))
                {
                    return View();
                }
                else
                {
                    return View("Index");
                }
            }
            return View("Index");
        }

        [HttpPost]
        public JsonResult Delete(int id)
        {
            try
            {
                var project = _ProjectService.findProjectById(id);
                if (project != null)
                {
                    ApplicationUser currentUser = _MembershipService.GetUserById(User.Identity.GetUserId());
                    if (CheckPermissionForProject(currentUser, project.ProjectMembers))
                    {
                        //foreach (var CommeAttach in project.CommunityComments)
                        //{
                        //    foreach (var item in CommeAttach.CommentAttachments)
                        //    {
                        //        Util.DeleteFileLocal(item.ComentFileLocationPath);
                        //    }

                        //}
                        foreach (var ProAttach in project.ProjectAttachments)
                        {
                            Util.DeleteFileLocal(ProAttach.FileLocationPath);
                        }
                        _ProjectService.Delete(project);
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
                ProjectComment Comment = _ProjectService.findCommentById(id);
                if (Comment != null)
                {
                    ApplicationUser currentUser = _MembershipService.GetUserById(User.Identity.GetUserId());
                    if (CheckPermission(currentUser, Comment.ComentUserID))
                    {
                        _ProjectService.EditComment(id, content);
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
                ProjectComment Comment = _ProjectService.findCommentById(id);
                if (Comment != null)
                {
                    ApplicationUser currentUser = _MembershipService.GetUserById(User.Identity.GetUserId());
                    if (CheckPermission(currentUser, Comment.ComentUserID))
                    {
                        //foreach (var Attachment in Comment.CommentAttachments)
                        //{
                        //    Util.DeleteFileLocal(Attachment.ComentFileLocationPath);
                        //}
                        _ProjectService.DeleteComment(Comment);
                        return Json(new { success = true, RedirectURL = Url.Action("ProjectDetails", new { @id = Comment.ProjectID, @From = "Comment" }) });
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

        [HttpPost]
        public ActionResult Members_Read([DataSourceRequest] DataSourceRequest request, int PrjctId)
        {
            if (PrjctId != 0)
            {
                ApplicationUser currentUser = _MembershipService.GetUserById(User.Identity.GetUserId());
                var Project = _ProjectService.findProjectById(PrjctId);
                return Json(_MembershipService.GetMembersInProject(Project.ProjectMembers).ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult ReadAllMembers([DataSourceRequest] DataSourceRequest request)
        {
            return Json(_MembershipService.GetAllMembersForProject().ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeleteProjectMember(string MemberId, int PrjctId)
        {
            ApplicationUser currentUser = _MembershipService.GetUserById(User.Identity.GetUserId());
            if (currentUser.ExtendAspNetUser != null && currentUser.ExtendAspNetUser.SystemAdmin)
            {
                _MembershipService.DeleteProjectMember(MemberId, PrjctId);
                return Json(true);
            }
            return Json(false);
        }

        [HttpPost]
        public JsonResult UpdatePercent(string MemberId, int PrjctId, decimal Percent)
        {
            //ApplicationUser currentUser = _MembershipService.GetUserById(User.Identity.GetUserId());
            //if (currentUser.ExtendAspNetUser != null && currentUser.ExtendAspNetUser.SystemAdmin)
            //{
            //    _MembershipService.DeleteProjectMember(MemberId, PrjctId);
            //    return Json(true);
            //}
            try
            {
                var ProjectMember = _ProjectService.FindProjectMember(MemberId, PrjctId);
                ProjectMember.PercentInProject = Percent;
                _ProjectService.SaveChange();
                return Json(true);
            }
            catch (Exception)
            {
                return Json(false);
            }

        }

        public ActionResult Create(int? CommId)
        {
            InitDropDown();
            return View();
        }
        [HttpPost]
        public ActionResult Create(CreateProjectViewModel project)
        {
            InitDropDown();
            return View(project);
        }
        private void InitDropDown() {
            ViewBag.Categories = _CommonService.getCommonById("Ctgry").ChildCommom.Select(c => new SelectListItem
            {
                Text = c.ComName,
                Value = c.ComCode
            });

            ViewBag.ProjectSize = _CommonService.getCommonById("ProSiz").ChildCommom.Select(c => new SelectListItem
            {
                Text = c.ComName,
                Value = c.ComCode
            });
            ViewBag.Client = _ProjectService.GetAllCompany().Select(c => new SelectListItem
            {
                Text = c.CompanyName,
                Value = c.CompanyID.ToString()
            });
            ViewBag.ListProjectStatus = _CommonService.getCommonById("ProSta").ChildCommom.Select(c => new SelectListItem
            {
                Text = c.ComName,
                Value = c.ComCode
            });
        }

        #region Project Details Helper
        private void InitDropDownlist()
        {
            var objDict = new Dictionary<string, string>();
            foreach (var cultureInfo in CultureInfo.GetCultures(CultureTypes.SpecificCultures))
            {
                var regionInfo = new RegionInfo(cultureInfo.Name);
                if (!objDict.ContainsKey(regionInfo.EnglishName))
                {
                    objDict.Add(regionInfo.EnglishName, regionInfo.TwoLetterISORegionName.ToLower());
                }
            }
            var obj = objDict.OrderBy(p => p.Key).ToArray();
            ViewBag.ListCountries = obj.Select(c => new SelectListItem
            {
                Text = c.Key,
                Value = c.Key
            });
            ViewBag.ListType = _CommonService.getAllSpecialtyType().ToList().Select(c => new SelectListItem
            {
                Text = c.ComName,
                Value = c.ComCode
            });
        }
        private string GetPMName(ICollection<ProjectMember> firstQuery)
        {
            if (firstQuery.Any() && firstQuery.Count > 0)
            {
                var queryResult = firstQuery.Where(p => p.ProjectRole == "PR_PM").FirstOrDefault();
                if (queryResult != null)
                {
                    var user = _MembershipService.GetUserById(queryResult.MemberID);
                    return user.FirstName + " " + user.LastName;
                }
            }
            return string.Empty;
        }
        private string GetMainImageURL(ICollection<ProjectAttachment> firstQuery)
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

        private bool CheckPermissionForProject(ApplicationUser currentUser, ICollection<ProjectMember> firstQuery)
        {
            if (firstQuery.SingleOrDefault(m => m.MemberID.Equals(currentUser.Id) && m.ProjectRole.Equals("PR_PM")) != null)
            {
                return true;
            }
            else
            {
                return currentUser.ExtendAspNetUser != null && currentUser.ExtendAspNetUser.SystemAdmin;
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

        private List<ProjectAttachmentViewModel> getProjectAttachments(ICollection<ProjectAttachment> ProjectAttachment)
        {
            List<ProjectAttachmentViewModel> ListProjectAttachments = new List<ProjectAttachmentViewModel>();
            foreach (var item in ProjectAttachment)
            {
                if (item.DisplayImgYn == false)
                {
                    ListProjectAttachments.Add(new ProjectAttachmentViewModel
                    {
                        Url = item.FileLocationPath,
                        Filename = item.FileName,
                        isFile = item.ImgOrOther,
                        FileSize = item.FileSize
                    });
                }
            }
            return ListProjectAttachments;
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