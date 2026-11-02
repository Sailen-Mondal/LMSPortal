using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LMSPortal.Models
{
    public class Course
    {
        public Course()
        {
            Modules = new HashSet<Module>();
            Enrollments = new HashSet<Enrollment>();
            CreatedDate = DateTime.UtcNow;
            IsPublished = true;
            IsArchived = false;
            Level = CourseLevel.Beginner;
        }

        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        [Required]
        [StringLength(220)]
        [Index("IX_Course_Slug", IsUnique = true)]
        public string Slug { get; set; }

        [Required]
        [StringLength(500)]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Short Description")]
        public string ShortDescription { get; set; }

        [DataType(DataType.MultilineText)]
        [Display(Name = "Full Description")]
        public string FullDescription { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Instructor Name")]
        public string InstructorName { get; set; }

        [StringLength(150)]
        [Display(Name = "Instructor Title")]
        public string InstructorTitle { get; set; }

        [StringLength(500)]
        [Display(Name = "Instructor Bio")]
        public string InstructorBio { get; set; }

        [StringLength(255)]
        [Display(Name = "Instructor Avatar URL")]
        public string InstructorAvatarUrl { get; set; }

        [StringLength(255)]
        [Display(Name = "Thumbnail URL")]
        public string ThumbnailUrl { get; set; }

        [Display(Name = "Skill Level")]
        public CourseLevel Level { get; set; }

        [Display(Name = "Duration (Minutes)")]
        public int DurationMinutes { get; set; }

        [Display(Name = "Featured Course")]
        public bool IsFeatured { get; set; }

        [Display(Name = "Published")]
        public bool IsPublished { get; set; }

        [Display(Name = "Archived")]
        public bool IsArchived { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }

        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }

        public virtual ICollection<Module> Modules { get; set; }

        public virtual ICollection<Enrollment> Enrollments { get; set; }
    }
}
