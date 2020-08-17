namespace Trace.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NewModelrunning : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.traceability_logs", "ModelRunningFlag", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.traceability_logs", "ModelRunningFlag");
        }
    }
}
