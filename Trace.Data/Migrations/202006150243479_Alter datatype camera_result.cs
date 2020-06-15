namespace Trace.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Alterdatatypecamera_result : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.camera_results", "TestResult", c => c.String(unicode: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.camera_results", "TestResult", c => c.Boolean(nullable: false));
        }
    }
}
