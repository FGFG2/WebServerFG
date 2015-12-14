namespace WebServer.DataContext
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.LogEntries",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TimeStamp = c.DateTime(nullable: false),
                        Message = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.SmartPlaneUsers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ReleatedApplicationUserId = c.String(),
                        RankingPoints = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Achievements",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Description = c.String(),
                        ImageUrl = c.String(),
                        Progress = c.Byte(nullable: false),
                        SmartPlaneUser_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.SmartPlaneUsers", t => t.SmartPlaneUser_Id)
                .Index(t => t.SmartPlaneUser_Id);
            
            CreateTable(
                "dbo.ConnectedDatas",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TimeStamp = c.Long(nullable: false),
                        Value = c.Boolean(nullable: false),
                        SmartPlaneUser_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.SmartPlaneUsers", t => t.SmartPlaneUser_Id)
                .Index(t => t.SmartPlaneUser_Id);
            
            CreateTable(
                "dbo.MotorDatas",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TimeStamp = c.Long(nullable: false),
                        Value = c.Int(nullable: false),
                        SmartPlaneUser_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.SmartPlaneUsers", t => t.SmartPlaneUser_Id)
                .Index(t => t.SmartPlaneUser_Id);
            
            CreateTable(
                "dbo.RudderDatas",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TimeStamp = c.Long(nullable: false),
                        Value = c.Int(nullable: false),
                        SmartPlaneUser_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.SmartPlaneUsers", t => t.SmartPlaneUser_Id)
                .Index(t => t.SmartPlaneUser_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RudderDatas", "SmartPlaneUser_Id", "dbo.SmartPlaneUsers");
            DropForeignKey("dbo.MotorDatas", "SmartPlaneUser_Id", "dbo.SmartPlaneUsers");
            DropForeignKey("dbo.ConnectedDatas", "SmartPlaneUser_Id", "dbo.SmartPlaneUsers");
            DropForeignKey("dbo.Achievements", "SmartPlaneUser_Id", "dbo.SmartPlaneUsers");
            DropIndex("dbo.RudderDatas", new[] { "SmartPlaneUser_Id" });
            DropIndex("dbo.MotorDatas", new[] { "SmartPlaneUser_Id" });
            DropIndex("dbo.ConnectedDatas", new[] { "SmartPlaneUser_Id" });
            DropIndex("dbo.Achievements", new[] { "SmartPlaneUser_Id" });
            DropTable("dbo.RudderDatas");
            DropTable("dbo.MotorDatas");
            DropTable("dbo.ConnectedDatas");
            DropTable("dbo.Achievements");
            DropTable("dbo.SmartPlaneUsers");
            DropTable("dbo.LogEntries");
        }
    }
}
