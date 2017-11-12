namespace BEN_Community.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Mofify_Database : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.tbCompany", newName: "tblCompany");
            AddColumn("dbo.tblCommon", "CommonType", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.tblCommon", "CommonType");
            RenameTable(name: "dbo.tblCompany", newName: "tbCompany");
        }
    }
}
