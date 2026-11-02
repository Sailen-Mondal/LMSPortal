using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using LMSPortal.Areas.Admin.ViewModels;
using LMSPortal.Models;

namespace LMSPortal.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ModuleManagementController : Controller
    {
        private readonly ApplicationDbContext _db;

        public ModuleManagementController()
        {
            _db = new ApplicationDbContext();
        }

        public ModuleManagementController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET: Admin/ModuleManagement?courseId=5
        public ActionResult Index(int courseId)
        {
            var course = _db.Courses
                .Include(c => c.Modules.Select(m => m.Lessons))
                .FirstOrDefault(c => c.Id == courseId);

            if (course == null)
            {
                return HttpNotFound("Course not found.");
            }

            return View(course);
        }

        // GET: Admin/ModuleManagement/Create?courseId=5
        public ActionResult Create(int courseId)
        {
            var course = _db.Courses.Find(courseId);
            if (course == null)
            {
                return HttpNotFound("Course not found.");
            }

            int nextOrder = _db.Modules.Where(m => m.CourseId == courseId).Select(m => (int?)m.OrderIndex).Max() ?? 0;

            var model = new ModuleFormViewModel
            {
                CourseId = courseId,
                CourseTitle = course.Title,
                OrderIndex = nextOrder + 1
            };

            return View(model);
        }

        // POST: Admin/ModuleManagement/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ModuleFormViewModel model)
        {
            if (ModelState.IsValid)
            {
                var module = new Module
                {
                    CourseId = model.CourseId,
                    Title = model.Title.Trim(),
                    Description = model.Description?.Trim(),
                    OrderIndex = model.OrderIndex
                };

                _db.Modules.Add(module);
                _db.SaveChanges();

                TempData["SuccessMessage"] = $"Module '{module.Title}' created successfully.";
                return RedirectToAction("Index", new { courseId = model.CourseId });
            }

            var course = _db.Courses.Find(model.CourseId);
            model.CourseTitle = course != null ? course.Title : string.Empty;
            return View(model);
        }

        // GET: Admin/ModuleManagement/Edit/5
        public ActionResult Edit(int id)
        {
            var module = _db.Modules.Include(m => m.Course).FirstOrDefault(m => m.Id == id);
            if (module == null)
            {
                return HttpNotFound("Module not found.");
            }

            var model = new ModuleFormViewModel
            {
                Id = module.Id,
                CourseId = module.CourseId,
                CourseTitle = module.Course.Title,
                Title = module.Title,
                Description = module.Description,
                OrderIndex = module.OrderIndex
            };

            return View(model);
        }

        // POST: Admin/ModuleManagement/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ModuleFormViewModel model)
        {
            if (ModelState.IsValid)
            {
                var module = _db.Modules.Find(model.Id);
                if (module == null)
                {
                    return HttpNotFound("Module not found.");
                }

                module.Title = model.Title.Trim();
                module.Description = model.Description?.Trim();
                module.OrderIndex = model.OrderIndex;

                _db.SaveChanges();

                TempData["SuccessMessage"] = $"Module '{module.Title}' updated successfully.";
                return RedirectToAction("Index", new { courseId = module.CourseId });
            }

            return View(model);
        }

        // POST: Admin/ModuleManagement/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            var module = _db.Modules.Include(m => m.Lessons).FirstOrDefault(m => m.Id == id);
            if (module == null)
            {
                return HttpNotFound("Module not found.");
            }

            int courseId = module.CourseId;

            // Remove child lessons
            foreach (var lesson in module.Lessons.ToList())
            {
                _db.Lessons.Remove(lesson);
            }

            _db.Modules.Remove(module);
            _db.SaveChanges();

            TempData["SuccessMessage"] = $"Module '{module.Title}' and its lessons were removed.";
            return RedirectToAction("Index", new { courseId = courseId });
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
