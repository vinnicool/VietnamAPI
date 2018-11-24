namespace CloudApiVietnam.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FormulierEnFormContentGemaakt : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.FormContents",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FormId = c.String(),
                        Content = c.String(),
                        Formulieren_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Formulierens", t => t.Formulieren_Id)
                .Index(t => t.Formulieren_Id);
            
            CreateTable(
                "dbo.Formulierens",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Region = c.String(),
                        FormTemplate = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.FormContents", "Formulieren_Id", "dbo.Formulierens");
            DropIndex("dbo.FormContents", new[] { "Formulieren_Id" });
            DropTable("dbo.Formulierens");
            DropTable("dbo.FormContents");
        }
    }
}
