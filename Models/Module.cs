using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LMSPortal.Models
{
    public class Module
    {
        public Module()
        {
            Lessons = new HashSet<Lesson>();
            OrderIndex = 1;
        }

        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Course")]
        public int CourseId { get; set; }

        [Required]
        [StringLength(150)]
        [Display(Name = "Module Title")]
        public string Title { get; set; }

        [StringLength(500)]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [Display(Name = "Order Index")]
        public int OrderIndex { get; set; }

        [ForeignKey("CourseId")]
        public virtual Course Course { get; set; }

        public virtual ICollection<Lesson> Lessons { get; set; }
    }
}
