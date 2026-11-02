using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LMSPortal.Models
{
    public class Enrollment
    {
        public Enrollment()
        {
            EnrolledDate = DateTime.UtcNow;
            LastAccessedDate = DateTime.UtcNow;
            Status = EnrollmentStatus.Active;
            CompletionPercentage = 0;
        }

        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(128)]
        [Index("IX_Enrollment_User_Course", 1, IsUnique = true)]
        public string UserId { get; set; }

        [Required]
        [Index("IX_Enrollment_User_Course", 2, IsUnique = true)]
        public int CourseId { get; set; }

        public DateTime EnrolledDate { get; set; }

        public DateTime? LastAccessedDate { get; set; }

        public EnrollmentStatus Status { get; set; }

        [Range(0, 100)]
        [Display(Name = "Completion Progress (%)")]
        public int CompletionPercentage { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }

        [ForeignKey("CourseId")]
        public virtual Course Course { get; set; }
    }
}
