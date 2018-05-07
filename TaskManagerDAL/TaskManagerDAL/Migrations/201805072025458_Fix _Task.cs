namespace TaskManagerDAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Fix_Task : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.People", "Team_Id", "dbo.Teams");
            DropForeignKey("dbo._Task", "Assignee_Id1", "dbo.People");
            DropForeignKey("dbo._Task", "Author_Id1", "dbo.People");
            DropForeignKey("dbo._Task", "Status_Id1", "dbo.Status");
            DropIndex("dbo.People", new[] { "Team_Id" });
            DropIndex("dbo._Task", new[] { "Assignee_Id1" });
            DropIndex("dbo._Task", new[] { "Author_Id1" });
            DropIndex("dbo._Task", new[] { "Status_Id1" });
            RenameColumn(table: "dbo.People", name: "Team_Id", newName: "TeamId");
            RenameColumn(table: "dbo._Task", name: "Assignee_Id1", newName: "AssigneeId");
            RenameColumn(table: "dbo._Task", name: "Author_Id1", newName: "AuthorId");
            RenameColumn(table: "dbo._Task", name: "Status_Id1", newName: "StatusId");
            AlterColumn("dbo.People", "TeamId", c => c.Int());
            AlterColumn("dbo._Task", "AssigneeId", c => c.Int(nullable: false));
            AlterColumn("dbo._Task", "AuthorId", c => c.Int(nullable: false));
            AlterColumn("dbo._Task", "StatusId", c => c.Int(nullable: false));
            CreateIndex("dbo.People", "TeamId");
            CreateIndex("dbo._Task", "AuthorId");
            CreateIndex("dbo._Task", "AssigneeId");
            CreateIndex("dbo._Task", "StatusId");
            AddForeignKey("dbo.People", "TeamId", "dbo.Teams", "Id");
            AddForeignKey("dbo._Task", "AssigneeId", "dbo.People", "Id");
            AddForeignKey("dbo._Task", "AuthorId", "dbo.People", "Id");
            AddForeignKey("dbo._Task", "StatusId", "dbo.Status", "Id");
            DropColumn("dbo._Task", "Author_Id");
            DropColumn("dbo._Task", "Assignee_Id");
            DropColumn("dbo._Task", "Status_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo._Task", "Status_Id", c => c.Int(nullable: false));
            AddColumn("dbo._Task", "Assignee_Id", c => c.Int(nullable: false));
            AddColumn("dbo._Task", "Author_Id", c => c.Int(nullable: false));
            DropForeignKey("dbo._Task", "StatusId", "dbo.Status");
            DropForeignKey("dbo._Task", "AuthorId", "dbo.People");
            DropForeignKey("dbo._Task", "AssigneeId", "dbo.People");
            DropForeignKey("dbo.People", "TeamId", "dbo.Teams");
            DropIndex("dbo._Task", new[] { "StatusId" });
            DropIndex("dbo._Task", new[] { "AssigneeId" });
            DropIndex("dbo._Task", new[] { "AuthorId" });
            DropIndex("dbo.People", new[] { "TeamId" });
            AlterColumn("dbo._Task", "StatusId", c => c.Int());
            AlterColumn("dbo._Task", "AuthorId", c => c.Int());
            AlterColumn("dbo._Task", "AssigneeId", c => c.Int());
            AlterColumn("dbo.People", "TeamId", c => c.Int());
            RenameColumn(table: "dbo._Task", name: "StatusId", newName: "Status_Id1");
            RenameColumn(table: "dbo._Task", name: "AuthorId", newName: "Author_Id1");
            RenameColumn(table: "dbo._Task", name: "AssigneeId", newName: "Assignee_Id1");
            RenameColumn(table: "dbo.People", name: "TeamId", newName: "Team_Id");
            CreateIndex("dbo._Task", "Status_Id1");
            CreateIndex("dbo._Task", "Author_Id1");
            CreateIndex("dbo._Task", "Assignee_Id1");
            CreateIndex("dbo.People", "Team_Id");
            AddForeignKey("dbo._Task", "Status_Id1", "dbo.Status", "Id");
            AddForeignKey("dbo._Task", "Author_Id1", "dbo.People", "Id");
            AddForeignKey("dbo._Task", "Assignee_Id1", "dbo.People", "Id");
            AddForeignKey("dbo.People", "Team_Id", "dbo.Teams", "Id");
        }
    }
}
