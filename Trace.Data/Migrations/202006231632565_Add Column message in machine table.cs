namespace Trace.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddColumnmessageinmachinetable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.machines", "MessageResult", c => c.String(unicode: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.machines", "MessageResult");
        }
    }
}
