namespace Trace.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitailProject2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.traceability_logs", "Result", c => c.String(unicode: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.traceability_logs", "Result");
        }
    }
}
