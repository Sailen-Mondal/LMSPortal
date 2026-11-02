using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LMSPortal.Models
{
    public class Lesson
    {
        public Lesson()
        {
            Completions = new HashSet<LessonCompletion>();
            OrderIndex = 1;
            DurationMinutes = 5;
            IsFreePreview = false;
        }

        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Module")]
        public int ModuleId { get; set; }

        [Required]
        [StringLength(150)]
        [Display(Name = "Lesson Title")]
        public string Title { get; set; }

        [DataType(DataType.MultilineText)]
        [Display(Name = "Lesson Content")]
        public string Content { get; set; }

        [StringLength(500)]
        [Display(Name = "Video URL / Embed Link")]
        public string VideoUrl { get; set; }

        [Display(Name = "Duration (Minutes)")]
        public int DurationMinutes { get; set; }

        [Display(Name = "Order Index")]
        public int OrderIndex { get; set; }

        [Display(Name = "Free Preview")]
        public bool IsFreePreview { get; set; }

        [ForeignKey("ModuleId")]
        public virtual Module Module { get; set; }

        public virtual ICollection<LessonCompletion> Completions { get; set; }
    }
}
