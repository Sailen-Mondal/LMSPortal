namespace LMSPortal.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using LMSPortal.Models;

    public sealed class Configuration : DbMigrationsConfiguration<LMSPortal.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(LMSPortal.Models.ApplicationDbContext context)
        {
            DatabaseSeeder.Seed(context);
        }
    }
}
