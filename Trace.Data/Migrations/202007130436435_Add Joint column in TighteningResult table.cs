namespace Trace.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddJointcolumninTighteningResulttable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.tightening_results", "JointMin", c => c.Decimal(nullable: false, precision: 15, scale: 2));
            AddColumn("dbo.tightening_results", "JointMax", c => c.Decimal(nullable: false, precision: 15, scale: 2));
            AddColumn("dbo.tightening_results", "JointTarget", c => c.Decimal(nullable: false, precision: 15, scale: 2));
            AddColumn("dbo.tightening_results", "JointResult", c => c.Decimal(nullable: false, precision: 15, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.tightening_results", "JointResult");
            DropColumn("dbo.tightening_results", "JointTarget");
            DropColumn("dbo.tightening_results", "JointMax");
            DropColumn("dbo.tightening_results", "JointMin");
        }
    }
}
