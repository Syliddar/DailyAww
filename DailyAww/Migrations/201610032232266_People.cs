namespace DailyAww.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class People : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.Employees", newName: "People");
        }
        
        public override void Down()
        {
            RenameTable(name: "dbo.People", newName: "Employees");
        }
    }
}
