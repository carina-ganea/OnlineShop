namespace OnlineShop.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Actualizare : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Categories",
                c => new
                    {
                        IdCategorie = c.Int(nullable: false, identity: true),
                        Denumire = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.IdCategorie);
            
            CreateTable(
                "dbo.Produs",
                c => new
                    {
                        IdProdus = c.Int(nullable: false, identity: true),
                        NumeProdus = c.String(nullable: false),
                        Pret = c.Single(nullable: false),
                        Descriere = c.String(),
                        IdCategorie = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.IdProdus)
                .ForeignKey("dbo.Categories", t => t.IdCategorie, cascadeDelete: true)
                .Index(t => t.IdCategorie);
            
            CreateTable(
                "dbo.Reviews",
                c => new
                    {
                        IdReview = c.Int(nullable: false, identity: true),
                        Continut = c.String(nullable: false),
                        IdUser = c.Int(nullable: false),
                        IdProdus = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.IdReview)
                .ForeignKey("dbo.Produs", t => t.IdProdus, cascadeDelete: true)
                .Index(t => t.IdProdus);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Reviews", "IdProdus", "dbo.Produs");
            DropForeignKey("dbo.Produs", "IdCategorie", "dbo.Categories");
            DropIndex("dbo.Reviews", new[] { "IdProdus" });
            DropIndex("dbo.Produs", new[] { "IdCategorie" });
            DropTable("dbo.Reviews");
            DropTable("dbo.Produs");
            DropTable("dbo.Categories");
        }
    }
}
