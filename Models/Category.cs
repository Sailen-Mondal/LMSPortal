using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LMSPortal.Models
{
    public class Category
    {
        public Category()
        {
            Courses = new HashSet<Course>();
            IsActive = true;
        }

        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        [Index("IX_Category_Name", IsUnique = true)]
        public string Name { get; set; }

        [Required]
        [StringLength(120)]
        [Index("IX_Category_Slug", IsUnique = true)]
        public string Slug { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [StringLength(50)]
        public string IconClass { get; set; }

        public int DisplayOrder { get; set; }

        public bool IsActive { get; set; }

        public virtual ICollection<Course> Courses { get; set; }
    }
}
