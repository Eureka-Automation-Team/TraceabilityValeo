namespace Trace.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Addtightening_repairstable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.tightening_repairs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        No = c.String(nullable: false, unicode: false),
                        Min = c.Decimal(nullable: false, precision: 15, scale: 2),
                        Max = c.Decimal(nullable: false, precision: 15, scale: 2),
                        Target = c.Decimal(nullable: false, precision: 15, scale: 2),
                        Result = c.Decimal(nullable: false, precision: 15, scale: 2),
                        TestResult = c.String(maxLength: 10, storeType: "nvarchar"),
                        JointMin = c.Decimal(nullable: false, precision: 15, scale: 2),
                        JointMax = c.Decimal(nullable: false, precision: 15, scale: 2),
                        JointTarget = c.Decimal(nullable: false, precision: 15, scale: 2),
                        JointResult = c.Decimal(nullable: false, precision: 15, scale: 2),
                        JointTestResult = c.String(maxLength: 10, storeType: "nvarchar"),
                        TighteningResultId = c.Int(nullable: false),
                        LastUpdateDate = c.DateTime(nullable: false, precision: 0),
                        LastUpdatedBy = c.Int(nullable: false),
                        CreationDate = c.DateTime(nullable: false, precision: 0),
                        CreatedBy = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.tightening_results", t => t.TighteningResultId, cascadeDelete: true)
                .Index(t => t.TighteningResultId);
            
            AddColumn("dbo.traceability_logs", "FinishFlag", c => c.Boolean(nullable: false));
            AddColumn("dbo.tightening_results", "JointTestResult", c => c.String(maxLength: 10, storeType: "nvarchar"));
            AlterColumn("dbo.tightening_results", "TestResult", c => c.String(maxLength: 10, storeType: "nvarchar"));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.tightening_repairs", "TighteningResultId", "dbo.tightening_results");
            DropIndex("dbo.tightening_repairs", new[] { "TighteningResultId" });
            AlterColumn("dbo.tightening_results", "TestResult", c => c.String(unicode: false));
            DropColumn("dbo.tightening_results", "JointTestResult");
            DropColumn("dbo.traceability_logs", "FinishFlag");
            DropTable("dbo.tightening_repairs");
        }
    }
}
