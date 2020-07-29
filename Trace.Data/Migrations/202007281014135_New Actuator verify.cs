namespace Trace.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NewActuatorverify : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.machines", "RequestCodeActuater", c => c.Boolean(nullable: false));
            AddColumn("dbo.machines", "ActuatorResult", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.machines", "ActuatorResult");
            DropColumn("dbo.machines", "RequestCodeActuater");
        }
    }
}
