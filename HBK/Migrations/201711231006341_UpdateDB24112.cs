namespace HBK.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateDB24112 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.tblProduct", "ProductCategoryId", c => c.Int(nullable: false));
            CreateIndex("dbo.tblProduct", "ProductCategoryId");
            AddForeignKey("dbo.tblProduct", "ProductCategoryId", "dbo.tblProductCategory", "ProductCategoryId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.tblProduct", "ProductCategoryId", "dbo.tblProductCategory");
            DropIndex("dbo.tblProduct", new[] { "ProductCategoryId" });
            DropColumn("dbo.tblProduct", "ProductCategoryId");
        }
    }
}
