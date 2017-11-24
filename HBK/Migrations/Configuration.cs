namespace HBK.Migrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<HBK.Models.HBKDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(HBK.Models.HBKDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
            //if (System.Diagnostics.Debugger.IsAttached == false)
            //{
            //    System.Diagnostics.Debugger.Launch();
            //}
            #region Admin
            var admin = context.Users.SingleOrDefault(u => u.Email.Equals("worknit@gmail.com"));
            if (admin == null)
            {
                var userStore = new UserStore<ApplicationUser>(context);
                var userManager = new UserManager<ApplicationUser>(userStore);
                //var passwordHash = new PasswordHasher();

                ApplicationUser UserAdmin = new ApplicationUser
                {
                    UserName = "worknit@gmail.com",
                    Email = "worknit@gmail.com",
                    PhoneNumber = "031-689-3138",
                    FirstName = "Administrator",
                    LastName = "Worknit",
                    Address = "9F 910-3D-ho, Indeogwon IT Valley, 40, Imi-ro, Uiwang-si, Gyeonggi-do",
                    Country = "Korea",
                    //PasswordHash = passwordHash.HashPassword(""),
                };
                var result = userManager.Create(UserAdmin, "work&ben#170401");
                //if (result.Succeeded)
                //{
                //    ExtendAspNetUser extendAdmin = new ExtendAspNetUser
                //    {
                //        UserID = UserAdmin.Id,
                //        KorName = "Administrator Worknit",
                //        EngName = "Administrator Worknit",
                //        SpecialtyType = "IT_G",
                //        CareerInfo = "Hello everyone, I'm a Administrator",
                //        CareerDuration = 69,
                //        SelfIntroduction = "Hello everyone, I'm a Administrator",
                //        Recommender = "worknit@gmail.com",
                //        RegistrationDate = DateTime.Now,
                //        LastLoginDate = DateTime.Now,
                //        SystemAdmin = true
                //    };
                //    context.ExtendAspNetUsers.AddOrUpdate(extendAdmin);
                //}
            }
            else
            {
                //ExtendAspNetUser extendAdmin = new ExtendAspNetUser
                //{
                //    UserID = admin.Id,
                //    KorName = "Administrator Worknit",
                //    EngName = "Administrator Worknit",
                //    SpecialtyType = "IT_G",
                //    CareerInfo = "Hello everyone, I'm a Administrator",
                //    CareerDuration = 69,
                //    SelfIntroduction = "Hello everyone, I'm a Administrator",
                //    Recommender = "worknit@gmail.com",
                //    RegistrationDate = DateTime.Now,
                //    LastLoginDate = DateTime.Now,
                //    SystemAdmin = true
                //};
                //context.ExtendAspNetUsers.AddOrUpdate(extendAdmin);
            }
            #endregion

        
        }
    }
}
