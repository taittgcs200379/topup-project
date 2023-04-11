namespace topup_project.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class idea2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Ideas", "IsAccepted", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Ideas", "IsAccepted");
        }
    }
}
