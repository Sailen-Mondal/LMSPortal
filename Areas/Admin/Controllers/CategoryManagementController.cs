using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using LMSPortal.Areas.Admin.Controllers;
using LMSPortal.Models;

namespace LMSPortal.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CategoryManagementController : Controller
    {
        private readonly ApplicationDbContext _db;

        public CategoryManagementController()
        {
            _db = new ApplicationDbContext();
        }

        public CategoryManagementController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET: Admin/CategoryManagement
        public ActionResult Index()
        {
            var categories = _db.Categories
                .Include(c => c.Courses)
                .OrderBy(c => c.DisplayOrder)
                .ThenBy(c => c.Name)
                .ToList();

            return View(categories);
        }

        // GET: Admin/CategoryManagement/Create
        public ActionResult Create()
        {
            return View(new Category { DisplayOrder = 1, IsActive = true, IconClass = "bi-journal-code" });
        }

        // POST: Admin/CategoryManagement/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Category model)
        {
            if (string.IsNullOrWhiteSpace(model.Slug))
            {
                model.Slug = CourseManagementController.GenerateSlug(model.Name);
            }
            else
            {
                model.Slug = CourseManagementController.GenerateSlug(model.Slug);
            }

            if (_db.Categories.Any(c => c.Name == model.Name))
            {
                ModelState.AddModelError("Name", "A category with this name already exists.");
            }
            if (_db.Categories.Any(c => c.Slug == model.Slug))
            {
                ModelState.AddModelError("Slug", "A category with this slug already exists.");
            }

            if (ModelState.IsValid)
            {
                _db.Categories.Add(model);
                _db.SaveChanges();

                TempData["SuccessMessage"] = $"Category '{model.Name}' created successfully.";
                return RedirectToAction("Index");
            }

            return View(model);
        }

        // GET: Admin/CategoryManagement/Edit/5
        public ActionResult Edit(int id)
        {
            var category = _db.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound("Category not found.");
            }
            return View(category);
        }

        // POST: Admin/CategoryManagement/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Category model)
        {
            model.Slug = CourseManagementController.GenerateSlug(model.Slug ?? model.Name);

            if (_db.Categories.Any(c => c.Name == model.Name && c.Id != model.Id))
            {
                ModelState.AddModelError("Name", "Another category already has this name.");
            }
            if (_db.Categories.Any(c => c.Slug == model.Slug && c.Id != model.Id))
            {
                ModelState.AddModelError("Slug", "Another category already has this slug.");
            }

            if (ModelState.IsValid)
            {
                var category = _db.Categories.Find(model.Id);
                if (category == null)
                {
                    return HttpNotFound("Category not found.");
                }

                category.Name = model.Name.Trim();
                category.Slug = model.Slug.Trim();
                category.Description = model.Description?.Trim();
                category.IconClass = model.IconClass?.Trim();
                category.DisplayOrder = model.DisplayOrder;
                category.IsActive = model.IsActive;

                _db.SaveChanges();

                TempData["SuccessMessage"] = $"Category '{category.Name}' updated successfully.";
                return RedirectToAction("Index");
            }

            return View(model);
        }

        // POST: Admin/CategoryManagement/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            var category = _db.Categories.Include(c => c.Courses).FirstOrDefault(c => c.Id == id);
            if (category == null)
            {
                return HttpNotFound("Category not found.");
            }

            if (category.Courses.Any())
            {
                TempData["ErrorMessage"] = $"Cannot delete category '{category.Name}' because it contains {category.Courses.Count} course(s). Reassign or remove courses first.";
                return RedirectToAction("Index");
            }

            _db.Categories.Remove(category);
            _db.SaveChanges();

            TempData["SuccessMessage"] = $"Category '{category.Name}' was deleted.";
            return RedirectToAction("Index");
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
