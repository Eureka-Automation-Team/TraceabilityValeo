namespace Trace.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddLengthcolumntoPLCTagtable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.plc_tags", "Length", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.plc_tags", "Length");
        }
    }
}
