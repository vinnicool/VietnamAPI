namespace CloudApiVietnam.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class isdeletedreadded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Formulierens", "IsDeleted", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Formulierens", "IsDeleted");
        }
    }
}
