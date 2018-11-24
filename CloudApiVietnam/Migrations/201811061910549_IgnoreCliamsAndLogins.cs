namespace CloudApiVietnam.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IgnoreCliamsAndLogins : DbMigration
    {
        public override void Up()
        {
        }
        
        public override void Down()
        {
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUserLogins");
        }
    }
}
