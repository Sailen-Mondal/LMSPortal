using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using LMSPortal.Models;
using LMSPortal.ViewModels;
using Microsoft.AspNet.Identity;

namespace LMSPortal.Controllers
{
    public class LearningController : Controller
    {
        private readonly ApplicationDbContext _db;

        public LearningController()
        {
            _db = new ApplicationDbContext();
        }

        public LearningController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET: /learning/{courseId}/{lessonId?}
        [Route("learning/{courseId:int}/{lessonId:int?}")]
        public ActionResult Player(int courseId, int? lessonId)
        {
            var course = _db.Courses
                .Include(c => c.Modules.Select(m => m.Lessons))
                .FirstOrDefault(c => c.Id == courseId);

            if (course == null)
            {
                return HttpNotFound("The requested course could not be found.");
            }

            var userId = User.Identity.IsAuthenticated ? User.Identity.GetUserId() : null;
            bool isAdmin = User.IsInRole("Admin");
            bool isEnrolled = userId != null && _db.Enrollments.Any(e => e.CourseId == courseId && e.UserId == userId);

            // Flatten ordered curriculum
            var orderedModules = course.Modules.OrderBy(m => m.OrderIndex).ToList();
            var allLessons = orderedModules
                .SelectMany(m => (m.Lessons ?? Enumerable.Empty<Lesson>()).OrderBy(l => l.OrderIndex))
                .ToList();

            if (!allLessons.Any())
            {
                ViewBag.ErrorMessage = "This course does not have any lessons published yet.";
                return View("NoLessons", course);
            }

            // Determine target lesson
            Lesson currentLesson = null;
            if (lessonId.HasValue)
            {
                currentLesson = allLessons.FirstOrDefault(l => l.Id == lessonId.Value);
                if (currentLesson == null)
                {
                    // Check if lesson exists in another course (tampering check)
                    var foreignLesson = _db.Lessons.Include(l => l.Module).FirstOrDefault(l => l.Id == lessonId.Value);
                    if (foreignLesson != null && foreignLesson.Module != null && foreignLesson.Module.CourseId != courseId)
                    {
                        return HttpNotFound("The requested lesson does not belong to this course.");
                    }
                    return HttpNotFound("The requested lesson could not be found in this course.");
                }
            }
            else
            {
                // Find first uncompleted lesson if enrolled, else first lesson
                if (isEnrolled)
                {
                    var userCompletions = _db.LessonCompletions
                        .Where(lc => lc.UserId == userId)
                        .Select(lc => lc.LessonId)
                        .ToHashSet();

                    currentLesson = allLessons.FirstOrDefault(l => !userCompletions.Contains(l.Id)) ?? allLessons.First();
                }
                else
                {
                    currentLesson = allLessons.FirstOrDefault(l => l.IsFreePreview) ?? allLessons.First();
                }
            }

            // Authorization verification
            if (!isEnrolled && !isAdmin)
            {
                if (!currentLesson.IsFreePreview)
                {
                    if (!User.Identity.IsAuthenticated)
                    {
                        return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("Player", "Learning", new { courseId = courseId, lessonId = currentLesson.Id }) });
                    }

                    TempData["ErrorMessage"] = "You must enroll in this course to access protected lessons.";
                    return RedirectToAction("Details", "Course", new { slug = course.Slug });
                }
            }

            // Update enrollment last accessed date if enrolled
            if (isEnrolled)
            {
                var enrollment = _db.Enrollments.FirstOrDefault(e => e.CourseId == courseId && e.UserId == userId);
                if (enrollment != null)
                {
                    enrollment.LastAccessedDate = DateTime.UtcNow;
                    _db.SaveChanges();
                }
            }

            // Fetch completions for this user
            var completedLessonIds = new HashSet<int>();
            if (userId != null)
            {
                completedLessonIds = _db.LessonCompletions
                    .Where(lc => lc.UserId == userId)
                    .Select(lc => lc.LessonId)
                    .ToHashSet();
            }

            // Compute Previous and Next lesson IDs
            int currentIndex = allLessons.FindIndex(l => l.Id == currentLesson.Id);
            int? previousLessonId = currentIndex > 0 ? (int?)allLessons[currentIndex - 1].Id : null;
            int? nextLessonId = currentIndex < allLessons.Count - 1 ? (int?)allLessons[currentIndex + 1].Id : null;

            int totalLessons = allLessons.Count;
            int completedCount = allLessons.Count(l => completedLessonIds.Contains(l.Id));
            int progressPercentage = totalLessons == 0 ? 0 : (int)Math.Round((double)completedCount * 100.0 / totalLessons);

            var navModules = orderedModules.Select(m => new LearningModuleNavViewModel
            {
                ModuleId = m.Id,
                Title = m.Title,
                OrderIndex = m.OrderIndex,
                Lessons = (m.Lessons ?? Enumerable.Empty<Lesson>())
                    .OrderBy(l => l.OrderIndex)
                    .Select(l => new LearningLessonNavViewModel
                    {
                        LessonId = l.Id,
                        Title = l.Title,
                        OrderIndex = l.OrderIndex,
                        DurationMinutes = l.DurationMinutes,
                        IsFreePreview = l.IsFreePreview,
                        IsCurrent = l.Id == currentLesson.Id,
                        IsCompleted = completedLessonIds.Contains(l.Id)
                    }).ToList()
            }).ToList();

