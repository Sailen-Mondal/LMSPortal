using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using LMSPortal.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LMSPortal.Tests
{
    [TestClass]
    public class DomainModelTests
    {
        [TestMethod]
        public void Category_DefaultInitialization_HasEmptyCoursesCollectionAndIsActive()
        {
            var category = new Category();

            Assert.IsNotNull(category.Courses, "Courses collection should be initialized.");
            Assert.AreEqual(0, category.Courses.Count, "New category should have 0 courses.");
            Assert.IsTrue(category.IsActive, "New category should be active by default.");
        }

        [TestMethod]
        public void Course_DefaultInitialization_SetsDefaultsCorrectly()
        {
            var course = new Course();

            Assert.IsNotNull(course.Modules, "Modules collection should be initialized.");
            Assert.IsNotNull(course.Enrollments, "Enrollments collection should be initialized.");
            Assert.IsTrue(course.IsPublished, "New course should be published by default.");
            Assert.IsFalse(course.IsArchived, "New course should not be archived by default.");
            Assert.AreEqual(CourseLevel.Beginner, course.Level, "Default level should be Beginner.");
            Assert.IsTrue((DateTime.UtcNow - course.CreatedDate).TotalSeconds < 5, "CreatedDate should be UtcNow.");
        }

        [TestMethod]
        public void Module_DefaultInitialization_SetsDefaultsCorrectly()
        {
            var module = new Module();

            Assert.IsNotNull(module.Lessons, "Lessons collection should be initialized.");
            Assert.AreEqual(1, module.OrderIndex, "Default OrderIndex should be 1.");
        }

        [TestMethod]
        public void Lesson_DefaultInitialization_SetsDefaultsCorrectly()
        {
            var lesson = new Lesson();

            Assert.IsNotNull(lesson.Completions, "Completions collection should be initialized.");
            Assert.AreEqual(1, lesson.OrderIndex, "Default OrderIndex should be 1.");
            Assert.AreEqual(5, lesson.DurationMinutes, "Default DurationMinutes should be 5.");
            Assert.IsFalse(lesson.IsFreePreview, "Default IsFreePreview should be false.");
        }

        [TestMethod]
        public void Enrollment_DefaultInitialization_SetsActiveStatusAndZeroProgress()
        {
            var enrollment = new Enrollment();

            Assert.AreEqual(EnrollmentStatus.Active, enrollment.Status, "Default status should be Active.");
            Assert.AreEqual(0, enrollment.CompletionPercentage, "Default progress should be 0%.");
            Assert.IsTrue((DateTime.UtcNow - enrollment.EnrolledDate).TotalSeconds < 5, "EnrolledDate should be UtcNow.");
        }

        [TestMethod]
        public void LessonCompletion_DefaultInitialization_SetsUtcDate()
        {
            var completion = new LessonCompletion();

            Assert.IsTrue((DateTime.UtcNow - completion.CompletedDate).TotalSeconds < 5, "CompletedDate should be UtcNow.");
        }

        [TestMethod]
        public void ApplicationUser_DefaultInitialization_SetsCollectionsAndDefaults()
        {
            var user = new ApplicationUser();

            Assert.IsNotNull(user.Enrollments, "Enrollments collection should be initialized.");
            Assert.IsNotNull(user.LessonCompletions, "LessonCompletions collection should be initialized.");
            Assert.IsTrue((DateTime.UtcNow - user.CreatedDate).TotalSeconds < 5, "CreatedDate should be UtcNow.");
        }

        [TestMethod]
        public void CourseLevel_EnumValues_MatchExpectedValues()
        {
            Assert.AreEqual(0, (int)CourseLevel.Beginner);
            Assert.AreEqual(1, (int)CourseLevel.Intermediate);
            Assert.AreEqual(2, (int)CourseLevel.Advanced);
            Assert.AreEqual(3, (int)CourseLevel.AllLevels);
        }

        [TestMethod]
        public void EnrollmentStatus_EnumValues_MatchExpectedValues()
        {
            Assert.AreEqual(0, (int)EnrollmentStatus.Active);
            Assert.AreEqual(1, (int)EnrollmentStatus.Completed);
            Assert.AreEqual(2, (int)EnrollmentStatus.Dropped);
        }

        [TestMethod]
        public void ApplicationDbContext_HasExpectedDbSets()
        {
            using (var context = new ApplicationDbContext())
            {
                Assert.IsNotNull(context.Categories, "Categories DbSet must exist.");
                Assert.IsNotNull(context.Courses, "Courses DbSet must exist.");
                Assert.IsNotNull(context.Modules, "Modules DbSet must exist.");
                Assert.IsNotNull(context.Lessons, "Lessons DbSet must exist.");
                Assert.IsNotNull(context.Enrollments, "Enrollments DbSet must exist.");
                Assert.IsNotNull(context.LessonCompletions, "LessonCompletions DbSet must exist.");
                Assert.IsNotNull(context.Users, "Users DbSet must exist.");
            }
        }
    }
}
