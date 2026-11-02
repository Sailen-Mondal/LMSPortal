using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using LMSPortal.Areas.Admin.ViewModels;
using LMSPortal.Models;

namespace LMSPortal.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class StudentManagementController : Controller
    {
        private readonly ApplicationDbContext _db;

        public StudentManagementController()
        {
            _db = new ApplicationDbContext();
        }

        public StudentManagementController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET: Admin/StudentManagement
        public ActionResult Index()
        {
            var studentRole = _db.Roles.FirstOrDefault(r => r.Name == "Student");
            var query = _db.Users.AsQueryable();

            if (studentRole != null)
            {
                query = query.Where(u => u.Roles.Any(r => r.RoleId == studentRole.Id));
            }

            var students = query
                .Include(u => u.Enrollments)
                .OrderByDescending(u => u.CreatedDate)
                .Select(u => new AdminStudentItemViewModel
                {
                    Id = u.Id,
                    FullName = u.FullName ?? u.UserName,
                    Email = u.Email,
                    CreatedDate = u.CreatedDate,
                    EnrollmentCount = u.Enrollments.Count,
                    CompletedCoursesCount = u.Enrollments.Count(e => e.Status == EnrollmentStatus.Completed || e.CompletionPercentage == 100)
                })
                .ToList();

            return View(students);
        }

        // GET: Admin/StudentManagement/Details/userId
        public ActionResult Details(string id)
        {
            var student = _db.Users
                .Include(u => u.Enrollments.Select(e => e.Course))
                .FirstOrDefault(u => u.Id == id);

            if (student == null)
            {
                return HttpNotFound("Student not found.");
            }

            return View(student);
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
