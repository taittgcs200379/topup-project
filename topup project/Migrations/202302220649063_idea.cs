namespace topup_project.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class idea : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Ideas",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Text = c.String(),
                        FilePath = c.String(),
                        DateTime = c.DateTime(),
                        UserId = c.String(),
                        CategoryId = c.Int(nullable: false),
                        TopicId = c.Int(nullable: false),
                        
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Ideas");
        }
    }
}
