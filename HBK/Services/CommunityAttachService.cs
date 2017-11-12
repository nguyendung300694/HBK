using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HBK;
using HBK.Models;

namespace HBK.Services
{
    public interface ICommunityAttachmentService
    {
        void AddCommunityAttachment(CommunityAttachment Attachment);
    }

    public class CommunityAttachmentService : ICommunityAttachmentService
    {

        private HBKDbContext _db;

        public CommunityAttachmentService(IDbFactory dbFactory)
        {
            _db = dbFactory.Init();
        }
        public void AddCommunityAttachment(CommunityAttachment Attachment)
        {
            _db.CommunityAttachments.Add(Attachment);
            _db.SaveChanges();
        }
    }
}