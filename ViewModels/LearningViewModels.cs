using System.Collections.Generic;

namespace LMSPortal.ViewModels
{
    public class LearningPlayerViewModel
    {
        public LearningPlayerViewModel()
        {
            Modules = new List<LearningModuleNavViewModel>();
        }

        public int CourseId { get; set; }
        public string CourseTitle { get; set; }
        public string CourseSlug { get; set; }

        public int CurrentLessonId { get; set; }
        public string CurrentLessonTitle { get; set; }
        public string CurrentLessonContent { get; set; }
        public string CurrentLessonVideoUrl { get; set; }
        public int CurrentLessonDuration { get; set; }
        public bool IsCurrentLessonCompleted { get; set; }
        public bool IsFreePreview { get; set; }
        public bool IsEnrolled { get; set; }

        public int CurrentModuleId { get; set; }
        public string CurrentModuleTitle { get; set; }

        public int? PreviousLessonId { get; set; }
        public int? NextLessonId { get; set; }

        public int CourseProgressPercentage { get; set; }
        public int CompletedLessonsCount { get; set; }
        public int TotalLessonsCount { get; set; }

        public List<LearningModuleNavViewModel> Modules { get; set; }
    }

    public class LearningModuleNavViewModel
    {
        public LearningModuleNavViewModel()
        {
            Lessons = new List<LearningLessonNavViewModel>();
        }

        public int ModuleId { get; set; }
        public string Title { get; set; }
        public int OrderIndex { get; set; }
        public List<LearningLessonNavViewModel> Lessons { get; set; }
    }

    public class LearningLessonNavViewModel
    {
        public int LessonId { get; set; }
        public string Title { get; set; }
        public int OrderIndex { get; set; }
        public int DurationMinutes { get; set; }
        public bool IsFreePreview { get; set; }
        public bool IsCurrent { get; set; }
        public bool IsCompleted { get; set; }
    }
}
