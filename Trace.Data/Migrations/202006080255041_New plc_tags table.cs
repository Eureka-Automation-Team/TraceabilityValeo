namespace Trace.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Newplc_tagstable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.plc_tags",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PlcTag = c.String(nullable: false, maxLength: 250, storeType: "nvarchar"),
                        Description = c.String(maxLength: 250, storeType: "nvarchar"),
                        ReadAbleFlag = c.Boolean(nullable: false),
                        WriteAbleFlag = c.Boolean(nullable: false),
                        DataType = c.String(nullable: false, maxLength: 50, storeType: "nvarchar"),
                        TypeCode = c.String(nullable: false, maxLength: 100, storeType: "nvarchar"),
                        MachineId = c.Int(nullable: false),
                        StationId = c.Int(nullable: false),
                        LastUpdateDate = c.DateTime(nullable: false, precision: 0),
                        LastUpdatedBy = c.Int(nullable: false),
                        CreationDate = c.DateTime(nullable: false, precision: 0),
                        CreatedBy = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.machines", t => t.MachineId, cascadeDelete: true)
                .ForeignKey("dbo.stations", t => t.StationId, cascadeDelete: true)
                .Index(t => t.MachineId)
                .Index(t => t.StationId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.plc_tags", "StationId", "dbo.stations");
            DropForeignKey("dbo.plc_tags", "MachineId", "dbo.machines");
            DropIndex("dbo.plc_tags", new[] { "StationId" });
            DropIndex("dbo.plc_tags", new[] { "MachineId" });
            DropTable("dbo.plc_tags");
        }
    }
}
