using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using LMSPortal.Areas.Admin.ViewModels;
using LMSPortal.Models;

namespace LMSPortal.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminDashboardController : Controller
    {
        private readonly ApplicationDbContext _db;

        public AdminDashboardController()
        {
            _db = new ApplicationDbContext();
        }

        public AdminDashboardController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET: Admin/AdminDashboard
        public ActionResult Index()
        {
            int totalCourses = _db.Courses.Count();
            int publishedCourses = _db.Courses.Count(c => c.IsPublished);
            int draftCourses = totalCourses - publishedCourses;
            int totalEnrollments = _db.Enrollments.Count();
            int totalCompletions = _db.Enrollments.Count(e => e.Status == EnrollmentStatus.Completed || e.CompletionPercentage == 100);
            int totalCategories = _db.Categories.Count();
            int totalLessons = _db.Lessons.Count();

            // Registered students
            var studentRole = _db.Roles.FirstOrDefault(r => r.Name == "Student");
            int totalStudents = studentRole != null
                ? _db.Users.Count(u => u.Roles.Any(r => r.RoleId == studentRole.Id))
                : _db.Users.Count();

            var recentCourses = _db.Courses
                .Include(c => c.Category)
                .Include(c => c.Enrollments)
                .OrderByDescending(c => c.CreatedDate)
                .Take(5)
                .ToList();

            var recentEnrollments = _db.Enrollments
                .Include(e => e.User)
                .Include(e => e.Course)
                .OrderByDescending(e => e.EnrolledDate)
                .Take(6)
                .Select(e => new AdminEnrollmentItemViewModel
                {
                    EnrollmentId = e.Id,
                    StudentName = e.User.FullName ?? e.User.UserName,
                    StudentEmail = e.User.Email,
                    CourseTitle = e.Course.Title,
                    EnrolledDate = e.EnrolledDate,
                    CompletionPercentage = e.CompletionPercentage,
                    Status = e.Status
                })
                .ToList();

            var model = new AdminDashboardViewModel
            {
                TotalCourses = totalCourses,
                PublishedCourses = publishedCourses,
                DraftCourses = draftCourses,
                TotalStudents = totalStudents,
                TotalEnrollments = totalEnrollments,
                TotalCompletions = totalCompletions,
                TotalCategories = totalCategories,
                TotalLessons = totalLessons,
                RecentCourses = recentCourses,
                RecentEnrollments = recentEnrollments
            };

            return View(model);
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
