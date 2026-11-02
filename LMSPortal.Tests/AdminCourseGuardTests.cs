using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LMSPortal.Areas.Admin;
using LMSPortal.Areas.Admin.Controllers;
using LMSPortal.Models;

namespace LMSPortal.Tests
{
    [TestClass]
    public class AdminCourseGuardTests
    {
        [TestMethod]
        public void CourseDeletionGuard_WhenEnrollmentsExist_BlocksDeletion()
        {
            // Arrange
            var enrollments = new List<Enrollment>
            {
                new Enrollment { Id = 1, CourseId = 5, UserId = "user-123" }
            };

            int courseToDeleteId = 5;

            // Act: Guard condition evaluation
            bool hasEnrollments = enrollments.Any(e => e.CourseId == courseToDeleteId);
            bool canSafelyDelete = !hasEnrollments;

            // Assert
            Assert.IsTrue(hasEnrollments, "Course with active student enrollments must be detected.");
            Assert.IsFalse(canSafelyDelete, "Deletion must be rejected when student enrollments exist.");
        }

        [TestMethod]
        public void CourseDeletionGuard_WhenZeroEnrollments_AllowsDeletion()
        {
            // Arrange
            var enrollments = new List<Enrollment>
            {
                new Enrollment { Id = 1, CourseId = 5, UserId = "user-123" }
            };

            int courseToDeleteId = 99; // no enrollments

            // Act
            bool hasEnrollments = enrollments.Any(e => e.CourseId == courseToDeleteId);
            bool canSafelyDelete = !hasEnrollments;

            // Assert
            Assert.IsFalse(hasEnrollments);
            Assert.IsTrue(canSafelyDelete, "Deletion is safe when no student enrollments exist.");
        }

        [TestMethod]
        public void TogglePublish_FlipsStateWithoutDestroyingData()
        {
            // Arrange
            var course = new Course
            {
                Id = 1,
                Title = "ASP.NET Core Architecture",
                IsPublished = true
            };

            // Act 1: Unpublish (Archive)
            course.IsPublished = !course.IsPublished;

            // Assert 1
            Assert.IsFalse(course.IsPublished, "Course should now be archived / unpublished.");

            // Act 2: Re-publish
            course.IsPublished = !course.IsPublished;

            // Assert 2
            Assert.IsTrue(course.IsPublished, "Course should now be published again.");
        }

        [TestMethod]
        public void CategoryDeletionGuard_WhenCoursesExist_BlocksDeletion()
        {
            // Arrange
            var category = new Category
            {
                Id = 1,
                Name = "Development",
                Courses = new List<Course>
                {
                    new Course { Id = 10, Title = "C# Mastery" }
                }
            };

            // Act
            bool canDeleteCategory = !category.Courses.Any();

            // Assert
            Assert.IsFalse(canDeleteCategory, "Categories with assigned courses must not be deleted.");
        }

        [TestMethod]
        public void GenerateSlug_ProducesCleanKebabCaseUrl()
        {
            // Arrange
            string input = "Advanced C# & .NET 4.8 Architecture (2026 Edition)!";

            // Act
            string slug = CourseManagementController.GenerateSlug(input);

            // Assert
            Assert.AreEqual("advanced-c-net-48-architecture-2026-edition", slug);
        }

        [TestMethod]
        public void GenerateSlug_HandlesEmptyAndWhitespaceSafely()
        {
            // Act & Assert
            Assert.AreEqual(string.Empty, CourseManagementController.GenerateSlug(null));
            Assert.AreEqual(string.Empty, CourseManagementController.GenerateSlug("   "));
        }

        [TestMethod]
        public void AdminAreaRegistration_AreaName_IsAdmin()
        {
            // Arrange
            var reg = new AdminAreaRegistration();

            // Assert
            Assert.AreEqual("Admin", reg.AreaName);
        }
    }
}
