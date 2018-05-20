namespace TaskManagerDAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixPerson : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.People", "TeamId", "dbo.Teams");
            DropIndex("dbo.People", new[] { "TeamId" });
            AlterColumn("dbo.People", "TeamId", c => c.Int());
            CreateIndex("dbo.People", "TeamId");
            AddForeignKey("dbo.People", "TeamId", "dbo.Teams", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.People", "TeamId", "dbo.Teams");
            DropIndex("dbo.People", new[] { "TeamId" });
            AlterColumn("dbo.People", "TeamId", c => c.Int(nullable: false));
            CreateIndex("dbo.People", "TeamId");
            AddForeignKey("dbo.People", "TeamId", "dbo.Teams", "Id", cascadeDelete: true);
        }
    }
}
