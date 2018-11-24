namespace CloudApiVietnam.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IgnoreUserLogins : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Users", "FirstName");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Users", "FirstName", c => c.String());
        }
    }
}
