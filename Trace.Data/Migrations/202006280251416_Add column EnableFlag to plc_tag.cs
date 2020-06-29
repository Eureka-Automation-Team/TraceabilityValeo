namespace Trace.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddcolumnEnableFlagtoplc_tag : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.plc_tags", "EnableFlag", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.plc_tags", "EnableFlag");
        }
    }
}
