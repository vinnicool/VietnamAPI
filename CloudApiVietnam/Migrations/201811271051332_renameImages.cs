namespace CloudApiVietnam.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class renameImages : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Images", "ImageData", c => c.Binary());
            DropColumn("dbo.Images", "image");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Images", "image", c => c.Binary());
            DropColumn("dbo.Images", "ImageData");
        }
    }
}
