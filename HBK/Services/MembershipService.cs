using HBK.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HBK.Services
{
    public interface IMembershipService
    {
        ApplicationUser GetUserByEmail(string email);

        ApplicationUser GetUserById(string Id);
      
        void UpdateLastLogin(string Email);

        void ChangeLockOut(string Email);
        void Save();

        void DeleteProjectMember(string MemberId, int ProjectId);

      
        void DeleteAccount(string Id);


     
    }
    public class MembershipService : IMembershipService
    {
        private readonly HBKDbContext _db;
        //private ApplicationUserManager _userManager;
        public MembershipService(IDbFactory dbFactory)
        {
            _db = dbFactory.Init();
        }
        public ApplicationUser GetUserByEmail(string email)
        {
            return _db.Users.Where(u => u.Email.Equals(email) || u.UserName.Equals(email)).FirstOrDefault();
        }
   

        public ApplicationUser GetUserById(string Id)
        {
            return _db.Users.Find(Id);
        }

   

     
      
        public void ChangeLockOut(string Email)
        {
            var user = _db.Users.SingleOrDefault(u => u.Email.Equals(Email));
            if (user != null)
            {
                if (user.LockoutEnabled == true && user.LockoutEndDateUtc.HasValue && user.LockoutEndDateUtc.Value > DateTime.Now)
                {
                    user.LockoutEndDateUtc = null;
                }
                else
                {
                    user.LockoutEnabled = true;
                    user.LockoutEndDateUtc = DateTime.Today.AddYears(100);
                }
                _db.SaveChanges();
            }
        }

        public void Save()
        {
            _db.SaveChanges();
        }

        public void UpdateLastLogin(string Email)
        {
            throw new NotImplementedException();
        }

        public void DeleteProjectMember(string MemberId, int ProjectId)
        {
            throw new NotImplementedException();
        }

        public void DeleteAccount(string Id)
        {
            throw new NotImplementedException();
        }
    }
}