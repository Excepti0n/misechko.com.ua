namespace misechko.com.data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Test : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TeamMember",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Headline = c.String(),
                        LinkPath = c.String(),
                        PublishDate = c.DateTime(nullable: false),
                        Culture = c.String(),
                        ListWeight = c.Int(nullable: false),
                        Hidden = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AboutMenuItem",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Headline = c.String(),
                        LinkPath = c.String(),
                        PublishDate = c.DateTime(nullable: false),
                        Culture = c.String(),
                        ListWeight = c.Int(nullable: false),
                        Hidden = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Project", "Hidden", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Industry", "Hidden");
            DropColumn("dbo.Practice", "Hidden");
            DropColumn("dbo.LawNew", "Hidden");
            DropColumn("dbo.New", "Hidden");
            DropColumn("dbo.Brochure", "Hidden");
            DropColumn("dbo.Publication", "Hidden");
            DropColumn("dbo.Award", "Hidden");
            DropColumn("dbo.Project", "Hidden");
            DropTable("dbo.AboutMenuItem");
            DropTable("dbo.TeamMember");
        }
    }
}
