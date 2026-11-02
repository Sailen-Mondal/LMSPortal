using System;
using System.Collections.Generic;
using System.Linq;
using LMSPortal.Models;
using LMSPortal.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LMSPortal.Tests
{
    [TestClass]
    public class CourseCatalogTests
    {
        private List<Course> GetMockCourses()
        {
            var catWeb = new Category { Id = 1, Name = "Web", Slug = "web" };
            var catData = new Category { Id = 2, Name = "Data", Slug = "data" };

            return new List<Course>
            {
                new Course
                {
                    Id = 1,
                    Title = "C# Enterprise Patterns",
                    Slug = "csharp-enterprise-patterns",
                    ShortDescription = "Clean architecture and patterns",
                    InstructorName = "Sarah Jenkins",
                    Category = catWeb,
                    CategoryId = 1,
                    Level = CourseLevel.Advanced,
                    DurationMinutes = 300,
                    IsPublished = true,
                    IsArchived = false,
                    CreatedDate = DateTime.UtcNow.AddDays(-10)
                },
                new Course
                {
                    Id = 2,
                    Title = "Python Machine Learning",
                    Slug = "python-machine-learning",
                    ShortDescription = "Data science and neural nets",
                    InstructorName = "Marcus Vance",
                    Category = catData,
                    CategoryId = 2,
                    Level = CourseLevel.Beginner,
                    DurationMinutes = 180,
                    IsPublished = true,
                    IsArchived = false,
                    CreatedDate = DateTime.UtcNow.AddDays(-5)
                },
                new Course
                {
                    Id = 3,
                    Title = "Modern React",
                    Slug = "modern-react",
                    ShortDescription = "Frontend component architecture",
                    InstructorName = "Elena Rostova",
                    Category = catWeb,
                    CategoryId = 1,
                    Level = CourseLevel.Intermediate,
                    DurationMinutes = 240,
                    IsPublished = true,
                    IsArchived = false,
                    CreatedDate = DateTime.UtcNow.AddDays(-1)
                }
            };
        }

        [TestMethod]
        public void CourseCatalog_FilterBySearchKeyword_MatchesTitleOrDescription()
        {
            var courses = GetMockCourses().AsQueryable();
            string keyword = "react";

            var filtered = courses.Where(c => c.Title.ToLower().Contains(keyword) || 
                                              c.ShortDescription.ToLower().Contains(keyword)).ToList();

            Assert.AreEqual(1, filtered.Count);
            Assert.AreEqual("Modern React", filtered[0].Title);
        }

        [TestMethod]
        public void CourseCatalog_FilterByCategory_FiltersCorrectCategory()
        {
            var courses = GetMockCourses().AsQueryable();
            string categorySlug = "data";

            var filtered = courses.Where(c => c.Category.Slug == categorySlug).ToList();

            Assert.AreEqual(1, filtered.Count);
            Assert.AreEqual("Python Machine Learning", filtered[0].Title);
        }

        [TestMethod]
        public void CourseCatalog_FilterByLevel_FiltersCorrectSkillLevel()
        {
            var courses = GetMockCourses().AsQueryable();
            CourseLevel level = CourseLevel.Advanced;

            var filtered = courses.Where(c => c.Level == level).ToList();

            Assert.AreEqual(1, filtered.Count);
            Assert.AreEqual("C# Enterprise Patterns", filtered[0].Title);
        }

        [TestMethod]
        public void CourseCatalog_SortByTitle_OrdersAlphabetically()
        {
            var courses = GetMockCourses().AsQueryable();

            var sorted = courses.OrderBy(c => c.Title).ToList();

            Assert.AreEqual("C# Enterprise Patterns", sorted[0].Title);
            Assert.AreEqual("Modern React", sorted[1].Title);
            Assert.AreEqual("Python Machine Learning", sorted[2].Title);
        }

        [TestMethod]
        public void CourseCatalog_SortByDuration_OrdersDescending()
        {
            var courses = GetMockCourses().AsQueryable();

            var sorted = courses.OrderByDescending(c => c.DurationMinutes).ToList();

            Assert.AreEqual(300, sorted[0].DurationMinutes);
            Assert.AreEqual(240, sorted[1].DurationMinutes);
            Assert.AreEqual(180, sorted[2].DurationMinutes);
        }

        [TestMethod]
        public void CourseDetails_FreePreviewDetection_FindsFirstPreviewLesson()
        {
            var course = new Course { Title = "Architecture Deep Dive" };
            var module = new Module { Title = "Module 1", OrderIndex = 1 };
            
            var lesson1 = new Lesson { Id = 10, Title = "Intro", IsFreePreview = false, OrderIndex = 1 };
            var lesson2 = new Lesson { Id = 11, Title = "Preview Video", IsFreePreview = true, OrderIndex = 2 };
            var lesson3 = new Lesson { Id = 12, Title = "Advanced Topic", IsFreePreview = false, OrderIndex = 3 };

            module.Lessons.Add(lesson1);
            module.Lessons.Add(lesson2);
            module.Lessons.Add(lesson3);
            course.Modules.Add(module);

            var previewLesson = course.Modules
                .OrderBy(m => m.OrderIndex)
                .SelectMany(m => m.Lessons)
                .OrderBy(l => l.OrderIndex)
                .FirstOrDefault(l => l.IsFreePreview);

            Assert.IsNotNull(previewLesson);
            Assert.AreEqual(11, previewLesson.Id);
            Assert.AreEqual("Preview Video", previewLesson.Title);
        }
    }
}
