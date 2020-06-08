namespace Trace.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Updatemachinetable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.machines", "RequestLogging", c => c.Boolean(nullable: false));
            AddColumn("dbo.machines", "CompletedLogging", c => c.Boolean(nullable: false));
            AddColumn("dbo.stations", "Description", c => c.String(unicode: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.stations", "Description");
            DropColumn("dbo.machines", "CompletedLogging");
            DropColumn("dbo.machines", "RequestLogging");
        }
    }
}
