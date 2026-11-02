# Admin ViewModels Component Package

## Overview
Provides specialized ViewModels for the administrative portal, handling course creation and editing, module and lesson forms, student profile inspect views, and KPI metric aggregation.

## Interface
- `[AdminViewModels.cs:AdminDashboardViewModel]` `public class AdminDashboardViewModel` (TotalCourses, PublishedCourses, DraftCourses, TotalStudents, TotalEnrollments, TotalCompletions, TotalCategories, TotalLessons, RecentCourses, RecentEnrollments)
- `[AdminViewModels.cs:AdminEnrollmentItemViewModel]` `public class AdminEnrollmentItemViewModel` (EnrollmentId, StudentName, StudentEmail, CourseTitle, EnrolledDate, CompletionPercentage, Status)
- `[AdminViewModels.cs:CourseFormViewModel]` `public class CourseFormViewModel` (Id, Title, Slug, ShortDescription, FullDescription, CategoryId, Level, DurationMinutes, ThumbnailUrl, InstructorName, InstructorBio, IsPublished, Categories)
- `[AdminViewModels.cs:ModuleFormViewModel]` `public class ModuleFormViewModel` (Id, CourseId, CourseTitle, Title, Description, OrderIndex)
- `[AdminViewModels.cs:LessonFormViewModel]` `public class LessonFormViewModel` (Id, ModuleId, CourseId, ModuleTitle, CourseTitle, Title, Content, VideoUrl, DurationMinutes, OrderIndex, IsFreePreview)
- `[AdminViewModels.cs:AdminStudentItemViewModel]` `public class AdminStudentItemViewModel` (Id, FullName, Email, CreatedDate, EnrollmentCount, CompletedCoursesCount)

## Invariants
- `[AdminViewModels.cs:CourseFormViewModel]` `Title` required (max 200 chars), `Slug` required (max 220 chars), `ShortDescription` required (max 500 chars), `CategoryId` required, `DurationMinutes` range [1, 10000], `InstructorName` required (max 100 chars).
- `[AdminViewModels.cs:ModuleFormViewModel]` `CourseId` required, `Title` required (max 150 chars).
- `[AdminViewModels.cs:LessonFormViewModel]` `ModuleId` required, `Title` required (max 150 chars), `DurationMinutes` range [1, 1000].

## Side Effects
- None

## Verification
- Run: `& "C:\Program Files\Microsoft Visual Studio\18\Community\Common7\IDE\CommonExtensions\Microsoft\TestWindow\vstest.console.exe" "D:\Projects\.NET\LMSPortal\LMSPortal.Tests\bin\Debug\net48\LMSPortal.Tests.dll" --Tests:AdminCourseGuardTests`
