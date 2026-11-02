using System.ComponentModel.DataAnnotations;

namespace LMSPortal.Models
{
    public enum CourseLevel
    {
        [Display(Name = "Beginner")]
        Beginner = 0,

        [Display(Name = "Intermediate")]
        Intermediate = 1,

        [Display(Name = "Advanced")]
        Advanced = 2,

        [Display(Name = "All Levels")]
        AllLevels = 3
    }

    public enum EnrollmentStatus
    {
        [Display(Name = "Active")]
        Active = 0,

        [Display(Name = "Completed")]
        Completed = 1,

        [Display(Name = "Dropped")]
        Dropped = 2
    }
}
