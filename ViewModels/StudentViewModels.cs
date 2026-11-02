using System;
using System.Collections.Generic;
using LMSPortal.Models;

namespace LMSPortal.ViewModels
{
    public class StudentDashboardViewModel
    {
        public StudentDashboardViewModel()
        {
            EnrolledCourses = new List<StudentCourseCardViewModel>();
        }

        public string StudentName { get; set; }
        public string AvatarUrl { get; set; }
        public DateTime MemberSince { get; set; }

        public int TotalEnrolled { get; set; }
        public int CompletedCourses { get; set; }
        public int InProgressCourses { get; set; }
        public int TotalLessonsCompleted { get; set; }
        public double TotalHoursLearned { get; set; }
        public int OverallProgressPercentage { get; set; }

        public StudentCourseCardViewModel ActiveCourse { get; set; }
        public List<StudentCourseCardViewModel> EnrolledCourses { get; set; }
    }

    public class StudentCourseCardViewModel
    {
        public int EnrollmentId { get; set; }
        public int CourseId { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
        public string CategoryName { get; set; }
        public string CategoryColor { get; set; }
        public string ThumbnailUrl { get; set; }
        public string InstructorName { get; set; }
        public CourseLevel Level { get; set; }
        public DateTime EnrolledDate { get; set; }
        public DateTime? LastAccessedDate { get; set; }
        public int CompletionPercentage { get; set; }
        public EnrollmentStatus Status { get; set; }
        public int CompletedLessonsCount { get; set; }
        public int TotalLessonsCount { get; set; }

        public int? NextLessonId { get; set; }
        public string NextLessonTitle { get; set; }
        public string NextModuleTitle { get; set; }
    }

    public class StudentMyCoursesViewModel
    {
        public StudentMyCoursesViewModel()
        {
            Courses = new List<StudentCourseCardViewModel>();
        }

        public string Filter { get; set; } // "all", "in-progress", "completed"
        public List<StudentCourseCardViewModel> Courses { get; set; }
        public int TotalCount { get; set; }
        public int InProgressCount { get; set; }
        public int CompletedCount { get; set; }
    }
}
