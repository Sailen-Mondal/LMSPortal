using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using LMSPortal.Models;
using LMSPortal.ViewModels;

namespace LMSPortal.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db;

        public HomeController()
        {
            _db = new ApplicationDbContext();
        }

        public HomeController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET: /
        public ActionResult Index()
        {
            var model = new LandingPageViewModel();

            // Query featured published courses with related data
            var featuredCourses = _db.Courses
                .Include(c => c.Category)
                .Include(c => c.Modules.Select(m => m.Lessons))
                .Where(c => c.IsPublished && !c.IsArchived && c.IsFeatured)
                .OrderByDescending(c => c.CreatedDate)
                .Take(6)
                .ToList();

            // If fewer than 4 featured courses exist, fetch latest published courses to fill the showcase
            if (featuredCourses.Count < 4)
            {
                var additionalCourses = _db.Courses
                    .Include(c => c.Category)
                    .Include(c => c.Modules.Select(m => m.Lessons))
                    .Where(c => c.IsPublished && !c.IsArchived)
                    .OrderByDescending(c => c.CreatedDate)
                    .Take(6)
                    .ToList();

                featuredCourses = additionalCourses;
            }

            model.FeaturedCourses = featuredCourses.Select(c => new CourseCardViewModel
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

            // Categories with course counts
            model.Categories = _db.Categories
                .Where(cat => cat.IsActive)
                .OrderBy(cat => cat.DisplayOrder)
                .Select(cat => new CategoryCardViewModel
                {
                    Id = cat.Id,
                    Name = cat.Name,
                    Slug = cat.Slug,
                    Description = cat.Description,
                    IconClass = cat.IconClass,
                    CourseCount = cat.Courses.Count(c => c.IsPublished && !c.IsArchived)
                })
                .ToList();

            // Aggregated platform metrics
            model.TotalCoursesCount = _db.Courses.Count(c => c.IsPublished && !c.IsArchived);
            model.TotalStudentsCount = _db.Enrollments.Select(e => e.UserId).Distinct().Count();
            model.TotalLessonsCount = _db.Lessons.Count(l => l.Module.Course.IsPublished && !l.Module.Course.IsArchived);

            return View(model);
        }

        // GET: /Home/About
        public ActionResult About()
        {
            ViewBag.Message = "Empowering learners worldwide through modern, accessible, enterprise-grade education.";
            return View();
        }

        // GET: /Home/Contact
        public ActionResult Contact()
        {
            ViewBag.Message = "Have a question or feedback? We'd love to hear from you.";
            return View();
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
