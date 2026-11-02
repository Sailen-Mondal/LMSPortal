using System;
using System.Data.Entity;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LMSPortal.Migrations;
using LMSPortal.Models;

namespace LMSPortal.Tests
{
    [TestClass]
    public class DatabaseInitializationTests
    {
        [TestMethod]
        public void Database_InitializeAndSeed_PopulatesAllEntitiesAndSeedData()
        {
            // Configure EF to migrate database to latest version and run seed
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<ApplicationDbContext, Configuration>());

            using (var db = new ApplicationDbContext())
            {
                // Force database initialization & seeder execution
                db.Database.Initialize(force: true);

                // Assert 1: Roles
                Assert.IsTrue(db.Roles.Any(r => r.Name == "Admin"), "Admin role should be seeded.");
                Assert.IsTrue(db.Roles.Any(r => r.Name == "Student"), "Student role should be seeded.");

                // Assert 2: Users
                var adminUser = db.Users.FirstOrDefault(u => u.Email == "admin@lmsportal.local");
                Assert.IsNotNull(adminUser, "Admin user should be seeded.");

                var studentUser = db.Users.FirstOrDefault(u => u.Email == "alex@lmsportal.local");
                Assert.IsNotNull(studentUser, "Student user should be seeded.");

                // Assert 3: Categories
                var categories = db.Categories.ToList();
                Assert.IsTrue(categories.Count >= 4, "At least 4 categories should be seeded.");

                // Assert 4: Courses & Curriculum
                var courses = db.Courses.Include(c => c.Modules.Select(m => m.Lessons)).ToList();
                Assert.IsTrue(courses.Count >= 6, "At least 6 courses should be seeded.");
                Assert.IsTrue(courses.All(c => c.Modules.Any()), "Each course should have curriculum modules.");
                Assert.IsTrue(courses.Any(c => c.Modules.Any(m => m.Lessons.Any(l => l.IsFreePreview))), "Free preview lessons should exist.");

                // Assert 5: Enrollments
                var enrollments = db.Enrollments.Where(e => e.UserId == studentUser.Id).ToList();
                Assert.IsTrue(enrollments.Any(), "Sample student should have seeded enrollments.");

                // Assert 6: Lesson Completions
                var completions = db.LessonCompletions.Where(lc => lc.UserId == studentUser.Id).ToList();
                Assert.IsTrue(completions.Any(), "Sample student should have seeded lesson completions.");
            }
        }
    }
}
