namespace Trace.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTighteningpositioncolumn : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.machines", "TighteningPosition", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.machines", "TighteningPosition");
        }
    }
}
