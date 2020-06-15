namespace Trace.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Alterdatatypetraceabilitylog : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.traceability_logs", "FinalResult", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.traceability_logs", "FinalResult", c => c.Boolean(nullable: false));
        }
    }
}
