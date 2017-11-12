using BEN_Community.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace BEN_Community.Services
{
    public interface IProjectService
    {
        IEnumerable<ProjectViewModel> getAllProjects();
        Project findProjectById(int id);
        ProjectComment findCommentById(int ID);
        void DeleteComment(ProjectComment comment);
        ProjectComment EditComment(int id, string content);
        void Delete(Project project);

        void WriteNewComment(ProjectDetailsViewModel project);

        //bool IsProjectManager(string userId, int projectId);

        ProjectMember FindProjectMember(string memberId, int projectId);
        void SaveChange();
        IEnumerable<Company> GetAllCompany();
    }
    public class ProjectService : IProjectService
    {
        private readonly BenComDbContext _db;
        public ProjectService(IDbFactory dbFactory)
        {
            _db = dbFactory.Init();
        }

        public Project findProjectById(int id)
        {
            return _db.Projects.SingleOrDefault(c => c.ProjectID == id);
        }

        public IEnumerable<ProjectViewModel> getAllProjects()
        {
            List<ProjectViewModel> result = new List<ProjectViewModel>();
            foreach (var item in _db.Projects)
            {
                result.Add(new ProjectViewModel
                {
                    ProjectId = item.ProjectID,
                    ProjectName = item.ProjectName,
                    PMName = GetPMName(item.ProjectMembers),
                    ImageURL = GetMainImageURL(item.ProjectAttachments),
                    StartDate = item.StartDate,
                    EndDate = item.EndDate,
                    Status = item.ProjectStatusObj.ComName,
                    MemberCount = item.ProjectMembers.Count
                });
            }
            return result;
        }

        public void WriteNewComment(ProjectDetailsViewModel project)
        {
            ProjectComment comment = new ProjectComment
            {
                ComentContent = project.NewComment,
                ProjectID = project.PrjctId,
                ComentTime = DateTime.Now,
                ComentUserID = project.AuthorId
            };
            _db.ProjectComments.Add(comment);
            _db.SaveChanges();
            //if (project.NewCommentAttachment != null)
            //{
            //    CommentAttachment Attachment = new CommentAttachment
            //    {
            //        ComentID = comment.ComentID,
            //        ComentFileLocationPath = Util.CreateCommentAttachment(project.AuthorId, project.NewCommentAttachment),
            //        CommentFileName = project.NewCommentAttachment.FileName,
            //        CommentFileSize = project.NewCommentAttachment.ContentLength,
            //        CommentFileType = project.NewCommentAttachment.ContentType,
            //        CommentImgOrOther = project.NewCommentAttachment.ContentType.Contains("image"),
            //    };
            //    _db.CommentAttachments.Add(Attachment);
            //    _db.SaveChanges();
            //}
        }
        private string GetPMName(ICollection<ProjectMember> firstQuery)
        {
            if (firstQuery.Any() && firstQuery.Count > 0)
            {
                var queryResult = firstQuery.Where(p => p.ProjectRole == "PR_PM").FirstOrDefault();
                if (queryResult != null)
                {
                    var user = _db.Users.Find(queryResult.MemberID);
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
                return queryResult != null ? (queryResult.FileLocationPath.Length > 2 ? "../" + queryResult.FileLocationPath.Substring(2) : Util.DefaultImage()) : Util.DefaultImage();
            }
            else
            {
                return Util.DefaultImage();
            }
        }

        public void Delete(Project project)
        {
            _db.Projects.Remove(project);
            _db.SaveChanges();
        }

        public void DeleteComment(ProjectComment comment)
        {
            _db.ProjectComments.Remove(comment);
            _db.SaveChanges();
        }

        public ProjectComment EditComment(int id, string content)
        {
            ProjectComment result = _db.ProjectComments.SingleOrDefault(c => c.ProjectCommentID == id);
            if (result != null)
            {
                result.ComentContent = content;
                result.ComentTime = DateTime.Now;
                _db.SaveChanges();
            }
            return result;
        }

        public ProjectComment findCommentById(int ID)
        {
            return _db.ProjectComments.Find(ID);
        }

        public ProjectMember FindProjectMember(string memberId, int projectId)
        {
            return _db.ProjectMembers.SingleOrDefault(p => p.MemberID == memberId && p.ProjectID == projectId);
        }

        public void SaveChange()
        {
            _db.SaveChanges();
        }

        public IEnumerable<Company> GetAllCompany()
        {
            return _db.Companies.AsEnumerable();
        }

        //public bool IsProjectManager(string userId, int projectId)
        //{
        //    return _db.ProjectMembers.SingleOrDefault(m => m.MemberID.Equals(userId) && m.ProjectRole.Equals("PR_PM")) != null;
        //}
    }
}