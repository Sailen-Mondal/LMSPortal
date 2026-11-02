# ViewModels Component Package

## Overview
Defines strongly typed ViewModels and Data Transfer Objects (DTOs) for LMSPortal's public storefront, student learning dashboard, courses catalog, account authentication, and interactive learning media player.

## Interface
- `[AccountViewModels.cs:LoginViewModel]` `public class LoginViewModel` (Email, Password, RememberMe, ReturnUrl)
- `[AccountViewModels.cs:RegisterViewModel]` `public class RegisterViewModel` (FullName, Email, Password, ConfirmPassword, ReturnUrl)
- `[HomeViewModels.cs:LandingPageViewModel]` `public class LandingPageViewModel` (FeaturedCourses, Categories, Metrics)
- `[HomeViewModels.cs:CategoryCardViewModel]` `public class CategoryCardViewModel` (Id, Name, Slug, Description, IconClass, CourseCount)
- `[HomeViewModels.cs:CourseCardViewModel]` `public class CourseCardViewModel` (Id, Title, Slug, ShortDescription, ThumbnailUrl, InstructorName, Level, DurationMinutes, LessonCount, IsFeatured)
- `[CourseViewModels.cs:CourseCatalogViewModel]` `public class CourseCatalogViewModel` (SearchQuery, SelectedCategorySlug, SelectedLevel, SortBy, Categories, Courses, TotalCount)
- `[CourseViewModels.cs:CourseDetailsViewModel]` `public class CourseDetailsViewModel` (Course, IsEnrolled, StudentProgress, ResumeLessonId, TotalLessons, TotalDurationMinutes, FirstPreviewLesson)
- `[StudentViewModels.cs:StudentDashboardViewModel]` `public class StudentDashboardViewModel` (StudentName, AvatarUrl, MemberSince, Metrics, EnrolledCourses)
- `[StudentViewModels.cs:StudentCourseCardViewModel]` `public class StudentCourseCardViewModel` (EnrollmentId, CourseId, Title, Slug, CategoryName, CompletionPercentage, Status, NextLessonId)
- `[StudentViewModels.cs:StudentMyCoursesViewModel]` `public class StudentMyCoursesViewModel` (Filter, Courses, Metrics)
- `[LearningViewModels.cs:LearningPlayerViewModel]` `public class LearningPlayerViewModel` (CourseId, CourseTitle, CurrentLessonId, CurrentLessonTitle, CurrentLessonVideoUrl, Modules, CompletedLessonsCount)
- `[LearningViewModels.cs:LearningModuleNavViewModel]` `public class LearningModuleNavViewModel` (ModuleId, Title, OrderIndex, Lessons)
- `[LearningViewModels.cs:LearningLessonNavViewModel]` `public class LearningLessonNavViewModel` (LessonId, Title, OrderIndex, DurationMinutes, IsFreePreview, IsCurrent, IsCompleted)

## Invariants
- `[AccountViewModels.cs:LoginViewModel.Email]` `Required`, `EmailAddress` validation attributes.
- `[AccountViewModels.cs:LoginViewModel.Password]` `Required` validation attribute.
- `[AccountViewModels.cs:RegisterViewModel.FullName]` `Required`, `StringLength(100, MinimumLength = 2)` constraints.
- `[AccountViewModels.cs:RegisterViewModel.Email]` `Required`, `EmailAddress` validation attributes.
- `[AccountViewModels.cs:RegisterViewModel.Password]` `Required`, `StringLength(100, MinimumLength = 6)` constraints.
- `[AccountViewModels.cs:RegisterViewModel.ConfirmPassword]` `Compare("Password")` equality constraint.

## Side Effects
- None

## Verification
- Run: `& "C:\Program Files\Microsoft Visual Studio\18\Community\Common7\IDE\CommonExtensions\Microsoft\TestWindow\vstest.console.exe" "D:\Projects\.NET\LMSPortal\LMSPortal.Tests\bin\Debug\net48\LMSPortal.Tests.dll" --Tests:AccountTests`
