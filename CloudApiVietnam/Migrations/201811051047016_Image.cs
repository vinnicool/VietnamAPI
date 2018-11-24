namespace CloudApiVietnam.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Image : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Images",
                c => new
                    {
                        name = c.String(nullable: false, maxLength: 128),
                        image = c.Binary(),
                    })
                .PrimaryKey(t => t.name);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Images");
        }
    }
}
