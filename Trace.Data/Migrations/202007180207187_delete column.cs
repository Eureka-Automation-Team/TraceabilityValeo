namespace Trace.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class deletecolumn : DbMigration
    {
        public override void Up()
        {
            //DropForeignKey("dbo.tightening_repairs", "TraceLogId", "dbo.traceability_logs");
            //DropIndex("dbo.tightening_repairs", new[] { "TraceLogId" });
            //DropColumn("dbo.tightening_repairs", "TraceLogId");
        }
        
        public override void Down()
        {
            //AddColumn("dbo.tightening_repairs", "TraceLogId", c => c.Int(nullable: false));
            //CreateIndex("dbo.tightening_repairs", "TraceLogId");
            //AddForeignKey("dbo.tightening_repairs", "TraceLogId", "dbo.traceability_logs", "Id", cascadeDelete: true);
        }
    }
}
