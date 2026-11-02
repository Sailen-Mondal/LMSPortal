# Models Component Package

## Overview
Encapsulates domain entities, database relationships, data annotations, validation rules, and the Entity Framework 6 `ApplicationDbContext` for LMSPortal. It implements Code First domain entities for courses, modules, lessons, student enrollments, lesson completions, and identity user accounts.

## Interface
- `[Enums.cs:CourseLevel]` `public enum CourseLevel` (`Beginner`, `Intermediate`, `Advanced`, `AllLevels`)
- `[Enums.cs:EnrollmentStatus]` `public enum EnrollmentStatus` (`Active`, `Completed`, `Dropped`)
- `[Category.cs:Category]` `public class Category` (Id, Name, Slug, Description, IconClass, DisplayOrder, IsActive, Courses)
- `[Course.cs:Course]` `public class Course` (Id, CategoryId, Title, Slug, ShortDescription, FullDescription, InstructorName, InstructorTitle, InstructorBio, InstructorAvatarUrl, ThumbnailUrl, Level, DurationMinutes, IsFeatured, IsPublished, IsArchived, CreatedDate, UpdatedDate, Category, Modules, Enrollments)
- `[Module.cs:Module]` `public class Module` (Id, CourseId, Title, Description, OrderIndex, Course, Lessons)
- `[Lesson.cs:Lesson]` `public class Lesson` (Id, ModuleId, Title, Content, VideoUrl, DurationMinutes, OrderIndex, IsFreePreview, Module, Completions)
- `[Enrollment.cs:Enrollment]` `public class Enrollment` (Id, UserId, CourseId, EnrolledDate, LastAccessedDate, Status, CompletionPercentage, User, Course)
- `[LessonCompletion.cs:LessonCompletion]` `public class LessonCompletion` (Id, UserId, LessonId, CompletedDate, User, Lesson)
- `[IdentityModels.cs:ApplicationUser]` `public class ApplicationUser : IdentityUser` (FullName, AvatarUrl, Bio, CreatedDate, Enrollments, LessonCompletions, GenerateUserIdentityAsync)
- `[IdentityModels.cs:ApplicationDbContext]` `public class ApplicationDbContext : IdentityDbContext<ApplicationUser>` (Categories, Courses, Modules, Lessons, Enrollments, LessonCompletions)
- `[DatabaseSeeder.cs:DatabaseSeeder]` `public static class DatabaseSeeder` (Seed, SeedRoles, SeedAdminUser, SeedStudentUser, SeedCategoriesAndCourses, SeedSampleEnrollments)

## Invariants
- `[Category.cs:Category.Name]` Required, max length 100, unique index `IX_Category_Name`.
- `[Category.cs:Category.Slug]` Required, max length 120, unique index `IX_Category_Slug`.
- `[Course.cs:Course.Slug]` Required, max length 220, unique index `IX_Course_Slug`.
- `[Enrollment.cs:Enrollment]` Composite unique index `IX_Enrollment_User_Course` across `UserId` and `CourseId`.
- `[Enrollment.cs:Enrollment.CompletionPercentage]` Clamped range `[0, 100]`.
- `[LessonCompletion.cs:LessonCompletion]` Composite unique index `IX_LessonCompletion_User_Lesson` across `UserId` and `LessonId`.
- `[IdentityModels.cs:ApplicationDbContext.OnModelCreating]` Cascade delete is disabled on `Course.Enrollments` and `ApplicationUser.Enrollments` to preserve historical student learning records. Cascade delete is enabled on `Course.Modules` and `Module.Lessons`.

## Side Effects
- `[IdentityModels.cs:ApplicationDbContext]` Manages database connection pool, transactions, and SQL Server persistence against `DefaultConnection`.
- `[DatabaseSeeder.cs:DatabaseSeeder.Seed]` Writes seed records (Roles, Users, Categories, Courses, Modules, Lessons, Enrollments, LessonCompletions) to the database on initial deployment.

## Verification
- Run: `& "C:\Program Files\Microsoft Visual Studio\18\Community\Common7\IDE\CommonExtensions\Microsoft\TestWindow\vstest.console.exe" "D:\Projects\.NET\LMSPortal\LMSPortal.Tests\bin\Debug\net48\LMSPortal.Tests.dll" --Tests:DomainModel`