            var model = new LearningPlayerViewModel
            {
                CourseId = course.Id,
                CourseTitle = course.Title,
                CourseSlug = course.Slug,
                CurrentLessonId = currentLesson.Id,
                CurrentLessonTitle = currentLesson.Title,
                CurrentLessonContent = currentLesson.Content,
                CurrentLessonVideoUrl = FormatEmbedVideoUrl(currentLesson.VideoUrl),
                CurrentLessonDuration = currentLesson.DurationMinutes,
                IsCurrentLessonCompleted = completedLessonIds.Contains(currentLesson.Id),
                IsFreePreview = currentLesson.IsFreePreview,
                IsEnrolled = isEnrolled || isAdmin,
                CurrentModuleId = currentLesson.ModuleId,
                CurrentModuleTitle = currentLesson.Module != null ? currentLesson.Module.Title : string.Empty,
                PreviousLessonId = previousLessonId,
                NextLessonId = nextLessonId,
                CourseProgressPercentage = progressPercentage,
                CompletedLessonsCount = completedCount,
                TotalLessonsCount = totalLessons,
                Modules = navModules
            };

            return View(model);
        }

        // POST: /learning/toggle-complete
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        [Route("learning/toggle-complete")]
        public ActionResult ToggleComplete(int courseId, int lessonId)
        {
            var userId = User.Identity.GetUserId();
            bool isAdmin = User.IsInRole("Admin");
            var enrollment = _db.Enrollments.FirstOrDefault(e => e.CourseId == courseId && e.UserId == userId);

            if (enrollment == null && !isAdmin)
            {
                return Json(new { success = false, message = "You must be enrolled in this course to track progress." }, JsonRequestBehavior.DenyGet);
            }

            var lesson = _db.Lessons.Include(l => l.Module).FirstOrDefault(l => l.Id == lessonId);
            if (lesson == null || lesson.Module == null || lesson.Module.CourseId != courseId)
            {
                return Json(new { success = false, message = "Invalid lesson or lesson does not belong to this course." }, JsonRequestBehavior.DenyGet);
            }

            var existingCompletion = _db.LessonCompletions.FirstOrDefault(lc => lc.UserId == userId && lc.LessonId == lessonId);
            bool isCompletedNow;

            if (existingCompletion != null)
            {
                // Unmark completed
                _db.LessonCompletions.Remove(existingCompletion);
                isCompletedNow = false;
            }
            else
            {
                // Mark completed
                _db.LessonCompletions.Add(new LessonCompletion
                {
                    UserId = userId,
                    LessonId = lessonId,
                    CompletedDate = DateTime.UtcNow
                });
                isCompletedNow = true;
            }

            _db.SaveChanges();

            // Recalculate course progress
            var courseLessons = _db.Lessons
                .Where(l => l.Module.CourseId == courseId)
                .OrderBy(l => l.Module.OrderIndex)
                .ThenBy(l => l.OrderIndex)
                .Select(l => l.Id)
                .ToList();

            var userCompletions = _db.LessonCompletions
                .Where(lc => lc.UserId == userId && courseLessons.Contains(lc.LessonId))
                .Select(lc => lc.LessonId)
                .ToHashSet();

            int totalLessons = courseLessons.Count;
            int completedCount = userCompletions.Count;
            int percentage = totalLessons == 0 ? 0 : (int)Math.Round((double)completedCount * 100.0 / totalLessons);

            // Synchronize enrollment cached record if enrollment exists
            if (enrollment != null)
            {
                enrollment.CompletionPercentage = percentage;
                enrollment.LastAccessedDate = DateTime.UtcNow;
                if (percentage == 100 && totalLessons > 0)
                {
                    enrollment.Status = EnrollmentStatus.Completed;
                }
                else if (enrollment.Status == EnrollmentStatus.Completed && percentage < 100)
                {
                    enrollment.Status = EnrollmentStatus.Active;
                }
                _db.SaveChanges();
            }

            // Find next uncompleted lesson
            int? nextUncompletedLessonId = null;
            if (isCompletedNow)
            {
                int currentIdx = courseLessons.IndexOf(lessonId);
                for (int i = currentIdx + 1; i < courseLessons.Count; i++)
                {
                    if (!userCompletions.Contains(courseLessons[i]))
                    {
                        nextUncompletedLessonId = courseLessons[i];
                        break;
                    }
                }
            }

            return Json(new
            {
                success = true,
                isCompleted = isCompletedNow,
                progressPercentage = percentage,
                completedCount = completedCount,
                totalLessons = totalLessons,
                nextLessonId = nextUncompletedLessonId
            }, JsonRequestBehavior.DenyGet);
        }

        public static string FormatEmbedVideoUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url)) return null;

            // YouTube short URL: https://youtu.be/VIDEO_ID
            var matchShort = Regex.Match(url, @"youtu\.be/([a-zA-Z0-9_\-]+)");
            if (matchShort.Success)
            {
                return $"https://www.youtube.com/embed/{matchShort.Groups[1].Value}?rel=0&modestbranding=1";
            }

            // YouTube watch URL: https://www.youtube.com/watch?v=VIDEO_ID
            var matchWatch = Regex.Match(url, @"(?:youtube\.com/(?:watch\?.*v=|embed/))([a-zA-Z0-9_\-]+)");
            if (matchWatch.Success)
            {
                return $"https://www.youtube.com/embed/{matchWatch.Groups[1].Value}?rel=0&modestbranding=1";
            }

            return url;
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
