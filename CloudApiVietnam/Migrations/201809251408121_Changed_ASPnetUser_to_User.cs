namespace CloudApiVietnam.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Changed_ASPnetUser_to_User : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.AspNetUsers", newName: "Users");
        }
        
        public override void Down()
        {
            RenameTable(name: "dbo.Users", newName: "AspNetUsers");
        }
    }
}
