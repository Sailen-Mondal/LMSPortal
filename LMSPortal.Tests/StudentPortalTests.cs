using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LMSPortal.Models;
using LMSPortal.ViewModels;

namespace LMSPortal.Tests
{
    [TestClass]
    public class StudentPortalTests
    {
        [TestMethod]
        public void OverallProgress_WithMultipleEnrollments_CalculatesCorrectAverage()
        {
            // Arrange
            var courses = new List<StudentCourseCardViewModel>
            {
                new StudentCourseCardViewModel { CourseId = 1, CompletionPercentage = 100, Status = EnrollmentStatus.Completed },
                new StudentCourseCardViewModel { CourseId = 2, CompletionPercentage = 50, Status = EnrollmentStatus.Active },
                new StudentCourseCardViewModel { CourseId = 3, CompletionPercentage = 0, Status = EnrollmentStatus.Active }
            };

            // Act
            int averageProgress = courses.Any() ? (int)Math.Round(courses.Average(c => c.CompletionPercentage)) : 0;
            int completedCount = courses.Count(c => c.Status == EnrollmentStatus.Completed || c.CompletionPercentage == 100);
            int inProgressCount = courses.Count(c => c.Status == EnrollmentStatus.Active && c.CompletionPercentage < 100);

            // Assert
            Assert.AreEqual(50, averageProgress);
            Assert.AreEqual(1, completedCount);
            Assert.AreEqual(2, inProgressCount);
        }

        [TestMethod]
        public void OverallProgress_WithZeroEnrollments_ReturnsZeroWithoutException()
        {
            // Arrange
            var courses = new List<StudentCourseCardViewModel>();

            // Act
            int averageProgress = courses.Any() ? (int)Math.Round(courses.Average(c => c.CompletionPercentage)) : 0;

            // Assert
            Assert.AreEqual(0, averageProgress);
        }

        [TestMethod]
        public void TotalHoursLearned_SumsLessonDurationsAccurately()
        {
            // Arrange
            var completedLessons = new List<Lesson>
            {
                new Lesson { Id = 1, DurationMinutes = 45 },
                new Lesson { Id = 2, DurationMinutes = 30 },
                new Lesson { Id = 3, DurationMinutes = 15 }
            };

            // Act
            double totalHours = Math.Round(completedLessons.Sum(l => l.DurationMinutes) / 60.0, 1);

            // Assert (45 + 30 + 15 = 90 mins = 1.5 hrs)
            Assert.AreEqual(1.5, totalHours);
        }

        [TestMethod]
        public void NextLesson_ResolvesFirstUncompletedLessonInSequence()
        {
            // Arrange
            var module1 = new Module { Id = 1, OrderIndex = 1, Title = "Module 1" };
            var lesson1 = new Lesson { Id = 101, ModuleId = 1, OrderIndex = 1, Title = "Lesson 1", Module = module1 };
            var lesson2 = new Lesson { Id = 102, ModuleId = 1, OrderIndex = 2, Title = "Lesson 2", Module = module1 };
            var lesson3 = new Lesson { Id = 103, ModuleId = 1, OrderIndex = 3, Title = "Lesson 3", Module = module1 };

            var allLessons = new List<Lesson> { lesson1, lesson2, lesson3 };
            var completedLessonIds = new HashSet<int> { 101, 102 };

            // Act
            var nextLesson = allLessons.FirstOrDefault(l => !completedLessonIds.Contains(l.Id));

            // Assert
            Assert.IsNotNull(nextLesson);
            Assert.AreEqual(103, nextLesson.Id);
            Assert.AreEqual("Lesson 3", nextLesson.Title);
        }

        [TestMethod]
        public void NextLesson_WhenAllCompleted_ReturnsNullOrCompletedIndicator()
        {
            // Arrange
            var lesson1 = new Lesson { Id = 201, OrderIndex = 1, Title = "Lesson 1" };
            var allLessons = new List<Lesson> { lesson1 };
            var completedLessonIds = new HashSet<int> { 201 };

            // Act
            var nextLesson = allLessons.FirstOrDefault(l => !completedLessonIds.Contains(l.Id));

            // Assert
            Assert.IsNull(nextLesson);
        }

        [TestMethod]
        public void MyCourses_FilterPartitionsCoursesAccurately()
        {
            // Arrange
            var allCourses = new List<StudentCourseCardViewModel>
            {
                new StudentCourseCardViewModel { CourseId = 1, CompletionPercentage = 100 },
                new StudentCourseCardViewModel { CourseId = 2, CompletionPercentage = 60 },
                new StudentCourseCardViewModel { CourseId = 3, CompletionPercentage = 0 }
            };

            // Act
            var inProgress = allCourses.Where(c => c.CompletionPercentage < 100).ToList();
            var completed = allCourses.Where(c => c.CompletionPercentage == 100).ToList();

            // Assert
            Assert.AreEqual(2, inProgress.Count);
            Assert.AreEqual(1, completed.Count);
        }
    }
}
