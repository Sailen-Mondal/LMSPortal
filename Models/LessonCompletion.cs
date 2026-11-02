using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LMSPortal.Models
{
    public class LessonCompletion
    {
        public LessonCompletion()
        {
            CompletedDate = DateTime.UtcNow;
        }

        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(128)]
        [Index("IX_LessonCompletion_User_Lesson", 1, IsUnique = true)]
        public string UserId { get; set; }

        [Required]
        [Index("IX_LessonCompletion_User_Lesson", 2, IsUnique = true)]
        public int LessonId { get; set; }

        public DateTime CompletedDate { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }

        [ForeignKey("LessonId")]
        public virtual Lesson Lesson { get; set; }
    }
}
