using System.Collections.Generic;
using LMSPortal.Models;

namespace LMSPortal.ViewModels
{
    public class LandingPageViewModel
    {
        public List<CourseCardViewModel> FeaturedCourses { get; set; } = new List<CourseCardViewModel>();
        public List<CategoryCardViewModel> Categories { get; set; } = new List<CategoryCardViewModel>();
        public int TotalCoursesCount { get; set; }
        public int TotalStudentsCount { get; set; }
        public int TotalLessonsCount { get; set; }
    }

    public class CategoryCardViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public string Description { get; set; }
        public string IconClass { get; set; }
        public int CourseCount { get; set; }
    }

    public class CourseCardViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
        public string ShortDescription { get; set; }
        public string ThumbnailUrl { get; set; }
        public string InstructorName { get; set; }
        public string InstructorTitle { get; set; }
        public string InstructorAvatarUrl { get; set; }
        public string CategoryName { get; set; }
        public string CategorySlug { get; set; }
        public CourseLevel Level { get; set; }
        public int DurationMinutes { get; set; }
        public int LessonCount { get; set; }
        public bool IsFeatured { get; set; }
    }
}
