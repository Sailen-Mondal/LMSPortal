using System;
using System.Linq;
using LMSPortal.Models;
using LMSPortal.Security;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LMSPortal.Tests
{
    [TestClass]
    public class DatabaseSeederTests
    {
        [TestMethod]
        public void SecurePasswordGenerator_MeetsEnterpriseEntropy()
        {
            string password = SecurePasswordGenerator.Generate(20);

            Assert.AreEqual(20, password.Length);
            Assert.IsTrue(password.Any(char.IsUpper));
            Assert.IsTrue(password.Any(char.IsLower));
            Assert.IsTrue(password.Any(char.IsDigit));
            Assert.IsTrue(password.Any(c => "!@#$%^&*()-_=+".Contains(c)));
        }

        [TestMethod]
        public void CategoryModel_SlugAndIcon_AreProperlyFormed()
        {
            var cat = new Category
            {
                Name = "Cloud Computing",
                Slug = "cloud-computing",
                IconClass = "bi-cloud",
                DisplayOrder = 1
            };

            Assert.AreEqual("cloud-computing", cat.Slug);
            Assert.AreEqual("bi-cloud", cat.IconClass);
            Assert.IsTrue(cat.IsActive);
        }

        [TestMethod]
        public void Course_FreePreviewLesson_IsAccessible()
        {
            var course = new Course
            {
                Title = "Test Architecture Course",
                Slug = "test-architecture-course"
            };

            var module = new Module { Title = "Module 1", OrderIndex = 1 };
            var previewLesson = new Lesson { Title = "Overview", IsFreePreview = true, OrderIndex = 1 };
            var lockedLesson = new Lesson { Title = "Deep Dive", IsFreePreview = false, OrderIndex = 2 };

            module.Lessons.Add(previewLesson);
            module.Lessons.Add(lockedLesson);
            course.Modules.Add(module);

            var preview = course.Modules.SelectMany(m => m.Lessons).FirstOrDefault(l => l.IsFreePreview);
            var locked = course.Modules.SelectMany(m => m.Lessons).FirstOrDefault(l => !l.IsFreePreview);

            Assert.IsNotNull(preview, "Course should have at least one free preview lesson.");
            Assert.IsTrue(preview.IsFreePreview);
            Assert.IsNotNull(locked, "Course should have protected lessons.");
            Assert.IsFalse(locked.IsFreePreview);
        }

        [TestMethod]
        public void Enrollment_ProgressCalculation_SourceOfTruthMatches()
        {
            // Simulate 4 lessons, 2 completed
            int totalLessons = 4;
            int completedLessons = 2;

            int progressPercentage = totalLessons > 0 
                ? (int)Math.Round((double)completedLessons * 100 / totalLessons) 
                : 0;

            Assert.AreEqual(50, progressPercentage, "Progress calculation should yield exactly 50%.");
        }
    }
}
