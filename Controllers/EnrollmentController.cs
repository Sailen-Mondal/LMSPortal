using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using LMSPortal.Models;
using Microsoft.AspNet.Identity;

namespace LMSPortal.Controllers
{
    [Authorize]
    public class EnrollmentController : Controller
    {
        private readonly ApplicationDbContext _db;

        public EnrollmentController()
        {
            _db = new ApplicationDbContext();
        }

        public EnrollmentController(ApplicationDbContext db)
        {
            _db = db;
        }

        // POST: /Enrollment/Enroll
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Enroll(int courseId)
        {
            string userId = User.Identity.GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            }

            // 1. Verify course existence and availability
            var course = _db.Courses.FirstOrDefault(c => c.Id == courseId);
            if (course == null)
            {
                return HttpNotFound("Course not found.");
            }

            if ((!course.IsPublished || course.IsArchived) && !User.IsInRole("Admin"))
            {
                TempData["ErrorMessage"] = "This course is not open for enrollment.";
                return RedirectToAction("Index", "Course");
            }

            // 2. Application-level duplicate check
            var existingEnrollment = _db.Enrollments.FirstOrDefault(e => e.CourseId == courseId && e.UserId == userId);
            if (existingEnrollment != null)
            {
                TempData["InfoMessage"] = "You are already enrolled in this course.";
                return RedirectToAction("Confirmation", new { id = courseId });
            }

            // 3. Create and persist new free enrollment record
            var enrollment = new Enrollment
            {
                UserId = userId,
                CourseId = courseId,
                EnrolledDate = DateTime.UtcNow,
                LastAccessedDate = DateTime.UtcNow,
                Status = EnrollmentStatus.Active,
                CompletionPercentage = 0
            };

            try
            {
                _db.Enrollments.Add(enrollment);
                _db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                // Handle potential race condition caught by the database unique index IX_Enrollment_User_Course
                TempData["InfoMessage"] = "You are already enrolled in this course.";
                return RedirectToAction("Confirmation", new { id = courseId });
            }

            TempData["SuccessMessage"] = $"Congratulations! You have successfully enrolled in '{course.Title}'.";
            return RedirectToAction("Confirmation", new { id = courseId });
        }

        // GET: /Enrollment/Confirmation/{id}
        public ActionResult Confirmation(int id)
        {
            string userId = User.Identity.GetUserId();
            var enrollment = _db.Enrollments
                .Include(e => e.Course.Category)
                .Include(e => e.Course.Modules.Select(m => m.Lessons))
                .FirstOrDefault(e => e.CourseId == id && e.UserId == userId);

            if (enrollment == null)
            {
                // If not enrolled or admin checking, verify course exists
                var course = _db.Courses
                    .Include(c => c.Category)
                    .Include(c => c.Modules.Select(m => m.Lessons))
                    .FirstOrDefault(c => c.Id == id);

                if (course == null) return HttpNotFound();

                ViewBag.Course = course;
                ViewBag.FirstLessonId = course.Modules
                    .OrderBy(m => m.OrderIndex)
                    .SelectMany(m => m.Lessons)
                    .OrderBy(l => l.OrderIndex)
                    .FirstOrDefault()?.Id;

                return View();
            }

            ViewBag.Course = enrollment.Course;
            ViewBag.FirstLessonId = enrollment.Course.Modules
                .OrderBy(m => m.OrderIndex)
                .SelectMany(m => m.Lessons)
                .OrderBy(l => l.OrderIndex)
                .FirstOrDefault()?.Id;

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
