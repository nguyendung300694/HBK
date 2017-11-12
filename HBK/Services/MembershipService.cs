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
        void AddExtendUser(ExtendAspNetUser user);

        IEnumerable<MemberViewModel> GetAllMembers(ApplicationUser currentUser);
        void UpdateLastLogin(string Email);

        void ChangeLockOut(string Email);
        void Save();

        IEnumerable<MemberViewModel> GetMembersInProject( ICollection<ProjectMember> members);

        void DeleteProjectMember(string MemberId, int ProjectId);

        IEnumerable<MemberInformationViewModel> GetInformationAllMembers(ApplicationUser currentUser);

        IEnumerable<MemberInformationViewModel> GetInformationAllAccounts(ApplicationUser currentUser);

        void DeleteAccount(string Id);


        IEnumerable<MemberViewModel> GetAllMembersForProject();
        //void UpdateUserInformation(MemberInformationViewModel userInformation);
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
        public void AddExtendUser(ExtendAspNetUser user)
        {
            _db.ExtendAspNetUsers.Add(user);
            _db.SaveChanges();
        }

        public ApplicationUser GetUserById(string Id)
        {
            return _db.Users.Find(Id);
        }

        public IEnumerable<MemberViewModel> GetAllMembers(ApplicationUser currentUser)
        {
            IQueryable<ApplicationUser> listUsers = null;
            bool isAdmin = currentUser.ExtendAspNetUser != null && currentUser.ExtendAspNetUser.SystemAdmin == true;
            if (isAdmin)
            {
                listUsers = _db.Users.Where(u => u.ExtendAspNetUser != null && u.ExtendAspNetUser.SystemAdmin == false);
            }
            else
            {
                listUsers = _db.Users.Where(u => (u.LockoutEndDateUtc.HasValue == false || (u.LockoutEndDateUtc.HasValue == true && u.LockoutEndDateUtc.Value < DateTime.Now)) &&
                                                 u.ExtendAspNetUser != null && u.ExtendAspNetUser.SystemAdmin == false);
            }
            List<MemberViewModel> listMember = new List<MemberViewModel>();
            foreach (var item in listUsers)
            {
                listMember.Add(new MemberViewModel
                {
                    //Id = item.Id,
                    Email = item.Email,
                    //FirstName = item.FirstName,
                    //LastName = item.LastName,
                    Tel = item.PhoneNumber,
                    Address = item.Address ?? "",
                    Country = item.Country,
                    Avatar = "../" + getAvatar(item).Substring(2),
                    SpecialtyType = item.ExtendAspNetUser.Common.ComName,
                    SNSSite = item.ExtendAspNetUser.SnsSite ?? "",
                    Recommender = item.ExtendAspNetUser.Recommender,
                    Introduction = item.ExtendAspNetUser.SelfIntroduction,
                    //Career = item.ExtendAspNetUser.CareerInfo,
                    //CareerDuration = item.ExtendAspNetUser.CareerDuration,
                    IsModifier = CheckPermission(currentUser, item.Id),
                    Fullname = item.FirstName + " " + item.LastName,
                    LastLoginDate = item.ExtendAspNetUser.LastLoginDate,
                    RegistrationDate = item.ExtendAspNetUser.RegistrationDate,
                    IsLocked = isAdmin ? item.LockoutEnabled == true && item.LockoutEndDateUtc.HasValue && item.LockoutEndDateUtc.Value > DateTime.Now : false,

                });
            }
            return listMember;
        }

        public IEnumerable<MemberViewModel> GetAllMembersForProject()
        {
            IQueryable<ApplicationUser> listUsers = null;

                listUsers = _db.Users.Where(u => (u.LockoutEndDateUtc.HasValue == false || (u.LockoutEndDateUtc.HasValue == true && u.LockoutEndDateUtc.Value < DateTime.Now)) &&
                                                 u.ExtendAspNetUser != null && u.ExtendAspNetUser.SystemAdmin == false);
            List<MemberViewModel> listMember = new List<MemberViewModel>();
            foreach (var item in listUsers)
            {
                listMember.Add(new MemberViewModel
                {
                    Email = item.Email,
                    Country = item.Country,
                    Avatar = "/../../" + getAvatar(item).Substring(2),
                    SpecialtyType = item.ExtendAspNetUser.Common.ComName,
                    Fullname = item.FirstName + " " + item.LastName,
                });
            }
            return listMember;
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

        public void UpdateLastLogin(string Email)
        {
            var user = _db.Users.SingleOrDefault(u => u.Email.Equals(Email));
            if (user != null && user.ExtendAspNetUser != null)
            {
                user.ExtendAspNetUser.LastLoginDate = DateTime.Now;
                _db.SaveChanges();
            }
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

        public IEnumerable<MemberViewModel> GetMembersInProject( ICollection<ProjectMember> members)
        {
            //bool isAdmin = currentUser.ExtendAspNetUser != null && currentUser.ExtendAspNetUser.SystemAdmin == true;
            List<MemberViewModel> listMember = new List<MemberViewModel>();
            foreach (var member in members)
            {
                var user = GetUserById(member.MemberID);
                listMember.Add(new MemberViewModel
                {
                    //Email = user.Email,
                    //Tel = user.PhoneNumber,
                    //Address = user.Address ?? "",
                    //Country = user.Country,
                    //Avatar = "../../" + getAvatar(user).Substring(2),
                    //SpecialtyType = user.ExtendAspNetUser.Common.ComName,
                    //SNSSite = user.ExtendAspNetUser.SnsSite ?? "",
                    //Recommender = user.ExtendAspNetUser.Recommender,
                    //Introduction = user.ExtendAspNetUser.SelfIntroduction,
                    //IsModifier = CheckPermission(currentUser, user.Id),
                    //Fullname = user.FirstName + " " + user.LastName,
                    //LastLoginDate = user.ExtendAspNetUser.LastLoginDate,
                    //RegistrationDate = user.ExtendAspNetUser.RegistrationDate,
                    //IsLocked = isAdmin ? user.LockoutEnabled == true && user.LockoutEndDateUtc.HasValue && user.LockoutEndDateUtc.Value > DateTime.Now : false,
                    //ProjectRole = member.ProjectRoleObj.ComName,
                    //ProjectPercent = member.PercentInProject.ToString("0.#"),
                    //MemberId = member.MemberID,

                    Email = user.Email,
                    Country = user.Country,
                    Avatar = "../" + getAvatar(user).Substring(2),
                    SpecialtyType = user.ExtendAspNetUser.Common.ComName,
                    Fullname = user.FirstName + " " + user.LastName,
                    ProjectRole = member.ProjectRoleObj.ComName,
                    MemberId = member.MemberID,
                });
            }
            return listMember;
        }

        //public IEnumerable<MemberViewModel> GetMembersProject(ICollection<ProjectMember> members)
        //{
        //    List<MemberViewModel> listMember = new List<MemberViewModel>();
        //    foreach (var member in members)
        //    {
        //        var user = GetUserById(member.MemberID);
        //        listMember.Add(new MemberViewModel
        //        {
        //            Email = user.Email,
        //            Country = user.Country,
        //            Avatar = "../" + getAvatar(user).Substring(2),
        //            SpecialtyType = user.ExtendAspNetUser.Common.ComName,
        //            Fullname = user.FirstName + " " + user.LastName,
        //            ProjectRole = member.ProjectRoleObj.ComName,
        //            MemberId = member.MemberID,
        //        });
        //    }
        //    return listMember;
        //}

        public void DeleteProjectMember(string MemberId, int ProjectId)
        {
            var ProjectMember = _db.ProjectMembers.SingleOrDefault(p => p.ProjectID == ProjectId && p.MemberID.Equals(MemberId));
            if (ProjectMember != null)
            {
                _db.ProjectMembers.Remove(ProjectMember);
                _db.SaveChanges();
            }            
        }

        public IEnumerable<MemberInformationViewModel> GetInformationAllMembers(ApplicationUser currentUser)
        {
            IQueryable<ApplicationUser> listUsers = null;
            bool isAdmin = currentUser.ExtendAspNetUser != null && currentUser.ExtendAspNetUser.SystemAdmin == true;
            if (isAdmin)
            {
                listUsers = _db.Users.Where(u => u.ExtendAspNetUser != null && u.ExtendAspNetUser.SystemAdmin == false);
            }
            else
            {
                listUsers = _db.Users.Where(u => (u.LockoutEndDateUtc.HasValue == false || (u.LockoutEndDateUtc.HasValue == true && u.LockoutEndDateUtc.Value < DateTime.Now)) &&
                                                 u.ExtendAspNetUser != null && u.ExtendAspNetUser.SystemAdmin == false);
            }
            List<MemberInformationViewModel> listMember = new List<MemberInformationViewModel>();
            foreach (var item in listUsers)
            {
                listMember.Add(new MemberInformationViewModel
                {
                    Id = item.Id,
                    Email = item.Email,
                    FirstName = item.FirstName,
                    LastName = item.LastName,
                    IsChange = false,
                    SpecialtyTypeCode = item.ExtendAspNetUser.SpecialtyType,
                    SpecialtyType = item.ExtendAspNetUser.Common.ComName,
                    Recommender = item.ExtendAspNetUser.Recommender,
                    Career = item.ExtendAspNetUser.CareerInfo,
                    CareerDuration = item.ExtendAspNetUser.CareerDuration,
                    LastLoginDate = item.ExtendAspNetUser.LastLoginDate
                });
            }
            return listMember;
        }

        public IEnumerable<MemberInformationViewModel> GetInformationAllAccounts(ApplicationUser currentUser)
        {
            IQueryable<ApplicationUser> listUsers = null;
            bool isAdmin = currentUser.ExtendAspNetUser != null && currentUser.ExtendAspNetUser.SystemAdmin == true;
            if (isAdmin)
            {
                listUsers = _db.Users;
            }
            else
            {
                listUsers = _db.Users.Where(u => (u.LockoutEndDateUtc.HasValue == false || (u.LockoutEndDateUtc.HasValue == true && u.LockoutEndDateUtc.Value < DateTime.Now)) &&
                                                 u.ExtendAspNetUser != null && u.ExtendAspNetUser.SystemAdmin == false);
            }
            List<MemberInformationViewModel> listMember = new List<MemberInformationViewModel>();
            foreach (var item in listUsers)
            {
                listMember.Add(new MemberInformationViewModel
                {
                    Id = item.Id,
                    Email = item.Email,
                    FirstName = item.FirstName,
                    LastName = item.LastName,
                    IsChange = false,
                    SpecialtyTypeCode = item.ExtendAspNetUser.SpecialtyType,
                    SpecialtyType = item.ExtendAspNetUser.Common.ComName,
                    Recommender = item.ExtendAspNetUser.Recommender,
                    Career = item.ExtendAspNetUser.CareerInfo,
                    CareerDuration = item.ExtendAspNetUser.CareerDuration,
                    LastLoginDate = item.ExtendAspNetUser.LastLoginDate,
                });
            }
            return listMember;
        }

        public void DeleteAccount(string Id)
        {
            var user = _db.Users.Find(Id);
            if (user != null)
            {
                var extendUser = user.ExtendAspNetUser;
                var userPhoto = extendUser.UsersPhotos.ToList();
                _db.ExtendAspNetUsers.Remove(extendUser);
                foreach (var item in userPhoto)
                {
                    _db.UsersPhotos.Remove(item);
                    Util.DeleteFileLocal(item.UserPhtoFileLocationPath);
                    //_db.SaveChanges();
                }
                
                //var x = 10;
                _db.Users.Remove(user);
                _db.SaveChanges();
            }
        }
    }
}