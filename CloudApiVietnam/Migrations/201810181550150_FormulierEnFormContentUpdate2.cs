namespace CloudApiVietnam.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FormulierEnFormContentUpdate2 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.FormContents", "Formulieren_Id", "dbo.Formulierens");
            DropIndex("dbo.FormContents", new[] { "Formulieren_Id" });
            RenameColumn(table: "dbo.FormContents", name: "Formulieren_Id", newName: "FormulierenId");
            AlterColumn("dbo.FormContents", "FormulierenId", c => c.Int(nullable: false));
            CreateIndex("dbo.FormContents", "FormulierenId");
            AddForeignKey("dbo.FormContents", "FormulierenId", "dbo.Formulierens", "Id", cascadeDelete: true);
            DropColumn("dbo.FormContents", "FormId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.FormContents", "FormId", c => c.Int(nullable: false));
            DropForeignKey("dbo.FormContents", "FormulierenId", "dbo.Formulierens");
            DropIndex("dbo.FormContents", new[] { "FormulierenId" });
            AlterColumn("dbo.FormContents", "FormulierenId", c => c.Int());
            RenameColumn(table: "dbo.FormContents", name: "FormulierenId", newName: "Formulieren_Id");
            CreateIndex("dbo.FormContents", "Formulieren_Id");
            AddForeignKey("dbo.FormContents", "Formulieren_Id", "dbo.Formulierens", "Id");
        }
    }
}
