namespace Trace.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitailProject : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.camera_results",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CameraName = c.String(nullable: false, maxLength: 250, storeType: "nvarchar"),
                        TestResult = c.Boolean(nullable: false),
                        TraceLogId = c.Int(nullable: false),
                        LastUpdateDate = c.DateTime(nullable: false, precision: 0),
                        LastUpdatedBy = c.Int(nullable: false),
                        CreationDate = c.DateTime(nullable: false, precision: 0),
                        CreatedBy = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.traceability_logs", t => t.TraceLogId, cascadeDelete: true)
                .Index(t => t.TraceLogId);
            
            CreateTable(
                "dbo.traceability_logs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        StationId = c.Int(nullable: false),
                        MachineId = c.Int(nullable: false),
                        ItemCode = c.String(nullable: false, maxLength: 100, storeType: "nvarchar"),
                        ModelRunning = c.String(maxLength: 100, storeType: "nvarchar"),
                        Description = c.String(maxLength: 500, storeType: "nvarchar"),
                        PartSerialNumber = c.String(maxLength: 200, storeType: "nvarchar"),
                        Actuator = c.String(maxLength: 100, storeType: "nvarchar"),
                        ProductionDate = c.DateTime(nullable: false, precision: 0),
                        SwNumber = c.String(maxLength: 100, storeType: "nvarchar"),
                        CurrentMaximum = c.String(maxLength: 100, storeType: "nvarchar"),
                        OpenAngle = c.String(maxLength: 100, storeType: "nvarchar"),
                        LineErrorCounter = c.String(maxLength: 100, storeType: "nvarchar"),
                        Attribute1 = c.String(maxLength: 100, storeType: "nvarchar"),
                        Attribute2 = c.String(maxLength: 100, storeType: "nvarchar"),
                        Attribute3 = c.String(maxLength: 100, storeType: "nvarchar"),
                        Attribute4 = c.String(maxLength: 100, storeType: "nvarchar"),
                        Attribute5 = c.String(maxLength: 100, storeType: "nvarchar"),
                        Attribute6 = c.String(maxLength: 100, storeType: "nvarchar"),
                        Attribute7 = c.String(maxLength: 100, storeType: "nvarchar"),
                        Attribute8 = c.String(maxLength: 100, storeType: "nvarchar"),
                        Attribute9 = c.String(maxLength: 100, storeType: "nvarchar"),
                        Attribute10 = c.String(maxLength: 100, storeType: "nvarchar"),
                        MaxRepairTime = c.Int(nullable: false),
                        FinalResult = c.Boolean(nullable: false),
                        ImagePath = c.String(maxLength: 500, storeType: "nvarchar"),
                        QRCodePath = c.String(maxLength: 500, storeType: "nvarchar"),
                        LastUpdateDate = c.DateTime(nullable: false, precision: 0),
                        LastUpdatedBy = c.Int(nullable: false),
                        CreationDate = c.DateTime(nullable: false, precision: 0),
                        CreatedBy = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.machines", t => t.MachineId, cascadeDelete: true)
                .ForeignKey("dbo.stations", t => t.StationId, cascadeDelete: true)
                .Index(t => t.StationId)
                .Index(t => t.MachineId);
            
            CreateTable(
                "dbo.machines",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ManchineName = c.String(nullable: false, maxLength: 100, storeType: "nvarchar"),
                        ModelName = c.String(maxLength: 100, storeType: "nvarchar"),
                        StationId = c.Int(nullable: false),
                        OnlineFlag = c.Boolean(nullable: false),
                        LastUpdateDate = c.DateTime(nullable: false, precision: 0),
                        LastUpdatedBy = c.Int(nullable: false),
                        CreationDate = c.DateTime(nullable: false, precision: 0),
                        CreatedBy = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.stations", t => t.StationId, cascadeDelete: true)
                .Index(t => t.StationId);
            
            CreateTable(
                "dbo.stations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        StationNumber = c.String(nullable: false, maxLength: 25, storeType: "nvarchar"),
                        LastUpdateDate = c.DateTime(nullable: false, precision: 0),
                        LastUpdatedBy = c.Int(nullable: false),
                        CreationDate = c.DateTime(nullable: false, precision: 0),
                        CreatedBy = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.part_assemblies",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PartName = c.String(nullable: false, maxLength: 250, storeType: "nvarchar"),
                        SerialNumber = c.String(nullable: false, maxLength: 500, storeType: "nvarchar"),
                        TraceabilityLogId = c.Int(nullable: false),
                        LastUpdateDate = c.DateTime(nullable: false, precision: 0),
                        LastUpdatedBy = c.Int(nullable: false),
                        CreationDate = c.DateTime(nullable: false, precision: 0),
                        CreatedBy = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.traceability_logs", t => t.TraceabilityLogId, cascadeDelete: true)
                .Index(t => t.TraceabilityLogId);
            
            CreateTable(
                "dbo.tightening_results",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        No = c.Int(nullable: false),
                        Min = c.Decimal(nullable: false, precision: 15, scale: 2),
                        Max = c.Decimal(nullable: false, precision: 15, scale: 2),
                        Target = c.Decimal(nullable: false, precision: 15, scale: 2),
                        Result = c.Decimal(nullable: false, precision: 15, scale: 2),
                        TestResult = c.Boolean(nullable: false),
                        TraceLogId = c.Int(nullable: false),
                        LastUpdateDate = c.DateTime(nullable: false, precision: 0),
                        LastUpdatedBy = c.Int(nullable: false),
                        CreationDate = c.DateTime(nullable: false, precision: 0),
                        CreatedBy = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.traceability_logs", t => t.TraceLogId, cascadeDelete: true)
                .Index(t => t.TraceLogId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.tightening_results", "TraceLogId", "dbo.traceability_logs");
            DropForeignKey("dbo.traceability_logs", "StationId", "dbo.stations");
            DropForeignKey("dbo.part_assemblies", "TraceabilityLogId", "dbo.traceability_logs");
            DropForeignKey("dbo.traceability_logs", "MachineId", "dbo.machines");
            DropForeignKey("dbo.machines", "StationId", "dbo.stations");
            DropForeignKey("dbo.camera_results", "TraceLogId", "dbo.traceability_logs");
            DropIndex("dbo.tightening_results", new[] { "TraceLogId" });
            DropIndex("dbo.part_assemblies", new[] { "TraceabilityLogId" });
            DropIndex("dbo.machines", new[] { "StationId" });
            DropIndex("dbo.traceability_logs", new[] { "MachineId" });
            DropIndex("dbo.traceability_logs", new[] { "StationId" });
            DropIndex("dbo.camera_results", new[] { "TraceLogId" });
            DropTable("dbo.tightening_results");
            DropTable("dbo.part_assemblies");
            DropTable("dbo.stations");
            DropTable("dbo.machines");
            DropTable("dbo.traceability_logs");
            DropTable("dbo.camera_results");
        }
    }
}
