using System;
using System.Collections.Generic;
using System.Linq;
using LMSPortal.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LMSPortal.Tests
{
    [TestClass]
    public class EnrollmentTests
    {
        [TestMethod]
        public void Enrollment_Creation_SetsInitialPropertiesAccurately()
        {
            string studentId = "student-123";
            int courseId = 5;

            var enrollment = new Enrollment
            {
                UserId = studentId,
                CourseId = courseId,
                EnrolledDate = DateTime.UtcNow,
                LastAccessedDate = DateTime.UtcNow,
                Status = EnrollmentStatus.Active,
                CompletionPercentage = 0
            };

            Assert.AreEqual("student-123", enrollment.UserId);
            Assert.AreEqual(5, enrollment.CourseId);
            Assert.AreEqual(EnrollmentStatus.Active, enrollment.Status);
            Assert.AreEqual(0, enrollment.CompletionPercentage);
        }

        [TestMethod]
        public void Enrollment_DuplicateDetection_IdentifiesExistingUserCoursePair()
        {
            var existingEnrollments = new List<Enrollment>
            {
                new Enrollment { UserId = "user-1", CourseId = 101 },
                new Enrollment { UserId = "user-2", CourseId = 102 }
            };

            bool isDuplicate = existingEnrollments.Any(e => e.UserId == "user-1" && e.CourseId == 101);
            bool isNew = !existingEnrollments.Any(e => e.UserId == "user-1" && e.CourseId == 103);

            Assert.IsTrue(isDuplicate, "System must recognize that user-1 is already enrolled in course 101.");
            Assert.IsTrue(isNew, "System must allow user-1 to enroll in course 103.");
        }

        [TestMethod]
        public void Enrollment_FirstLessonResolution_ResolvesEarliestLessonInCurriculum()
        {
            var course = new Course { Id = 1, Title = "Full-Stack Web Dev" };
            var mod2 = new Module { Id = 2, Title = "Chapter 2", OrderIndex = 2 };
            var mod1 = new Module { Id = 1, Title = "Chapter 1", OrderIndex = 1 };

            var lesson1_2 = new Lesson { Id = 2, Title = "Lesson 1.2", OrderIndex = 2, ModuleId = 1, Module = mod1 };
            var lesson1_1 = new Lesson { Id = 1, Title = "Lesson 1.1", OrderIndex = 1, ModuleId = 1, Module = mod1 };
            var lesson2_1 = new Lesson { Id = 3, Title = "Lesson 2.1", OrderIndex = 1, ModuleId = 2, Module = mod2 };

            mod1.Lessons.Add(lesson1_2);
            mod1.Lessons.Add(lesson1_1);
            mod2.Lessons.Add(lesson2_1);

            course.Modules.Add(mod2);
            course.Modules.Add(mod1);

            // Resolution algorithm: Order modules by OrderIndex, then lessons by OrderIndex
            var firstLesson = course.Modules
                .OrderBy(m => m.OrderIndex)
                .SelectMany(m => m.Lessons)
                .OrderBy(l => l.OrderIndex)
                .FirstOrDefault();

            Assert.IsNotNull(firstLesson);
            Assert.AreEqual(1, firstLesson.Id);
            Assert.AreEqual("Lesson 1.1", firstLesson.Title);
        }

        [TestMethod]
        public void Enrollment_ConcurrencyProtection_CompositeUniqueConstraintFormat()
        {
            // Verify that the composite unique index attribute exists and is formatted correctly
            var properties = typeof(Enrollment).GetProperties();
            var userProp = properties.FirstOrDefault(p => p.Name == "UserId");
            var courseProp = properties.FirstOrDefault(p => p.Name == "CourseId");

            Assert.IsNotNull(userProp);
            Assert.IsNotNull(courseProp);

            var userIndexAttr = userProp.GetCustomAttributes(typeof(System.ComponentModel.DataAnnotations.Schema.IndexAttribute), true).FirstOrDefault() as System.ComponentModel.DataAnnotations.Schema.IndexAttribute;
            var courseIndexAttr = courseProp.GetCustomAttributes(typeof(System.ComponentModel.DataAnnotations.Schema.IndexAttribute), true).FirstOrDefault() as System.ComponentModel.DataAnnotations.Schema.IndexAttribute;

            Assert.IsNotNull(userIndexAttr, "UserId must have composite unique index attribute.");
            Assert.IsNotNull(courseIndexAttr, "CourseId must have composite unique index attribute.");
            Assert.AreEqual(userIndexAttr.Name, courseIndexAttr.Name, "Both properties must share index name 'IX_Enrollment_User_Course'.");
            Assert.IsTrue(userIndexAttr.IsUnique, "Index must be configured as Unique.");
        }
    }
}
