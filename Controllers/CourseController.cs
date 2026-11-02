using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using LMSPortal.Models;
using LMSPortal.ViewModels;
using Microsoft.AspNet.Identity;

namespace LMSPortal.Controllers
{
    public class CourseController : Controller
    {
        private readonly ApplicationDbContext _db;

        public CourseController()
        {
            _db = new ApplicationDbContext();
        }

        public CourseController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET: /courses or /Course/Index
        public ActionResult Index(string search, string category, CourseLevel? level, string sort)
        {
            var query = _db.Courses
                .Include(c => c.Category)
                .Include(c => c.Modules.Select(m => m.Lessons))
                .Where(c => c.IsPublished && !c.IsArchived);

            // Filter by search keyword
            if (!string.IsNullOrWhiteSpace(search))
            {
                string searchLower = search.Trim().ToLower();
                query = query.Where(c => c.Title.ToLower().Contains(searchLower) ||
                                         c.ShortDescription.ToLower().Contains(searchLower) ||
                                         c.InstructorName.ToLower().Contains(searchLower));
            }

            // Filter by category slug
            if (!string.IsNullOrWhiteSpace(category))
            {
                query = query.Where(c => c.Category.Slug == category);
            }

            // Filter by skill level
            if (level.HasValue)
            {
                query = query.Where(c => c.Level == level.Value);
            }

            // Sorting
            switch (sort?.ToLower())
            {
                case "title":
                    query = query.OrderBy(c => c.Title);
                    break;
                case "duration":
                    query = query.OrderByDescending(c => c.DurationMinutes);
                    break;
                case "newest":
                default:
                    query = query.OrderByDescending(c => c.CreatedDate);
                    break;
            }

            var courseCards = query.Select(c => new CourseCardViewModel
            {
                Id = c.Id,
                Title = c.Title,
                Slug = c.Slug,
                ShortDescription = c.ShortDescription,
                ThumbnailUrl = c.ThumbnailUrl,
                InstructorName = c.InstructorName,
                InstructorTitle = c.InstructorTitle,
                InstructorAvatarUrl = c.InstructorAvatarUrl,
                CategoryName = c.Category != null ? c.Category.Name : "General",
                CategorySlug = c.Category != null ? c.Category.Slug : "general",
                Level = c.Level,
                DurationMinutes = c.DurationMinutes,
                LessonCount = c.Modules.SelectMany(m => m.Lessons).Count(),
                IsFeatured = c.IsFeatured
            }).ToList();

            var viewModel = new CourseCatalogViewModel
            {
                SearchQuery = search,
                SelectedCategorySlug = category,
                SelectedLevel = level,
                SortBy = sort ?? "newest",
                Categories = _db.Categories.Where(cat => cat.IsActive).OrderBy(cat => cat.DisplayOrder).ToList(),
                Courses = courseCards,
                TotalCount = courseCards.Count
            };

            return View(viewModel);
        }

        // GET: /course/{slug} or /Course/Details/{slug}
        public ActionResult Details(string slug)
        {
            if (string.IsNullOrWhiteSpace(slug))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Course identifier is required.");
            }

            var course = _db.Courses
                .Include(c => c.Category)
                .Include(c => c.Modules.Select(m => m.Lessons))
                .FirstOrDefault(c => c.Slug == slug);

            if (course == null)
            {
                return HttpNotFound("The requested course could not be found.");
            }

            // Check publishing status (unless admin viewing)
            if ((!course.IsPublished || course.IsArchived) && !User.IsInRole("Admin"))
            {
                return HttpNotFound("This course is currently unavailable.");
            }

            var model = new CourseDetailsViewModel
            {
                Course = course,
                TotalLessons = course.Modules.SelectMany(m => m.Lessons).Count(),
                TotalDurationMinutes = course.Modules.SelectMany(m => m.Lessons).Sum(l => l.DurationMinutes),
                FirstPreviewLesson = course.Modules
                    .OrderBy(m => m.OrderIndex)
                    .SelectMany(m => m.Lessons)
                    .OrderBy(l => l.OrderIndex)
                    .FirstOrDefault(l => l.IsFreePreview)
            };

            // Check enrollment status for authenticated students
            if (User.Identity.IsAuthenticated)
            {
                string userId = User.Identity.GetUserId();
                var enrollment = _db.Enrollments.FirstOrDefault(e => e.CourseId == course.Id && e.UserId == userId);

                if (enrollment != null)
                {
                    model.IsEnrolled = true;
                    model.StudentProgress = enrollment.CompletionPercentage;

                    // Resolve resume lesson: first incomplete lesson, or first lesson if all done
                    var allLessons = course.Modules
                        .OrderBy(m => m.OrderIndex)
                        .SelectMany(m => m.Lessons)
                        .OrderBy(l => l.OrderIndex)
                        .ToList();

                    var completedLessonIds = _db.LessonCompletions
                        .Where(lc => lc.UserId == userId && lc.Lesson.Module.CourseId == course.Id)
                        .Select(lc => lc.LessonId)
                        .ToList();

                    var nextLesson = allLessons.FirstOrDefault(l => !completedLessonIds.Contains(l.Id));
                    model.ResumeLessonId = nextLesson != null ? nextLesson.Id : (allLessons.FirstOrDefault()?.Id);
                }
            }

            return View(model);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
