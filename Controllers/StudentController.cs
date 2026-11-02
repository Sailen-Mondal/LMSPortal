using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using LMSPortal.Models;
using LMSPortal.ViewModels;
using Microsoft.AspNet.Identity;

namespace LMSPortal.Controllers
{
    [Authorize]
    public class StudentController : Controller
    {
        private readonly ApplicationDbContext _db;

        public StudentController()
        {
            _db = new ApplicationDbContext();
        }

        public StudentController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET: /student/dashboard
        [Route("student/dashboard")]
        public ActionResult Dashboard()
        {
            var userId = User.Identity.GetUserId();
            var user = _db.Users.Find(userId);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var enrollments = _db.Enrollments
                .Include(e => e.Course.Category)
                .Include(e => e.Course.Modules.Select(m => m.Lessons))
                .Where(e => e.UserId == userId)
                .OrderByDescending(e => e.LastAccessedDate ?? e.EnrolledDate)
                .ToList();

            var completions = _db.LessonCompletions
                .Include(c => c.Lesson)
                .Where(c => c.UserId == userId)
                .ToList();

            var completedLessonIds = new HashSet<int>(completions.Select(c => c.LessonId));

            var courseCards = new List<StudentCourseCardViewModel>();
            bool hasSyncChanges = false;

            foreach (var enrollment in enrollments)
            {
                var card = MapToCardViewModel(enrollment, completedLessonIds, out bool syncNeeded);
                if (syncNeeded)
                {
                    hasSyncChanges = true;
                }
                courseCards.Add(card);
            }

            if (hasSyncChanges)
            {
                _db.SaveChanges();
            }

            int totalEnrolled = courseCards.Count;
            int completedCourses = courseCards.Count(c => c.Status == EnrollmentStatus.Completed || c.CompletionPercentage == 100);
            int inProgressCourses = courseCards.Count(c => c.Status == EnrollmentStatus.Active && c.CompletionPercentage < 100);
            int totalLessonsCompleted = completions.Count;
            double totalHoursLearned = Math.Round(completions.Sum(c => c.Lesson != null ? c.Lesson.DurationMinutes : 0) / 60.0, 1);
            int overallProgress = totalEnrolled > 0 ? (int)Math.Round(courseCards.Average(c => c.CompletionPercentage)) : 0;

            var activeCourse = courseCards.FirstOrDefault(c => c.CompletionPercentage < 100) ?? courseCards.FirstOrDefault();

            var model = new StudentDashboardViewModel
            {
                StudentName = user.FullName ?? user.UserName,
                AvatarUrl = user.AvatarUrl,
                MemberSince = user.CreatedDate,
                TotalEnrolled = totalEnrolled,
                CompletedCourses = completedCourses,
                InProgressCourses = inProgressCourses,
                TotalLessonsCompleted = totalLessonsCompleted,
                TotalHoursLearned = totalHoursLearned,
                OverallProgressPercentage = overallProgress,
                ActiveCourse = activeCourse,
                EnrolledCourses = courseCards
            };

            return View(model);
        }

