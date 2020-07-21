namespace Trace.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Update101 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.part_assemblies", "LineNumber", c => c.Int(nullable: false));
            AddColumn("dbo.tightening_results", "RepairFlag", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.tightening_results", "RepairFlag");
            DropColumn("dbo.part_assemblies", "LineNumber");
        }
    }
}
