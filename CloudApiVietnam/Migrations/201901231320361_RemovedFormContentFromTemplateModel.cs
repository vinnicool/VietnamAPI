namespace CloudApiVietnam.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemovedFormContentFromTemplateModel : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Formulierens", "IsDeleted");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Formulierens", "IsDeleted", c => c.Boolean(nullable: false));
        }
    }
}