        // GET: /student/my-courses
        [Route("student/my-courses")]
        public ActionResult MyCourses(string filter = "all")
        {
            var userId = User.Identity.GetUserId();
            var enrollments = _db.Enrollments
                .Include(e => e.Course.Category)
                .Include(e => e.Course.Modules.Select(m => m.Lessons))
                .Where(e => e.UserId == userId)
                .OrderByDescending(e => e.LastAccessedDate ?? e.EnrolledDate)
                .ToList();

            var completions = _db.LessonCompletions
                .Where(c => c.UserId == userId)
                .Select(c => c.LessonId)
                .ToList();

            var completedLessonIds = new HashSet<int>(completions);
            var courseCards = new List<StudentCourseCardViewModel>();
            bool hasSyncChanges = false;

            foreach (var enrollment in enrollments)
            {
                var card = MapToCardViewModel(enrollment, completedLessonIds, out bool syncNeeded);
                if (syncNeeded)
                {
                    hasSyncChanges = true;
                }
                courseCards.Add(card);
            }

            if (hasSyncChanges)
            {
                _db.SaveChanges();
            }

            int inProgressCount = courseCards.Count(c => c.CompletionPercentage < 100);
            int completedCount = courseCards.Count(c => c.CompletionPercentage == 100);

            var filteredCourses = courseCards;
            if (string.Equals(filter, "in-progress", StringComparison.OrdinalIgnoreCase))
            {
                filteredCourses = courseCards.Where(c => c.CompletionPercentage < 100).ToList();
            }
            else if (string.Equals(filter, "completed", StringComparison.OrdinalIgnoreCase))
            {
                filteredCourses = courseCards.Where(c => c.CompletionPercentage == 100).ToList();
            }
            else
            {
                filter = "all";
            }

            var model = new StudentMyCoursesViewModel
            {
                Filter = filter.ToLowerInvariant(),
                Courses = filteredCourses,
                TotalCount = courseCards.Count,
                InProgressCount = inProgressCount,
                CompletedCount = completedCount
            };

            return View(model);
        }

        private StudentCourseCardViewModel MapToCardViewModel(Enrollment enrollment, HashSet<int> completedLessonIds, out bool syncNeeded)
        {
            syncNeeded = false;
            var course = enrollment.Course;
            var allLessons = course.Modules != null
                ? course.Modules
                    .OrderBy(m => m.OrderIndex)
                    .SelectMany(m => (m.Lessons ?? Enumerable.Empty<Lesson>()).OrderBy(l => l.OrderIndex))
                    .ToList()
                : new List<Lesson>();

            int totalLessons = allLessons.Count;
            int completedLessons = allLessons.Count(l => completedLessonIds.Contains(l.Id));

            int actualPercentage = totalLessons == 0 ? 0 : (int)Math.Round((double)completedLessons * 100.0 / totalLessons);

            // Synchronize enrollment cached percentage and status if out of date
            if (enrollment.CompletionPercentage != actualPercentage)
            {
                enrollment.CompletionPercentage = actualPercentage;
                syncNeeded = true;
            }

            var expectedStatus = (actualPercentage == 100 && totalLessons > 0) ? EnrollmentStatus.Completed : EnrollmentStatus.Active;
            if (enrollment.Status != expectedStatus && enrollment.Status != EnrollmentStatus.Dropped)
            {
                enrollment.Status = expectedStatus;
                syncNeeded = true;
            }

            // Find next uncompleted lesson
            Lesson nextLesson = allLessons.FirstOrDefault(l => !completedLessonIds.Contains(l.Id));

            return new StudentCourseCardViewModel
            {
                EnrollmentId = enrollment.Id,
                CourseId = course.Id,
                Title = course.Title,
                Slug = course.Slug,
                CategoryName = course.Category != null ? course.Category.Name : "General",
                CategoryColor = "#4361ee",
                ThumbnailUrl = course.ThumbnailUrl,
                InstructorName = course.InstructorName,
                Level = course.Level,
                EnrolledDate = enrollment.EnrolledDate,
                LastAccessedDate = enrollment.LastAccessedDate,
                CompletionPercentage = actualPercentage,
                Status = enrollment.Status,
                CompletedLessonsCount = completedLessons,
                TotalLessonsCount = totalLessons,
                NextLessonId = nextLesson != null ? (int?)nextLesson.Id : (allLessons.FirstOrDefault() != null ? (int?)allLessons.First().Id : null),
                NextLessonTitle = nextLesson != null ? nextLesson.Title : (allLessons.Any() ? "Review Course" : "No lessons"),
                NextModuleTitle = nextLesson != null && nextLesson.Module != null ? nextLesson.Module.Title : null
            };
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && _db != null)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
