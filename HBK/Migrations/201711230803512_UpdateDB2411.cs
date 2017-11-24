namespace HBK.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateDB2411 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.tblProduct", "View", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.tblProduct", "View");
        }
    }
}
