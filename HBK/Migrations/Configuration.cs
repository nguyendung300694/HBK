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
                if (result.Succeeded)
                {
                    ExtendAspNetUser extendAdmin = new ExtendAspNetUser
                    {
                        UserID = UserAdmin.Id,
                        KorName = "Administrator Worknit",
                        EngName = "Administrator Worknit",
                        SpecialtyType = "IT_G",
                        CareerInfo = "Hello everyone, I'm a Administrator",
                        CareerDuration = 69,
                        SelfIntroduction = "Hello everyone, I'm a Administrator",
                        Recommender = "worknit@gmail.com",
                        RegistrationDate = DateTime.Now,
                        LastLoginDate = DateTime.Now,
                        SystemAdmin = true
                    };
                    context.ExtendAspNetUsers.AddOrUpdate(extendAdmin);
                }
            }
            else
            {
                ExtendAspNetUser extendAdmin = new ExtendAspNetUser
                {
                    UserID = admin.Id,
                    KorName = "Administrator Worknit",
                    EngName = "Administrator Worknit",
                    SpecialtyType = "IT_G",
                    CareerInfo = "Hello everyone, I'm a Administrator",
                    CareerDuration = 69,
                    SelfIntroduction = "Hello everyone, I'm a Administrator",
                    Recommender = "worknit@gmail.com",
                    RegistrationDate = DateTime.Now,
                    LastLoginDate = DateTime.Now,
                    SystemAdmin = true
                };
                context.ExtendAspNetUsers.AddOrUpdate(extendAdmin);
            }
            #endregion

            #region Specialty Type
            Common SpecialtyType = new Common
            {
                ComCode = "IT",
                ComName = "IT",
                CommonType = 1,
                ComName2 = "IT"
            };
            context.Commons.AddOrUpdate(SpecialtyType);
            Common IT_A = new Common
            {
                ComCode = "IT_A",
                ComSubCode = "IT",
                ComName = "Embedded developer",
                ComName2 = "Embedded developer",
                CommonType =1
            };
            context.Commons.AddOrUpdate(IT_A);

            Common IT_B = new Common
            {
                ComCode = "IT_B",
                ComSubCode = "IT",
                ComName = "Database Engineer",
                ComName2 = "Database Engineer",
                CommonType = 1
            };
            context.Commons.AddOrUpdate(IT_B);

            Common IT_C = new Common
            {
                ComCode = "IT_C",
                ComSubCode = "IT",
                ComName = "Network Engineer",
                ComName2 = "Network Engineer",
                CommonType = 1
            };
            context.Commons.AddOrUpdate(IT_C);

            Common IT_D = new Common
            {
                ComCode = "IT_D",
                ComSubCode = "IT",
                ComName = "Application developers",
                ComName2 = "Application developers",
                CommonType = 1
            };
            context.Commons.AddOrUpdate(IT_D);

            Common IT_E = new Common
            {
                ComCode = "IT_E",
                ComSubCode = "IT",
                ComName = "System Engineer",
                ComName2 = "System Engineer",
                CommonType = 1
            };
            context.Commons.AddOrUpdate(IT_E);


            Common IT_F = new Common
            {
                ComCode = "IT_F",
                ComSubCode = "IT",
                ComName = "Security Engineer",
                ComName2 = "Security Engineer",
                CommonType = 1
            };
            context.Commons.AddOrUpdate(IT_F);

            Common IT_G = new Common
            {
                ComCode = "IT_G",
                ComSubCode = "IT",
                ComName = "System Analysis Engineer",
                ComName2 = "System Analysis Engineer",
                CommonType = 1
            };
            context.Commons.AddOrUpdate(IT_G);
            #endregion

            #region Project Role
            Common ProjectRole = new Common
            {
                ComCode = "PrRole",
                ComName = "Project Role",
                ComName2 = "Project Role",
                CommonType =1
            };
            context.Commons.AddOrUpdate(ProjectRole);
            Common PR_PM = new Common
            {
                ComCode = "PR_PM",
                ComSubCode = "PrRole",
                ComName = "Project Manager",
                ComName2 = "Project Manager",
                CommonType = 1
            };
            context.Commons.AddOrUpdate(PR_PM);

            Common PR_PL = new Common
            {
                ComCode = "PR_PL",
                ComSubCode = "PrRole",
                ComName = "Project Leader",
                ComName2 = "Project Leader",
                CommonType = 1
            };
            context.Commons.AddOrUpdate(PR_PL);

            Common PR_PE = new Common
            {
                ComCode = "PR_PE",
                ComSubCode = "PrRole",
                ComName = "Project Engineer",
                ComName2 = "Project Engineer",
                CommonType = 1
            };
            context.Commons.AddOrUpdate(PR_PE);

            Common PR_PF = new Common
            {
                ComCode = "PR_PF",
                ComSubCode = "PrRole",
                ComName = "Project Finace",
                ComName2 = "Project Finace",
                CommonType = 1
            };
            context.Commons.AddOrUpdate(PR_PF);

            Common PR_QA = new Common
            {
                ComCode = "PR_QA",
                ComSubCode = "PrRole",
                ComName = "QA(Quality Assurance)",
                ComName2 = "QA(Quality Assurance)",
                CommonType = 1
            };
            context.Commons.AddOrUpdate(PR_QA);

            Common PR_PRM = new Common
            {
                ComCode = "PR_PRM",
                ComSubCode = "PrRole",
                ComName = "Project Risk Manager",
                ComName2 = "Project Risk Manager",
                CommonType = 1
            };
            context.Commons.AddOrUpdate(PR_PRM);

            Common PR_PIM = new Common
            {
                ComCode = "PR_PIM",
                ComSubCode = "PrRole",
                ComName = "Project Issue Manager",
                ComName2 = "Project Issue Manager",
                CommonType = 1
            };
            context.Commons.AddOrUpdate(PR_PIM);

            Common PR_Tch = new Common
            {
                ComCode = "PR_Tch",
                ComSubCode = "PrRole",
                ComName = "Teacher",
                ComName2 = "Teacher",
                CommonType = 1
            };
            context.Commons.AddOrUpdate(PR_Tch);

            Common PR_Etc = new Common
            {
                ComCode = "PR_Etc",
                ComSubCode = "PrRole",
                ComName = "Etc",
                ComName2 = "Etc",
                CommonType = 1
            };
            context.Commons.AddOrUpdate(PR_Etc);

            #endregion

            #region Category
            Common Category = new Common
            {
                ComCode = "Ctgry",
                ComName = "Category",
                ComName2 = "Category",
                CommonType=3
            };
            context.Commons.AddOrUpdate(Category);

            Common Ct_Con = new Common
            {
                ComCode = "Ct_Con",
                ComSubCode = "Ctgry",
                ComName = "Construct",
                ComName2 = "Construct",
                CommonType = 3
            };
            context.Commons.AddOrUpdate(Ct_Con);

            Common Ct_IC = new Common
            {
                ComCode = "Ct_IC",
                ComSubCode = "Ctgry",
                ComName = "Information communication",
                ComName2 = "Information communication",
                CommonType = 3
            };
            context.Commons.AddOrUpdate(Ct_IC);

            Common Ct_Sml = new Common
            {
                ComCode = "Ct_Sml",
                ComSubCode = "Ctgry",
                ComName = "Seminar",
                ComName2 = "Seminar",
                CommonType = 3
            };
            context.Commons.AddOrUpdate(Ct_Sml);

            Common Ct_Enr = new Common
            {
                ComCode = "Ct_Enr",
                ComSubCode = "Ctgry",
                ComName = "Energy",
                ComName2 = "Energy",
                CommonType = 3
            };
            context.Commons.AddOrUpdate(Ct_Enr);

            Common Ct_Pro = new Common
            {
                ComCode = "Ct_Pro",
                ComSubCode = "Ctgry",
                ComName = "Produce",
                ComName2 = "Produce",
                CommonType = 3
            };
            context.Commons.AddOrUpdate(Ct_Pro);

            Common Ct_Fin = new Common
            {
                ComCode = "Ct_Fin",
                ComSubCode = "Ctgry",
                ComName = "Finance",
                ComName2 = "Finance",
                CommonType = 3
            };
            context.Commons.AddOrUpdate(Ct_Fin);

            Common Ct_Edu = new Common
            {
                ComCode = "Ct_Edu",
                ComSubCode = "Ctgry",
                ComName = "Education",
                ComName2 = "Education",
                CommonType = 3
            };
            context.Commons.AddOrUpdate(Ct_Edu);

            Common Ct_Etc = new Common
            {
                ComCode = "Ct_Etc",
                ComSubCode = "Ctgry",
                ComName = "Etc",
                ComName2 = "Etc",
                CommonType = 3
            };
            context.Commons.AddOrUpdate(Ct_Etc);
            #endregion

            #region Project Size
            Common ProjectSize = new Common
            {
                ComCode = "ProSiz",
                ComName = "Project Size",
                ComName2 = "Project Size",
                CommonType =4
            };
            context.Commons.AddOrUpdate(ProjectSize);

            Common PS_L = new Common
            {
                ComCode = "PS_L",
                ComSubCode = "ProSiz",
                ComName = "Large",
                ComName2 = "Large",
                CommonType = 4
            };
            context.Commons.AddOrUpdate(PS_L);

            Common PS_M = new Common
            {
                ComCode = "PS_M",
                ComSubCode = "ProSiz",
                ComName = "Medium",
                ComName2 = "Medium",
                CommonType = 4
            };
            context.Commons.AddOrUpdate(PS_M);

            Common PS_S = new Common
            {
                ComCode = "PS_S",
                ComSubCode = "ProSiz",
                ComName = "Small",
                ComName2 = "Small",
                CommonType = 4
            };
            context.Commons.AddOrUpdate(PS_S);
            #endregion

            #region Project Status
            Common ProjectStatus = new Common
            {
                ComCode = "ProSta",
                ComName = "Project Status",
                ComName2 = "Project Status",
                CommonType =5
            };
            context.Commons.AddOrUpdate(ProjectStatus);

            Common Sta_Pl = new Common
            {
                ComCode = "Sta_Pl",
                ComSubCode = "ProSta",
                ComName = "Plan",
                ComName2 = "Plan",
                CommonType = 5
            };
            context.Commons.AddOrUpdate(Sta_Pl);

            Common Sta_Pr = new Common
            {
                ComCode = "Sta_Pr",
                ComSubCode = "ProSta",
                ComName = "Progress",
                ComName2 = "Progress",
                CommonType = 5
            };
            context.Commons.AddOrUpdate(Sta_Pr);

            Common Sta_Cl = new Common
            {
                ComCode = "Sta_Cl",
                ComSubCode = "ProSta",
                ComName = "Close",
                ComName2 = "Close",
                CommonType = 5
            };
            context.Commons.AddOrUpdate(Sta_Cl);

            Common Sta_Dr = new Common
            {
                ComCode = "Sta_Dr",
                ComSubCode = "ProSta",
                ComName = "Drop",
                ComName2 = "Drop",
                CommonType = 5
            };
            context.Commons.AddOrUpdate(Sta_Dr);
            #endregion

            #region Company
            Company WorkIT = new Company
            {
                CompanyName = "Work&IT",
                CompanyType = "PS_M"
            };
            context.Companies.Add(WorkIT);

            Company THLSoft = new Company
            {
                CompanyName = "THLSoft",
                CompanyType = "PS_M"
            };
            context.Companies.Add(THLSoft);
            #endregion

            //#region Project
            //Project Project1 = new Project
            //{
            //    CommID = 7,
            //    ProjectName ="Test",
            //    Category = "Ctgry",
            //    StartDate = DateTime.Now,
            //    EndDate = DateTime.Now,
            //    ProjectSize = "PS_M",
            //    ProjectStatus = "ProSta",
            //    CompnayID = 2,
            //    ProjectContents ="test",
            //    RegDate = DateTime.Now,
            //    RegEdit = DateTime.Now,
            //    RegUser = "ce5cce65-b6f8-4b90-a4dc-4d34c933daee"
            //};
            //context.Projects.Add(Project1);
            //#endregion
        }
    }
}
