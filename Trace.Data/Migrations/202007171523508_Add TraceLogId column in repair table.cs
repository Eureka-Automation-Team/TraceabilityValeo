namespace Trace.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTraceLogIdcolumninrepairtable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.tightening_repairs", "TraceLogId", c => c.Int(nullable: false));
            CreateIndex("dbo.tightening_repairs", "TraceLogId");
            AddForeignKey("dbo.tightening_repairs", "TraceLogId", "dbo.traceability_logs", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.tightening_repairs", "TraceLogId", "dbo.traceability_logs");
            DropIndex("dbo.tightening_repairs", new[] { "TraceLogId" });
            DropColumn("dbo.tightening_repairs", "TraceLogId");
        }
    }
}
