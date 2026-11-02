using System;
using System.Data.Entity;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using LMSPortal.Areas.Admin.ViewModels;
using LMSPortal.Models;

namespace LMSPortal.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CourseManagementController : Controller
    {
        private readonly ApplicationDbContext _db;

        public CourseManagementController()
        {
            _db = new ApplicationDbContext();
        }

        public CourseManagementController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET: Admin/CourseManagement
        public ActionResult Index()
        {
            var courses = _db.Courses
                .Include(c => c.Category)
                .Include(c => c.Modules)
                .Include(c => c.Enrollments)
                .OrderByDescending(c => c.CreatedDate)
                .ToList();

            return View(courses);
        }

        // GET: Admin/CourseManagement/Create
        public ActionResult Create()
        {
            var model = new CourseFormViewModel
            {
                Categories = GetCategoriesSelectList()
            };
            return View(model);
        }

        // POST: Admin/CourseManagement/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CourseFormViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Slug))
            {
                model.Slug = GenerateSlug(model.Title);
            }
            else
            {
                model.Slug = GenerateSlug(model.Slug);
            }

            if (_db.Courses.Any(c => c.Slug == model.Slug))
            {
                ModelState.AddModelError("Slug", "This URL slug is already in use by another course.");
            }

            if (ModelState.IsValid)
            {
                var course = new Course
                {
                    Title = model.Title.Trim(),
                    Slug = model.Slug.Trim(),
                    ShortDescription = model.ShortDescription?.Trim(),
                    FullDescription = model.FullDescription?.Trim(),
                    CategoryId = model.CategoryId,
                    Level = model.Level,
                    DurationMinutes = model.DurationMinutes,
                    ThumbnailUrl = model.ThumbnailUrl?.Trim(),
                    InstructorName = model.InstructorName.Trim(),
                    InstructorBio = model.InstructorBio?.Trim(),
                    IsPublished = model.IsPublished,
                    CreatedDate = DateTime.UtcNow,
                    UpdatedDate = DateTime.UtcNow
                };

                _db.Courses.Add(course);
                _db.SaveChanges();

                TempData["SuccessMessage"] = $"Course '{course.Title}' created successfully! You can now add curriculum modules and lessons.";
                return RedirectToAction("Index", "ModuleManagement", new { courseId = course.Id });
            }

            model.Categories = GetCategoriesSelectList(model.CategoryId);
            return View(model);
        }

        // GET: Admin/CourseManagement/Edit/5
        public ActionResult Edit(int id)
        {
            var course = _db.Courses.Find(id);
            if (course == null)
            {
                return HttpNotFound("Course not found.");
            }

            var model = new CourseFormViewModel
            {
                Id = course.Id,
                Title = course.Title,
                Slug = course.Slug,
                ShortDescription = course.ShortDescription,
                FullDescription = course.FullDescription,
                CategoryId = course.CategoryId,
                Level = course.Level,
                DurationMinutes = course.DurationMinutes,
                ThumbnailUrl = course.ThumbnailUrl,
                InstructorName = course.InstructorName,
                InstructorBio = course.InstructorBio,
                IsPublished = course.IsPublished,
                Categories = GetCategoriesSelectList(course.CategoryId)
            };

            return View(model);
        }

        // POST: Admin/CourseManagement/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CourseFormViewModel model)
        {
            model.Slug = GenerateSlug(model.Slug ?? model.Title);

            if (_db.Courses.Any(c => c.Slug == model.Slug && c.Id != model.Id))
            {
                ModelState.AddModelError("Slug", "This URL slug is already taken by another course.");
            }

            if (ModelState.IsValid)
            {
                var course = _db.Courses.Find(model.Id);
                if (course == null)
                {
                    return HttpNotFound("Course not found.");
                }

                course.Title = model.Title.Trim();
                course.Slug = model.Slug.Trim();
                course.ShortDescription = model.ShortDescription?.Trim();
                course.FullDescription = model.FullDescription?.Trim();
                course.CategoryId = model.CategoryId;
                course.Level = model.Level;
                course.DurationMinutes = model.DurationMinutes;
                course.ThumbnailUrl = model.ThumbnailUrl?.Trim();
                course.InstructorName = model.InstructorName.Trim();
                course.InstructorBio = model.InstructorBio?.Trim();
                course.IsPublished = model.IsPublished;
                course.UpdatedDate = DateTime.UtcNow;

                _db.SaveChanges();

                TempData["SuccessMessage"] = $"Course '{course.Title}' updated successfully.";
                return RedirectToAction("Index");
            }

            model.Categories = GetCategoriesSelectList(model.CategoryId);
            return View(model);
        }

        // POST: Admin/CourseManagement/TogglePublish/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TogglePublish(int id)
        {
            var course = _db.Courses.Find(id);
            if (course == null)
            {
                return HttpNotFound("Course not found.");
            }

            course.IsPublished = !course.IsPublished;
            course.UpdatedDate = DateTime.UtcNow;
            _db.SaveChanges();

            TempData["SuccessMessage"] = $"Course '{course.Title}' is now {(course.IsPublished ? "Published (Live)" : "Unpublished / Archived")}.";
            return RedirectToAction("Index");
        }

        // POST: Admin/CourseManagement/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            var course = _db.Courses
                .Include(c => c.Modules.Select(m => m.Lessons))
                .FirstOrDefault(c => c.Id == id);

            if (course == null)
            {
                return HttpNotFound("Course not found.");
            }

            // CRITICAL INTEGRITY CHECK: Prevent deletion if student enrollments exist
            int enrollmentCount = _db.Enrollments.Count(e => e.CourseId == id);
            if (enrollmentCount > 0)
            {
                TempData["ErrorMessage"] = $"Cannot delete course '{course.Title}' because {enrollmentCount} student(s) are currently enrolled. Archive or unpublish the course instead to protect student learning history.";
                return RedirectToAction("Index");
            }

            // Safe to delete course and its curriculum
            foreach (var module in course.Modules.ToList())
            {
                foreach (var lesson in module.Lessons.ToList())
                {
                    _db.Lessons.Remove(lesson);
                }
                _db.Modules.Remove(module);
            }

            _db.Courses.Remove(course);
            _db.SaveChanges();

            TempData["SuccessMessage"] = $"Course '{course.Title}' was successfully deleted.";
            return RedirectToAction("Index");
        }

        private SelectList GetCategoriesSelectList(int? selectedId = null)
        {
            var categories = _db.Categories
                .Where(c => c.IsActive)
                .OrderBy(c => c.DisplayOrder)
                .ThenBy(c => c.Name)
                .ToList();

            return new SelectList(categories, "Id", "Name", selectedId);
        }

        public static string GenerateSlug(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return string.Empty;
            string str = text.ToLowerInvariant();
            str = Regex.Replace(str, @"\s+", "-");
            str = Regex.Replace(str, @"[^a-z0-9\-]", "");
            str = Regex.Replace(str, @"-+", "-");
            return str.Trim('-');
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
