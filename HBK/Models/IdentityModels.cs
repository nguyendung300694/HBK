﻿using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HBK.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            //var currentInfor = await manager.Users.FirstOrDefaultAsync();
            //string FullName = string.Empty;
            //if (currentInfor != null)
            //{
            //    FullName = currentInfor.FirstName + " " + currentInfor.LastName;
            //}
            userIdentity.AddClaim(new Claim("FullName", FirstName + " " + LastName));
            // Add custom user claims here
            return userIdentity;
        }

        [StringLength(50)]
        public string FirstName { get; set; }

        [StringLength(50)]
        public string LastName { get; set; }

        [StringLength(500)]
        public string Address { get; set; }

        [StringLength(50)]
        public string Country { get; set; }

    }

    public class HBKDbContext : IdentityDbContext<ApplicationUser>
    {

        public DbSet<Product> Products { get; set; }
        public DbSet<ProductExtend> ProductExtends { get; set; }
        public DbSet<ProductCategory> ProductCategorys { get; set; }
        public DbSet<ProductCategoryExtend> ProductCategoryExtends { get; set; }

        public HBKDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static HBKDbContext Create()
        {
            return new HBKDbContext();
        }

        //protected override void OnModelCreating(DbModelBuilder modelBuilder)
        //{
        //    //modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        //    //modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
        //    //base.OnModelCreating(modelBuilder);
        //    modelBuilder.Entity<ProjectMember>().Property(x => x.PercentInProject).HasPrecision(3, 1);
        //}
    }
}