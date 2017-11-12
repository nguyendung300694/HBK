using HBK.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace HBK.Services
{
    public interface ICommunityService
    {
        IEnumerable<CommunicationViewModel> getAllCommunities();
        Community findCommunityById(int id);
        Community findCommunityByIdOfCommunityAttachment(int id);
        CommunityAttachment findCommunityAttachmentById(int id);
        Community increaseHits(int id);
        CommunityComment findCommentById(int ID);
        void DeleteComment(CommunityComment comment);
        CommunityComment EditComment(int id, string content);
        void Delete(Community community);
        CommunityComment WriteNewComment(CommunicationDetailsViewModel communication);
        void CreateCommunity(Community community);
        void DeleteCommunityAttachment(CommunityAttachment commAtt);
        void SaveChange();

    }
    public class CommunityService : ICommunityService
    {
        private readonly HBKDbContext _db;
        public CommunityService(IDbFactory dbFactory)
        {
            _db = dbFactory.Init();
        }

        public void CreateCommunity(Community community)
        {
                _db.Communities.Add(community);
                _db.SaveChanges();
        }

        public void Delete(Community community)
        {
            _db.Communities.Remove(community);
            _db.SaveChanges();
        }

        public void DeleteComment(CommunityComment comment)
        {
            _db.CommunityComments.Remove(comment);
            _db.SaveChanges();
        }

        public void DeleteCommunityAttachment(CommunityAttachment commAtt)
        {
            _db.CommunityAttachments.Remove(commAtt);
            _db.SaveChanges();
        }

        public CommunityComment EditComment(int id, string content)
        {
            CommunityComment result = _db.CommunityComments.SingleOrDefault(c => c.ComentID == id);
            if (result != null)
            {
                result.ComentContent = content;
                result.ComentTime = DateTime.Now;
                _db.SaveChanges();
            }
            return result;
        }

        public CommunityComment findCommentById(int id)
        {
            return _db.CommunityComments.SingleOrDefault(c => c.ComentID == id);
        }

        public CommunityAttachment findCommunityAttachmentById(int id)
        {
            return _db.CommunityAttachments.Find(id);
        }

        public Community findCommunityById(int id)
        {
            return _db.Communities.Find(id);
        }

        public Community findCommunityByIdOfCommunityAttachment(int id)
        {
            var CommunityAttachment = _db.CommunityAttachments.Find(id);
            return _db.Communities.Find(CommunityAttachment.CommID);
        }

        public IEnumerable<CommunicationViewModel> getAllCommunities()
        {
            List<CommunicationViewModel> result = new List<CommunicationViewModel>();
            foreach (var item in _db.Communities)
            {
                result.Add(new CommunicationViewModel
                {
                    CommunicationId = item.CommID,
                    Title = item.CommTitle,
                    Writer = item.AuthorUserName,
                    ImageURL = GetMainImageURL(item.CommunityAttachments),
                    Date = item.InsertDate,
                    CommentCount = item.CommunityComments.Count,
                    ViewCount = item.Hits
                });
            }
            return result;
        }

        public Community increaseHits(int id)
        {
            Community result = _db.Communities.SingleOrDefault(b => b.CommID == id);
            if (result != null)
            {
                result.Hits++;
                _db.SaveChanges();
            }
            return result;
        }

        public void SaveChange()
        {
            _db.SaveChanges();
        }

        public CommunityComment WriteNewComment(CommunicationDetailsViewModel communication)
        {
            CommunityComment comment = new CommunityComment
            {
                ComentContent = communication.NewComment,
                CommID = communication.CommuId,
                ComentTime = DateTime.Now,
                ComentUserID = communication.AuthorId
            };
            _db.CommunityComments.Add(comment);
            _db.SaveChanges();

            var CommunityComment = _db.CommunityComments.Find(comment.ComentID);
            
            if (communication.NewCommentAttachment != null)
            {
                CommentAttachment Attachment = new CommentAttachment
                {
                    ComentID = comment.ComentID,
                    ComentFileLocationPath = Util.CreateCommentAttachment(communication.AuthorId, communication.NewCommentAttachment),
                    CommentFileName = communication.NewCommentAttachment.FileName,
                    CommentFileSize = communication.NewCommentAttachment.ContentLength,
                    CommentFileType = communication.NewCommentAttachment.ContentType,
                    CommentImgOrOther = communication.NewCommentAttachment.ContentType.Contains("image"),
                };
                _db.CommentAttachments.Add(Attachment);
                _db.SaveChanges();
            }

            return CommunityComment;
        }

        private string GetMainImageURL(ICollection<CommunityAttachment> firstQuery)
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
    }
}