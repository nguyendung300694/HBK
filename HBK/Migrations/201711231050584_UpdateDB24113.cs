namespace HBK.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateDB24113 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.tblProductCategoryExtend", "FileSize", c => c.Int());
            AlterColumn("dbo.tblProductExtend", "FileSize", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.tblProductExtend", "FileSize", c => c.String(maxLength: 100));
            AlterColumn("dbo.tblProductCategoryExtend", "FileSize", c => c.String(maxLength: 100));
        }
    }
}
