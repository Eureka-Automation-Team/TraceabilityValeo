namespace Trace.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Addcolumnonmachine : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.machines", "RequestVerifyCode", c => c.Boolean(nullable: false));
            AddColumn("dbo.machines", "CodeVerify", c => c.String(unicode: false));
            AddColumn("dbo.machines", "CodeVerifyResult", c => c.Int(nullable: false));
            AlterColumn("dbo.machines", "OnlineFlag", c => c.Int(nullable: false));
            AlterColumn("dbo.machines", "CompletedLogging", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.machines", "CompletedLogging", c => c.Boolean(nullable: false));
            AlterColumn("dbo.machines", "OnlineFlag", c => c.Boolean(nullable: false));
            DropColumn("dbo.machines", "CodeVerifyResult");
            DropColumn("dbo.machines", "CodeVerify");
            DropColumn("dbo.machines", "RequestVerifyCode");
        }
    }
}
