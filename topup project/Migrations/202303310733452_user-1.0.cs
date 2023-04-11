namespace topup_project.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class user10 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "DepartmentId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "DepartmentId");
        }
    }
}
