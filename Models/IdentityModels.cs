using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace LMSPortal.Models
{
    // ApplicationUser extends IdentityUser with custom LMS attributes
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser()
        {
            CreatedDate = DateTime.UtcNow;
            Enrollments = new HashSet<Enrollment>();
            LessonCompletions = new HashSet<LessonCompletion>();
        }

        [Required]
        [StringLength(100)]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [StringLength(255)]
        [Display(Name = "Avatar URL")]
        public string AvatarUrl { get; set; }

        [StringLength(500)]
        [Display(Name = "Bio")]
        public string Bio { get; set; }

        [Display(Name = "Member Since")]
        public DateTime CreatedDate { get; set; }

        public virtual ICollection<Enrollment> Enrollments { get; set; }

        public virtual ICollection<LessonCompletion> LessonCompletions { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            
            // Add custom user claims here if needed
            userIdentity.AddClaim(new Claim("FullName", this.FullName ?? string.Empty));
            
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        static ApplicationDbContext()
        {
            // Code-level reference anchor to ensure EntityFramework.SqlServer.dll is copied and loaded
            var _ = System.Data.Entity.SqlServer.SqlProviderServices.Instance;
        }

        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
            Configuration.LazyLoadingEnabled = true;
            Configuration.ProxyCreationEnabled = true;
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Module> Modules { get; set; }
        public DbSet<Lesson> Lessons { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<LessonCompletion> LessonCompletions { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Prevent cascading delete of enrollments when a course or user is removed
            // to protect historical learning and enrollment records
            modelBuilder.Entity<Course>()
                .HasMany(c => c.Enrollments)
                .WithRequired(e => e.Course)
                .HasForeignKey(e => e.CourseId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ApplicationUser>()
                .HasMany(u => u.Enrollments)
                .WithRequired(e => e.User)
                .HasForeignKey(e => e.UserId)
                .WillCascadeOnDelete(false);

            // Deleting a course cascades to its curriculum modules and lessons
            modelBuilder.Entity<Course>()
                .HasMany(c => c.Modules)
                .WithRequired(m => m.Course)
                .HasForeignKey(m => m.CourseId)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<Module>()
                .HasMany(m => m.Lessons)
                .WithRequired(l => l.Module)
                .HasForeignKey(l => l.ModuleId)
                .WillCascadeOnDelete(true);
        }
    }
}
