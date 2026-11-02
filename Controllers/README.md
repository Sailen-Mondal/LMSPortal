# Controllers Component Package

## Overview
Houses ASP.NET MVC controllers servicing the public storefront, student dashboard, course catalog, course enrollment pipeline, authentication lifecycle, and interactive media player.

## Interface
- `[AccountController.cs:AccountController]` `public class AccountController : Controller`
  - `Login`, `Register`, `LogOff`, `Logout`, `SignInManager`, `UserManager`
- `[CourseController.cs:CourseController]` `public class CourseController : Controller`
  - `Index(string search, string category, CourseLevel? level, string sort)`, `Details(string slug)`
- `[EnrollmentController.cs:EnrollmentController]` `public class EnrollmentController : Controller`
  - `Enroll(int courseId)`, `Confirmation(int id)`
- `[HomeController.cs:HomeController]` `public class HomeController : Controller`
  - `Index()`, `About()`, `Contact()`
- `[LearningController.cs:LearningController]` `public class LearningController : Controller`
  - `Player(int courseId, int? lessonId)`, `ToggleComplete(int courseId, int lessonId)`, `FormatEmbedVideoUrl(string url)`
- `[StudentController.cs:StudentController]` `public class StudentController : Controller`
  - `Dashboard()`, `MyCourses(string filter = "all")`

## Invariants
- `[AccountController.cs:AuthorizeAttribute]` Secured by default with `[Authorize]`; only `Login` and `Register` explicitly allow anonymous access.
- `[AccountController.cs:PerformSignOut]` Sign-out terminates OWIN cookie authentication (`AuthenticationManager.SignOut`), abandons HTTP session (`Session.Abandon()`), and writes an expired `LMSPortal_Auth` cookie to `Response.Cookies`.
- `[CourseController.cs:CourseController.Index]` Only displays courses where `IsPublished == true` and `IsArchived == false`.
- `[EnrollmentController.cs:EnrollmentController.Enroll]` Validates authentication, verifies course availability, and blocks duplicate enrollments.
- `[LearningController.cs:LearningController.Player]` Enforces access control: lessons marked `IsFreePreview` allow unauthenticated/unenrolled access, while non-preview lessons strictly require active enrollment or `Admin` role.
- `[LearningController.cs:LearningController.ToggleComplete]` Recalculates course completion percentage and automatically transitions enrollment status between `Active` and `Completed`.

## Side Effects
- Mutates SQL Server database tables (`Enrollments`, `LessonCompletions`, `Users`) via Entity Framework `SaveChanges`.
- Manages HTTP authentication cookies (`LMSPortal_Auth`) and user session states.

## Verification
- Run: `& "C:\Program Files\Microsoft Visual Studio\18\Community\Common7\IDE\CommonExtensions\Microsoft\TestWindow\vstest.console.exe" "D:\Projects\.NET\LMSPortal\LMSPortal.Tests\bin\Debug\net48\LMSPortal.Tests.dll" --Tests:LearningPlayerTests`
