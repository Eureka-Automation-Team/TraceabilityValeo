namespace Trace.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Altercolumn : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.tightening_results", "TestResult", c => c.String(unicode: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.tightening_results", "TestResult", c => c.Boolean(nullable: false));
        }
    }
}
