using System.Collections.Generic;
using LMSPortal.Models;

namespace LMSPortal.ViewModels
{
    public class CourseCatalogViewModel
    {
        public string SearchQuery { get; set; }
        public string SelectedCategorySlug { get; set; }
        public CourseLevel? SelectedLevel { get; set; }
        public string SortBy { get; set; }
        public List<Category> Categories { get; set; } = new List<Category>();
        public List<CourseCardViewModel> Courses { get; set; } = new List<CourseCardViewModel>();
        public int TotalCount { get; set; }
    }

    public class CourseDetailsViewModel
    {
        public Course Course { get; set; }
        public bool IsEnrolled { get; set; }
        public int StudentProgress { get; set; }
        public int? ResumeLessonId { get; set; }
        public int TotalLessons { get; set; }
        public int TotalDurationMinutes { get; set; }
        public Lesson FirstPreviewLesson { get; set; }
    }
}
