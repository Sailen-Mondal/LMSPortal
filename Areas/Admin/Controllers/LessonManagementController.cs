using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using LMSPortal.Areas.Admin.ViewModels;
using LMSPortal.Models;

namespace LMSPortal.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class LessonManagementController : Controller
    {
        private readonly ApplicationDbContext _db;

        public LessonManagementController()
        {
            _db = new ApplicationDbContext();
        }

        public LessonManagementController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET: Admin/LessonManagement/Create?moduleId=5
        public ActionResult Create(int moduleId)
        {
            var module = _db.Modules.Include(m => m.Course).FirstOrDefault(m => m.Id == moduleId);
            if (module == null)
            {
                return HttpNotFound("Module not found.");
            }

            int nextOrder = _db.Lessons.Where(l => l.ModuleId == moduleId).Select(l => (int?)l.OrderIndex).Max() ?? 0;

            var model = new LessonFormViewModel
            {
                ModuleId = moduleId,
                CourseId = module.CourseId,
                ModuleTitle = module.Title,
                CourseTitle = module.Course.Title,
                OrderIndex = nextOrder + 1,
                DurationMinutes = 10,
                IsFreePreview = false
            };

            return View(model);
        }

        // POST: Admin/LessonManagement/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(LessonFormViewModel model)
        {
            if (ModelState.IsValid)
            {
                var lesson = new Lesson
                {
                    ModuleId = model.ModuleId,
                    Title = model.Title.Trim(),
                    Content = model.Content?.Trim(),
                    VideoUrl = model.VideoUrl?.Trim(),
                    DurationMinutes = model.DurationMinutes,
                    OrderIndex = model.OrderIndex,
                    IsFreePreview = model.IsFreePreview
                };

                _db.Lessons.Add(lesson);
                _db.SaveChanges();

                var module = _db.Modules.Find(model.ModuleId);
                TempData["SuccessMessage"] = $"Lesson '{lesson.Title}' added successfully.";
                return RedirectToAction("Index", "ModuleManagement", new { courseId = module.CourseId });
            }

            return View(model);
        }

        // GET: Admin/LessonManagement/Edit/5
        public ActionResult Edit(int id)
        {
            var lesson = _db.Lessons.Include(l => l.Module.Course).FirstOrDefault(l => l.Id == id);
            if (lesson == null)
            {
                return HttpNotFound("Lesson not found.");
            }

            var model = new LessonFormViewModel
            {
                Id = lesson.Id,
                ModuleId = lesson.ModuleId,
                CourseId = lesson.Module.CourseId,
                ModuleTitle = lesson.Module.Title,
                CourseTitle = lesson.Module.Course.Title,
                Title = lesson.Title,
                Content = lesson.Content,
                VideoUrl = lesson.VideoUrl,
                DurationMinutes = lesson.DurationMinutes,
                OrderIndex = lesson.OrderIndex,
                IsFreePreview = lesson.IsFreePreview
            };

            return View(model);
        }

        // POST: Admin/LessonManagement/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(LessonFormViewModel model)
        {
            if (ModelState.IsValid)
            {
                var lesson = _db.Lessons.Include(l => l.Module).FirstOrDefault(l => l.Id == model.Id);
                if (lesson == null)
                {
                    return HttpNotFound("Lesson not found.");
                }

                lesson.Title = model.Title.Trim();
                lesson.Content = model.Content?.Trim();
                lesson.VideoUrl = model.VideoUrl?.Trim();
                lesson.DurationMinutes = model.DurationMinutes;
                lesson.OrderIndex = model.OrderIndex;
                lesson.IsFreePreview = model.IsFreePreview;

                _db.SaveChanges();

                TempData["SuccessMessage"] = $"Lesson '{lesson.Title}' updated successfully.";
                return RedirectToAction("Index", "ModuleManagement", new { courseId = lesson.Module.CourseId });
            }

            return View(model);
        }

        // POST: Admin/LessonManagement/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            var lesson = _db.Lessons.Include(l => l.Module).FirstOrDefault(l => l.Id == id);
            if (lesson == null)
            {
                return HttpNotFound("Lesson not found.");
            }

            int courseId = lesson.Module.CourseId;

            // Remove completions for this lesson
            var completions = _db.LessonCompletions.Where(c => c.LessonId == id).ToList();
            foreach (var c in completions)
            {
                _db.LessonCompletions.Remove(c);
            }

            _db.Lessons.Remove(lesson);
            _db.SaveChanges();

            TempData["SuccessMessage"] = $"Lesson '{lesson.Title}' was removed.";
            return RedirectToAction("Index", "ModuleManagement", new { courseId = courseId });
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
