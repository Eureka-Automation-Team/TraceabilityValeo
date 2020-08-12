namespace Trace.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddrequiredfieldinPLCTagtable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.plc_tags", "RequiredFlag", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.plc_tags", "RequiredFlag");
        }
    }
}
