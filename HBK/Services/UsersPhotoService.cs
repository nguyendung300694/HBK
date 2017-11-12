using HBK.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HBK.Services
{
    public interface IUsersPhotoService
    {
        void AddUsersPhoto(UsersPhoto usersPhoto);
    }
    public class UsersPhotoService : IUsersPhotoService
    {
        private readonly HBKDbContext _db;
        public UsersPhotoService(IDbFactory dbFactory)
        {
            _db = dbFactory.Init();
        }
        public void AddUsersPhoto(UsersPhoto usersPhoto)
        {
            _db.UsersPhotos.Add(usersPhoto);
            _db.SaveChanges();
        }
    }
}