namespace HBK.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initDatabase : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.tblCommentAttachment",
                c => new
                    {
                        ComentAttachID = c.Int(nullable: false, identity: true),
                        CommentFileName = c.String(nullable: false, maxLength: 300),
                        CommentFileSize = c.Int(nullable: false),
                        CommentFileType = c.String(nullable: false, maxLength: 100),
                        ComentFileLocationPath = c.String(nullable: false, maxLength: 1000),
                        CommentImgOrOther = c.Boolean(nullable: false),
                        ComentID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ComentAttachID)
                .ForeignKey("dbo.tblCommunityComment", t => t.ComentID, cascadeDelete: true)
                .Index(t => t.ComentID);
            
            CreateTable(
                "dbo.tblCommunityComment",
                c => new
                    {
                        ComentID = c.Int(nullable: false, identity: true),
                        ComentContent = c.String(nullable: false, storeType: "ntext"),
                        ComentTime = c.DateTime(nullable: false),
                        ComentUserID = c.String(nullable: false, maxLength: 128),
                        CommID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ComentID)
                .ForeignKey("dbo.tblCommunity", t => t.CommID, cascadeDelete: true)
                .Index(t => t.CommID);
            
            CreateTable(
                "dbo.tblCommunity",
                c => new
                    {
                        CommID = c.Int(nullable: false, identity: true),
                        CommTitle = c.String(nullable: false, maxLength: 1000),
                        ComContent = c.String(nullable: false, storeType: "ntext"),
                        InsertDate = c.DateTime(nullable: false),
                        EditDate = c.DateTime(),
                        AuthorUserID = c.String(nullable: false, maxLength: 128),
                        AuthorUserName = c.String(nullable: false, maxLength: 128),
                        Hits = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.CommID);
            
            CreateTable(
                "dbo.tblCommunityAttachment",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        FileName = c.String(nullable: false, maxLength: 300),
                        FileSize = c.Int(nullable: false),
                        FileType = c.String(nullable: false, maxLength: 100),
                        FileLocationPath = c.String(nullable: false, maxLength: 1000),
                        DisplayImgYn = c.Boolean(nullable: false),
                        ImgOrOther = c.Boolean(nullable: false),
                        AttachComment = c.String(maxLength: 500),
                        CommID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.tblCommunity", t => t.CommID, cascadeDelete: true)
                .Index(t => t.CommID);
            
            CreateTable(
                "dbo.tblCommon",
                c => new
                    {
                        ComCode = c.String(nullable: false, maxLength: 6),
                        ComSubCode = c.String(maxLength: 6),
                        CommonType = c.Int(nullable: false),
                        ComName = c.String(nullable: false, maxLength: 50),
                        ComName2 = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.ComCode)
                .ForeignKey("dbo.tblCommon", t => t.ComSubCode)
                .Index(t => t.ComSubCode);
            
            CreateTable(
                "dbo.tblCompany",
                c => new
                    {
                        CompanyID = c.Int(nullable: false, identity: true),
                        CompanyName = c.String(nullable: false, maxLength: 500),
                        CompanyType = c.String(nullable: false, maxLength: 6),
                    })
                .PrimaryKey(t => t.CompanyID);
            
            CreateTable(
                "dbo.ExtendAspNetUsers",
                c => new
                    {
                        UserID = c.String(nullable: false, maxLength: 128),
                        KorName = c.String(nullable: false, maxLength: 150),
                        EngName = c.String(nullable: false, maxLength: 200),
                        SpecialtyType = c.String(nullable: false, maxLength: 6),
                        SnsSite = c.String(maxLength: 200),
                        CareerInfo = c.String(nullable: false, maxLength: 200),
                        CareerDuration = c.Int(nullable: false),
                        SelfIntroduction = c.String(nullable: false, storeType: "ntext"),
                        Recommender = c.String(nullable: false, maxLength: 128),
                        RegistrationDate = c.DateTime(nullable: false),
                        LastLoginDate = c.DateTime(nullable: false),
                        SystemAdmin = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.UserID)
                .ForeignKey("dbo.AspNetUsers", t => t.UserID)
                .ForeignKey("dbo.tblCommon", t => t.SpecialtyType, cascadeDelete: true)
                .Index(t => t.UserID)
                .Index(t => t.SpecialtyType);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        FirstName = c.String(maxLength: 50),
                        LastName = c.String(maxLength: 50),
                        Address = c.String(maxLength: 500),
                        Country = c.String(maxLength: 50),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.tblUsersPhoto",
                c => new
                    {
                        UserPhtoID = c.Int(nullable: false, identity: true),
                        UserPhtoFileName = c.String(nullable: false, maxLength: 300),
                        UserPhtoFileSize = c.Int(nullable: false),
                        UserPhtoFileType = c.String(nullable: false, maxLength: 100),
                        UserPhtoFileLocationPath = c.String(nullable: false, maxLength: 1000),
                        UserID = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.UserPhtoID)
                .ForeignKey("dbo.ExtendAspNetUsers", t => t.UserID)
                .Index(t => t.UserID);
            
            CreateTable(
                "dbo.tblProjectAttachment",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        FileName = c.String(nullable: false, maxLength: 300),
                        FileSize = c.Int(nullable: false),
                        FileType = c.String(nullable: false, maxLength: 100),
                        FileLocationPath = c.String(nullable: false, maxLength: 1000),
                        DisplayImgYn = c.Boolean(nullable: false),
                        ImgOrOther = c.Boolean(nullable: false),
                        AttachComment = c.String(maxLength: 500),
                        ProjectID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.tblProject", t => t.ProjectID, cascadeDelete: true)
                .Index(t => t.ProjectID);
            
            CreateTable(
                "dbo.tblProject",
                c => new
                    {
                        ProjectID = c.Int(nullable: false, identity: true),
                        CommID = c.Int(nullable: false),
                        ProjectName = c.String(nullable: false, maxLength: 300),
                        Category = c.String(maxLength: 6),
                        StartDate = c.DateTime(),
                        EndDate = c.DateTime(),
                        ProjectSize = c.String(maxLength: 6),
                        ProjectStatus = c.String(maxLength: 6),
                        CompnayID = c.Int(nullable: false),
                        ProjectContents = c.String(nullable: false, storeType: "ntext"),
                        RegDate = c.DateTime(nullable: false),
                        RegEdit = c.DateTime(),
                        RegUser = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.ProjectID)
                .ForeignKey("dbo.tblCommon", t => t.Category)
                .ForeignKey("dbo.tblCommunity", t => t.CommID, cascadeDelete: true)
                .ForeignKey("dbo.tblCompany", t => t.CompnayID, cascadeDelete: true)
                .ForeignKey("dbo.tblCommon", t => t.ProjectSize)
                .ForeignKey("dbo.tblCommon", t => t.ProjectStatus)
                .Index(t => t.CommID)
                .Index(t => t.Category)
                .Index(t => t.ProjectSize)
                .Index(t => t.ProjectStatus)
                .Index(t => t.CompnayID);
            
            CreateTable(
                "dbo.tbProjectComment",
                c => new
                    {
                        ProjectCommentID = c.Int(nullable: false, identity: true),
                        ComentContent = c.String(nullable: false, storeType: "ntext"),
                        ComentTime = c.DateTime(nullable: false),
                        ComentUserID = c.String(nullable: false, maxLength: 128),
                        ProjectID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ProjectCommentID)
                .ForeignKey("dbo.tblProject", t => t.ProjectID, cascadeDelete: true)
                .Index(t => t.ProjectID);
            
            CreateTable(
                "dbo.tblProjectMember",
                c => new
                    {
                        MemberID = c.String(nullable: false, maxLength: 128),
                        ProjectID = c.Int(nullable: false),
                        ProjectRole = c.String(nullable: false, maxLength: 6),
                        PercentInProject = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => new { t.MemberID, t.ProjectID })
                .ForeignKey("dbo.tblProject", t => t.ProjectID, cascadeDelete: true)
                .ForeignKey("dbo.tblCommon", t => t.ProjectRole, cascadeDelete: true)
                .Index(t => t.ProjectID)
                .Index(t => t.ProjectRole);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.tblProjectAttachment", "ProjectID", "dbo.tblProject");
            DropForeignKey("dbo.tblProject", "ProjectStatus", "dbo.tblCommon");
            DropForeignKey("dbo.tblProject", "ProjectSize", "dbo.tblCommon");
            DropForeignKey("dbo.tblProjectMember", "ProjectRole", "dbo.tblCommon");
            DropForeignKey("dbo.tblProjectMember", "ProjectID", "dbo.tblProject");
            DropForeignKey("dbo.tbProjectComment", "ProjectID", "dbo.tblProject");
            DropForeignKey("dbo.tblProject", "CompnayID", "dbo.tblCompany");
            DropForeignKey("dbo.tblProject", "CommID", "dbo.tblCommunity");
            DropForeignKey("dbo.tblProject", "Category", "dbo.tblCommon");
            DropForeignKey("dbo.tblUsersPhoto", "UserID", "dbo.ExtendAspNetUsers");
            DropForeignKey("dbo.ExtendAspNetUsers", "SpecialtyType", "dbo.tblCommon");
            DropForeignKey("dbo.ExtendAspNetUsers", "UserID", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.tblCommon", "ComSubCode", "dbo.tblCommon");
            DropForeignKey("dbo.tblCommentAttachment", "ComentID", "dbo.tblCommunityComment");
            DropForeignKey("dbo.tblCommunityComment", "CommID", "dbo.tblCommunity");
            DropForeignKey("dbo.tblCommunityAttachment", "CommID", "dbo.tblCommunity");
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.tblProjectMember", new[] { "ProjectRole" });
            DropIndex("dbo.tblProjectMember", new[] { "ProjectID" });
            DropIndex("dbo.tbProjectComment", new[] { "ProjectID" });
            DropIndex("dbo.tblProject", new[] { "CompnayID" });
            DropIndex("dbo.tblProject", new[] { "ProjectStatus" });
            DropIndex("dbo.tblProject", new[] { "ProjectSize" });
            DropIndex("dbo.tblProject", new[] { "Category" });
            DropIndex("dbo.tblProject", new[] { "CommID" });
            DropIndex("dbo.tblProjectAttachment", new[] { "ProjectID" });
            DropIndex("dbo.tblUsersPhoto", new[] { "UserID" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.ExtendAspNetUsers", new[] { "SpecialtyType" });
            DropIndex("dbo.ExtendAspNetUsers", new[] { "UserID" });
            DropIndex("dbo.tblCommon", new[] { "ComSubCode" });
            DropIndex("dbo.tblCommunityAttachment", new[] { "CommID" });
            DropIndex("dbo.tblCommunityComment", new[] { "CommID" });
            DropIndex("dbo.tblCommentAttachment", new[] { "ComentID" });
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.tblProjectMember");
            DropTable("dbo.tbProjectComment");
            DropTable("dbo.tblProject");
            DropTable("dbo.tblProjectAttachment");
            DropTable("dbo.tblUsersPhoto");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.ExtendAspNetUsers");
            DropTable("dbo.tblCompany");
            DropTable("dbo.tblCommon");
            DropTable("dbo.tblCommunityAttachment");
            DropTable("dbo.tblCommunity");
            DropTable("dbo.tblCommunityComment");
            DropTable("dbo.tblCommentAttachment");
        }
    }
}
