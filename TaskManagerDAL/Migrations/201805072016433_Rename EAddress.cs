namespace TaskManagerDAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenameEAddress : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.People", "TeamId", "dbo.Teams");
            DropIndex("dbo.People", new[] { "TeamId" });
            RenameColumn(table: "dbo.People", name: "TeamId", newName: "Team_Id");
            AddColumn("dbo.People", "Email", c => c.String());
            AlterColumn("dbo.People", "Team_Id", c => c.Int());
            CreateIndex("dbo.People", "Team_Id");
            AddForeignKey("dbo.People", "Team_Id", "dbo.Teams", "Id");
            DropColumn("dbo.People", "EAdress");
        }
        
        public override void Down()
        {
            AddColumn("dbo.People", "EAdress", c => c.String());
            DropForeignKey("dbo.People", "Team_Id", "dbo.Teams");
            DropIndex("dbo.People", new[] { "Team_Id" });
            AlterColumn("dbo.People", "Team_Id", c => c.Int(nullable: false));
            DropColumn("dbo.People", "Email");
            RenameColumn(table: "dbo.People", name: "Team_Id", newName: "TeamId");
            CreateIndex("dbo.People", "TeamId");
            AddForeignKey("dbo.People", "TeamId", "dbo.Teams", "Id", cascadeDelete: true);
        }
    }
}
