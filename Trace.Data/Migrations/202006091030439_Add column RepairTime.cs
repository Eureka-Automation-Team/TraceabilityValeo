namespace Trace.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddcolumnRepairTime : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.traceability_logs", "RepairTime", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.traceability_logs", "RepairTime");
        }
    }
}
