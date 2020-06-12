namespace Trace.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Altercolumntype : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.tightening_results", "No", c => c.String(nullable: false, unicode: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.tightening_results", "No", c => c.Int(nullable: false));
        }
    }
}
