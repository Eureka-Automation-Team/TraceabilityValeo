namespace Trace.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Addjigcolumn : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.traceability_logs", "Jig", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.traceability_logs", "Jig");
        }
    }
}
