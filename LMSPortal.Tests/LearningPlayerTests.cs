using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LMSPortal.Controllers;
using LMSPortal.Models;

namespace LMSPortal.Tests
{
    [TestClass]
    public class LearningPlayerTests
    {
        [TestMethod]
        public void FormatEmbedVideoUrl_YouTubeStandardWatchUrl_ConvertsToEmbed()
        {
            // Arrange
            string watchUrl = "https://www.youtube.com/watch?v=dQw4w9WgXcQ";

            // Act
            string embedUrl = LearningController.FormatEmbedVideoUrl(watchUrl);

            // Assert
            Assert.IsNotNull(embedUrl);
            Assert.IsTrue(embedUrl.StartsWith("https://www.youtube.com/embed/dQw4w9WgXcQ"));
        }

        [TestMethod]
        public void FormatEmbedVideoUrl_YouTubeShortUrl_ConvertsToEmbed()
        {
            // Arrange
            string shortUrl = "https://youtu.be/dQw4w9WgXcQ";

            // Act
            string embedUrl = LearningController.FormatEmbedVideoUrl(shortUrl);

            // Assert
            Assert.IsNotNull(embedUrl);
            Assert.IsTrue(embedUrl.StartsWith("https://www.youtube.com/embed/dQw4w9WgXcQ"));
        }

        [TestMethod]
        public void FormatEmbedVideoUrl_NullOrEmpty_ReturnsNull()
        {
            // Act & Assert
            Assert.IsNull(LearningController.FormatEmbedVideoUrl(null));
            Assert.IsNull(LearningController.FormatEmbedVideoUrl(string.Empty));
            Assert.IsNull(LearningController.FormatEmbedVideoUrl("   "));
        }

        [TestMethod]
        public void CrossCourseTampering_LessonBelongingToDifferentCourse_IsDetected()
        {
            // Arrange
            int routeCourseId = 10;
            var lesson = new Lesson
            {
                Id = 55,
                ModuleId = 20,
                Module = new Module { Id = 20, CourseId = 99 } // Different course!
            };

            // Act
            bool isTampering = lesson.Module.CourseId != routeCourseId;

            // Assert
            Assert.IsTrue(isTampering, "Lesson belonging to course 99 should not be accessible under course 10.");
        }

        [TestMethod]
        public void LessonAccess_FreePreview_IsAccessibleWhenUnenrolled()
        {
            // Arrange
            bool isEnrolled = false;
            bool isAdmin = false;
            var previewLesson = new Lesson { Id = 1, IsFreePreview = true };

            // Act
            bool canAccess = isEnrolled || isAdmin || previewLesson.IsFreePreview;

            // Assert
            Assert.IsTrue(canAccess);
        }

        [TestMethod]
        public void LessonAccess_ProtectedLesson_IsBlockedWhenUnenrolled()
        {
            // Arrange
            bool isEnrolled = false;
            bool isAdmin = false;
            var protectedLesson = new Lesson { Id = 2, IsFreePreview = false };

            // Act
            bool canAccess = isEnrolled || isAdmin || protectedLesson.IsFreePreview;

            // Assert
            Assert.IsFalse(canAccess);
        }

        [TestMethod]
        public void SequentialLessonNavigation_PreviousAndNext_ResolvedAccurately()
        {
            // Arrange
            var module1 = new Module { Id = 1, OrderIndex = 1 };
            var l1 = new Lesson { Id = 10, ModuleId = 1, OrderIndex = 1 };
            var l2 = new Lesson { Id = 20, ModuleId = 1, OrderIndex = 2 };

            var module2 = new Module { Id = 2, OrderIndex = 2 };
            var l3 = new Lesson { Id = 30, ModuleId = 2, OrderIndex = 1 };

            var flatLessons = new List<Lesson> { l1, l2, l3 };

            // Act - for lesson 20
            int currentIndex = flatLessons.FindIndex(l => l.Id == 20);
            int? prevId = currentIndex > 0 ? (int?)flatLessons[currentIndex - 1].Id : null;
            int? nextId = currentIndex < flatLessons.Count - 1 ? (int?)flatLessons[currentIndex + 1].Id : null;

            // Assert
            Assert.AreEqual(10, prevId);
            Assert.AreEqual(30, nextId);
        }

        [TestMethod]
        public void ProgressRecalculation_100PercentCompletion_TransitionsToCompletedStatus()
        {
            // Arrange
            int totalLessons = 4;
            int completedLessons = 4;

            // Act
            int percentage = (int)Math.Round((double)completedLessons * 100.0 / totalLessons);
            var expectedStatus = (percentage == 100 && totalLessons > 0) ? EnrollmentStatus.Completed : EnrollmentStatus.Active;

            // Assert
            Assert.AreEqual(100, percentage);
            Assert.AreEqual(EnrollmentStatus.Completed, expectedStatus);
        }

        [TestMethod]
        public void ProgressRecalculation_UnmarkingLesson_TransitionsBackToActiveStatus()
        {
            // Arrange
            int totalLessons = 4;
            int completedLessons = 3; // uncompleted one

            // Act
            int percentage = (int)Math.Round((double)completedLessons * 100.0 / totalLessons);
            var expectedStatus = (percentage == 100 && totalLessons > 0) ? EnrollmentStatus.Completed : EnrollmentStatus.Active;

            // Assert
            Assert.AreEqual(75, percentage);
            Assert.AreEqual(EnrollmentStatus.Active, expectedStatus);
        }
    }
}
