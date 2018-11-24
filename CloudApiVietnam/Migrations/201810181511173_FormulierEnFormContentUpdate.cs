namespace CloudApiVietnam.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FormulierEnFormContentUpdate : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.FormContents", "FormId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.FormContents", "FormId", c => c.String());
        }
    }
}
