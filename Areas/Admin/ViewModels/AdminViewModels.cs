using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using LMSPortal.Models;

namespace LMSPortal.Areas.Admin.ViewModels
{
    public class AdminDashboardViewModel
    {
        public AdminDashboardViewModel()
        {
            RecentCourses = new List<Course>();
            RecentEnrollments = new List<AdminEnrollmentItemViewModel>();
        }

        public int TotalCourses { get; set; }
        public int PublishedCourses { get; set; }
        public int DraftCourses { get; set; }
        public int TotalStudents { get; set; }
        public int TotalEnrollments { get; set; }
        public int TotalCompletions { get; set; }
        public int TotalCategories { get; set; }
        public int TotalLessons { get; set; }

        public List<Course> RecentCourses { get; set; }
        public List<AdminEnrollmentItemViewModel> RecentEnrollments { get; set; }
    }

    public class AdminEnrollmentItemViewModel
    {
        public int EnrollmentId { get; set; }
        public string StudentName { get; set; }
        public string StudentEmail { get; set; }
        public string CourseTitle { get; set; }
        public DateTime EnrolledDate { get; set; }
        public int CompletionPercentage { get; set; }
        public EnrollmentStatus Status { get; set; }
    }

    public class CourseFormViewModel
    {
        public CourseFormViewModel()
        {
            Categories = new List<SelectListItem>();
            Level = CourseLevel.Beginner;
            DurationMinutes = 60;
            IsPublished = true;
        }

        public int Id { get; set; }

        [Required(ErrorMessage = "Course title is required.")]
        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters.")]
        [Display(Name = "Course Title")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Course URL slug is required.")]
        [StringLength(220, ErrorMessage = "Slug cannot exceed 220 characters.")]
        [Display(Name = "URL Slug")]
        public string Slug { get; set; }

        [Required(ErrorMessage = "Short summary is required.")]
        [StringLength(500, ErrorMessage = "Short description cannot exceed 500 characters.")]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Short Summary")]
        public string ShortDescription { get; set; }

        [DataType(DataType.MultilineText)]
        [Display(Name = "Full Course Description")]
        public string FullDescription { get; set; }

        [Required(ErrorMessage = "Category is required.")]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        [Display(Name = "Skill Level")]
        public CourseLevel Level { get; set; }

        [Display(Name = "Total Duration (Minutes)")]
        [Range(1, 10000, ErrorMessage = "Duration must be at least 1 minute.")]
        public int DurationMinutes { get; set; }

        [StringLength(500)]
        [Display(Name = "Thumbnail Image URL")]
        public string ThumbnailUrl { get; set; }

        [Required(ErrorMessage = "Instructor name is required.")]
        [StringLength(100)]
        [Display(Name = "Instructor Name")]
        public string InstructorName { get; set; }

        [StringLength(500)]
        [Display(Name = "Instructor Bio")]
        public string InstructorBio { get; set; }

        [Display(Name = "Published (visible in catalog)")]
        public bool IsPublished { get; set; }

        public IEnumerable<SelectListItem> Categories { get; set; }
    }

    public class ModuleFormViewModel
    {
        public ModuleFormViewModel()
        {
            OrderIndex = 1;
        }

        public int Id { get; set; }

        [Required]
        public int CourseId { get; set; }

        public string CourseTitle { get; set; }

        [Required(ErrorMessage = "Module title is required.")]
        [StringLength(150, ErrorMessage = "Module title cannot exceed 150 characters.")]
        [Display(Name = "Module Title")]
        public string Title { get; set; }

        [StringLength(500)]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Order Index")]
        public int OrderIndex { get; set; }
    }

    public class LessonFormViewModel
    {
        public LessonFormViewModel()
        {
            OrderIndex = 1;
            DurationMinutes = 10;
            IsFreePreview = false;
        }

        public int Id { get; set; }

        [Required]
        public int ModuleId { get; set; }

        public int CourseId { get; set; }
        public string ModuleTitle { get; set; }
        public string CourseTitle { get; set; }

        [Required(ErrorMessage = "Lesson title is required.")]
        [StringLength(150, ErrorMessage = "Lesson title cannot exceed 150 characters.")]
        [Display(Name = "Lesson Title")]
        public string Title { get; set; }

        [DataType(DataType.MultilineText)]
        [Display(Name = "Lesson Content / Notes")]
        public string Content { get; set; }

        [StringLength(500)]
        [Display(Name = "Video URL / Embed Link (YouTube or Direct)")]
        public string VideoUrl { get; set; }

        [Display(Name = "Duration (Minutes)")]
        [Range(1, 1000, ErrorMessage = "Duration must be at least 1 minute.")]
        public int DurationMinutes { get; set; }

        [Display(Name = "Order Index")]
        public int OrderIndex { get; set; }

        [Display(Name = "Allow Free Preview (accessible without enrollment)")]
        public bool IsFreePreview { get; set; }
    }

    public class AdminStudentItemViewModel
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public DateTime CreatedDate { get; set; }
        public int EnrollmentCount { get; set; }
        public int CompletedCoursesCount { get; set; }
    }
}
