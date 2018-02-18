namespace DailyAww.Migrations
{
    using Microsoft.AspNet.Identity;
    using Models;
    using System;
    using System.Configuration;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<DailyAww.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "DailyAww.Models.ApplicationDbContext";
        }

        protected override void Seed(DailyAww.Models.ApplicationDbContext context)
        {
            var passwordHash = new PasswordHasher();
            string password = passwordHash.HashPassword("DontUseBadPasswords123!");
            var defaultUserName = ConfigurationManager.AppSettings["ServiceAddress"];
            context.Users.AddOrUpdate(u => u.UserName, new ApplicationUser
            {
                UserName = defaultUserName,
                PasswordHash = password
            });
            context.SaveChanges();
        }
    }
}
