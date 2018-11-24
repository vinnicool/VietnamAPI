namespace CloudApiVietnam.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class cleanupUsersTable : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Users", "EmailConfirmed");
            DropColumn("dbo.Users", "PhoneNumberConfirmed");
            DropColumn("dbo.Users", "TwoFactorEnabled");
            DropColumn("dbo.Users", "LockoutEndDateUtc");
            DropColumn("dbo.Users", "LockoutEnabled");
            DropColumn("dbo.Users", "AccessFailedCount");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Users", "AccessFailedCount", c => c.Int(nullable: false));
            AddColumn("dbo.Users", "LockoutEnabled", c => c.Boolean(nullable: false));
            AddColumn("dbo.Users", "LockoutEndDateUtc", c => c.DateTime());
            AddColumn("dbo.Users", "TwoFactorEnabled", c => c.Boolean(nullable: false));
            AddColumn("dbo.Users", "PhoneNumberConfirmed", c => c.Boolean(nullable: false));
            AddColumn("dbo.Users", "EmailConfirmed", c => c.Boolean(nullable: false));
        }
    }
}
