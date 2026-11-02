# Admin Controllers Component Package

## Overview
Houses administrative controllers responsible for platform analytics, category taxonomy management, full course and curriculum CRUD lifecycle, lesson content authoring, and student management.

## Interface
- `[AdminDashboardController.cs:AdminDashboardController]` `public class AdminDashboardController : Controller`
  - `Index()`
- `[CategoryManagementController.cs:CategoryManagementController]` `public class CategoryManagementController : Controller`
  - `Index()`, `Create()`, `Create(Category model)`, `Edit(int id)`, `Edit(Category model)`, `Delete(int id)`
- `[CourseManagementController.cs:CourseManagementController]` `public class CourseManagementController : Controller`
  - `Index()`, `Create()`, `Create(CourseFormViewModel model)`, `Edit(int id)`, `Edit(CourseFormViewModel model)`, `TogglePublish(int id)`, `Delete(int id)`, `GenerateSlug(string text)`
- `[ModuleManagementController.cs:ModuleManagementController]` `public class ModuleManagementController : Controller`
  - `Index(int courseId)`, `Create(int courseId)`, `Create(ModuleFormViewModel model)`, `Edit(int id)`, `Edit(ModuleFormViewModel model)`, `Delete(int id)`
- `[LessonManagementController.cs:LessonManagementController]` `public class LessonManagementController : Controller`
  - `Create(int moduleId)`, `Create(LessonFormViewModel model)`, `Edit(int id)`, `Edit(LessonFormViewModel model)`, `Delete(int id)`
- `[StudentManagementController.cs:StudentManagementController]` `public class StudentManagementController : Controller`
  - `Index()`, `Details(string id)`

## Invariants
- `[Authorize(Roles = "Admin")]` Strictly enforced across all administrative controllers; unauthorized requests are rejected by OWIN.
- `[ValidateAntiForgeryToken]` Enforced across all state-altering POST endpoints (`Create`, `Edit`, `TogglePublish`, `Delete`).
- `[CategoryManagementController.cs:Delete]` Deletion is blocked with an error if any courses are assigned to the category.
- `[CourseManagementController.cs:Delete]` Deletion is blocked with an error if active student enrollments exist.
- `[CourseManagementController.cs:Create]` Course slug uniqueness is verified against the database.

## Side Effects
- Creates, updates, and deletes `Courses`, `Modules`, `Lessons`, `Categories`, and `LessonCompletions` in SQL Server.
- Sets `TempData["SuccessMessage"]` or `TempData["ErrorMessage"]` flash messages for user UI feedback.

## Verification
- Run: `& "C:\Program Files\Microsoft Visual Studio\18\Community\Common7\IDE\CommonExtensions\Microsoft\TestWindow\vstest.console.exe" "D:\Projects\.NET\LMSPortal\LMSPortal.Tests\bin\Debug\net48\LMSPortal.Tests.dll" --Tests:AdminCourseGuardTests`
