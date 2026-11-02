using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using LMSPortal.Security;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace LMSPortal.Models
{
    public static class DatabaseSeeder
    {
        public static void Seed(ApplicationDbContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            var userStore = new UserStore<ApplicationUser>(context);
            var userManager = new UserManager<ApplicationUser>(userStore);

            var roleStore = new RoleStore<IdentityRole>(context);
            var roleManager = new RoleManager<IdentityRole>(roleStore);

            SeedRoles(roleManager);
            string adminId = SeedAdminUser(userManager);
            string studentId = SeedStudentUser(userManager);

            SeedCategoriesAndCourses(context);
            SeedSampleEnrollments(context, studentId);
        }

        public static void SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            string[] roles = { "Admin", "Student" };
            foreach (var roleName in roles)
            {
                if (!roleManager.RoleExists(roleName))
                {
                    roleManager.Create(new IdentityRole(roleName));
                }
            }
        }

        public static string SeedAdminUser(UserManager<ApplicationUser> userManager)
        {
            string adminEmail = ConfigurationManager.AppSettings["DefaultAdminEmail"] ?? "admin@lmsportal.local";
            string adminPassword = Environment.GetEnvironmentVariable("LMS_ADMIN_PASSWORD")
                                   ?? ConfigurationManager.AppSettings["DefaultAdminPassword"]
                                   ?? "Admin@123456";

            var existingAdmin = userManager.FindByEmail(adminEmail);
            if (existingAdmin != null)
            {
                if (!userManager.CheckPassword(existingAdmin, adminPassword))
                {
                    existingAdmin.PasswordHash = userManager.PasswordHasher.HashPassword(adminPassword);
                    existingAdmin.SecurityStamp = Guid.NewGuid().ToString();
                    userManager.Update(existingAdmin);
                }
                return existingAdmin.Id;
            }

            var adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                FullName = "System Administrator",
                EmailConfirmed = true,
                CreatedDate = DateTime.UtcNow,
                Bio = "Platform Lead & LMS Administrator"
            };

            var result = userManager.Create(adminUser, adminPassword);
            if (result.Succeeded)
            {
                userManager.AddToRole(adminUser.Id, "Admin");
                return adminUser.Id;
            }

            return null;
        }

        public static string SeedStudentUser(UserManager<ApplicationUser> userManager)
        {
            string studentEmail = "alex@lmsportal.local";
            var existingStudent = userManager.FindByEmail(studentEmail);
            if (existingStudent != null)
            {
                return existingStudent.Id;
            }

            string studentPassword = Environment.GetEnvironmentVariable("LMS_STUDENT_PASSWORD");
            if (string.IsNullOrWhiteSpace(studentPassword))
            {
                studentPassword = "Student@123456";
            }

            var studentUser = new ApplicationUser
            {
                UserName = studentEmail,
                Email = studentEmail,
                FullName = "Alex Mercer",
                EmailConfirmed = true,
                CreatedDate = DateTime.UtcNow,
                Bio = "Lifelong learner interested in Full-Stack Web Development, Data Science, and Cloud Architecture."
            };

            var result = userManager.Create(studentUser, studentPassword);
            if (result.Succeeded)
            {
                userManager.AddToRole(studentUser.Id, "Student");
                return studentUser.Id;
            }

            return null;
        }

        public static void SeedCategoriesAndCourses(ApplicationDbContext context)
        {
            if (context.Categories.Any()) return;

            // 1. Categories
            var catWeb = new Category
            {
                Name = "Web Development",
                Slug = "web-development",
                Description = "Master modern web development from foundational HTML/CSS to advanced full-stack architectures.",
                IconClass = "bi-code-slash",
                DisplayOrder = 1,
                IsActive = true
            };

            var catData = new Category
            {
                Name = "Data Science & AI",
                Slug = "data-science",
                Description = "Analyze datasets, build machine learning models, and master artificial intelligence algorithms.",
                IconClass = "bi-graph-up-arrow",
                DisplayOrder = 2,
                IsActive = true
            };

            var catCloud = new Category
            {
                Name = "Cloud & DevOps",
                Slug = "cloud-devops",
                Description = "Deploy, scale, and automate resilient cloud infrastructure using Docker, Kubernetes, and CI/CD.",
                IconClass = "bi-cloud-check",
                DisplayOrder = 3,
                IsActive = true
            };

            var catDesign = new Category
            {
                Name = "UI/UX Design",
                Slug = "ui-ux-design",
                Description = "Craft modern user experiences, high-fidelity prototypes, design systems, and responsive layouts.",
                IconClass = "bi-palette",
                DisplayOrder = 4,
                IsActive = true
            };

            context.Categories.AddRange(new[] { catWeb, catData, catCloud, catDesign });
            context.SaveChanges();

            // 2. Courses with Modules and Lessons
            var courses = new List<Course>
            {
                // Course 1
                new Course
                {
                    CategoryId = catWeb.Id,
                    Title = "Modern Full-Stack Web Development with C# and React",
                    Slug = "modern-fullstack-csharp-react",
                    ShortDescription = "Build scalable web applications from front to back with modern C#, ASP.NET, and React.",
                    FullDescription = "Comprehensive curriculum guiding you through enterprise architecture, RESTful API design, database modeling, and interactive frontend development.",
                    InstructorName = "Dr. Sarah Jenkins",
                    InstructorTitle = "Principal Software Architect",
                    InstructorBio = "15+ years architecting enterprise distributed systems and cloud platforms.",
                    InstructorAvatarUrl = "https://images.unsplash.com/photo-1573496359142-b8d87734a5a2?w=150&auto=format&fit=crop&q=80",
                    ThumbnailUrl = "https://images.unsplash.com/photo-1517694712202-14dd9538aa97?w=600&auto=format&fit=crop&q=80",
                    Level = CourseLevel.Intermediate,
                    DurationMinutes = 360,
                    IsFeatured = true,
                    IsPublished = true,
                    IsArchived = false,
                    CreatedDate = DateTime.UtcNow.AddMonths(-2),
                    Modules = new List<Module>
                    {
                        new Module
                        {
                            Title = "1.0 Architecture Foundations & Solution Setup",
                            Description = "Project structure, layers, and configuration best practices.",
                            OrderIndex = 1,
                            Lessons = new List<Lesson>
                            {
                                new Lesson
                                {
                                    Title = "1.1 Introduction to Modern Full-Stack Patterns",
                                    Content = "Welcome to the course! Learn how the client and server communicate via decoupled APIs and modern component hierarchies.",
                                    VideoUrl = "https://www.youtube.com/embed/dQw4w9WgXcQ",
                                    DurationMinutes = 15,
                                    OrderIndex = 1,
                                    IsFreePreview = true
                                },
                                new Lesson
                                {
                                    Title = "1.2 Project Scaffolding & Solution Layout",
                                    Content = "Organizing your solution with clear boundaries: Domain, Data, Services, and Presentation.",
                                    VideoUrl = "https://www.youtube.com/embed/dQw4w9WgXcQ",
                                    DurationMinutes = 25,
                                    OrderIndex = 2,
                                    IsFreePreview = false
                                }
                            }
                        },
                        new Module
                        {
                            Title = "2.0 Entity Framework Core & Data Modeling",
                            Description = "Relational persistence, migrations, and performance optimizations.",
                            OrderIndex = 2,
                            Lessons = new List<Lesson>
                            {
                                new Lesson
                                {
                                    Title = "2.1 Code First Modeling & Constraints",
                                    Content = "Configuring tables, foreign keys, unique composite indexes, and cascade behaviors.",
                                    VideoUrl = "https://www.youtube.com/embed/dQw4w9WgXcQ",
                                    DurationMinutes = 30,
                                    OrderIndex = 1,
                                    IsFreePreview = false
                                },
                                new Lesson
                                {
                                    Title = "2.2 Query Optimization with AsNoTracking & Projections",
                                    Content = "Techniques to eliminate N+1 queries and maximize throughput on high-traffic endpoints.",
                                    VideoUrl = "https://www.youtube.com/embed/dQw4w9WgXcQ",
                                    DurationMinutes = 35,
                                    OrderIndex = 2,
                                    IsFreePreview = false
                                }
                            }
                        },
                        new Module
                        {
                            Title = "3.0 React Single-Page Application Integration",
                            Description = "Component design, state management, and real-time API consumption.",
                            OrderIndex = 3,
                            Lessons = new List<Lesson>
                            {
                                new Lesson
                                {
                                    Title = "3.1 Component Architecture & State Hooks",
                                    Content = "Building predictable component trees with custom hooks and TypeScript interfaces.",
                                    VideoUrl = "https://www.youtube.com/embed/dQw4w9WgXcQ",
                                    DurationMinutes = 40,
                                    OrderIndex = 1,
                                    IsFreePreview = false
                                },
                                new Lesson
                                {
                                    Title = "3.2 Authentication Flows with JWT & Cookie Tokens",
                                    Content = "Managing session tokens securely without exposing secrets to client-side scripts.",
                                    VideoUrl = "https://www.youtube.com/embed/dQw4w9WgXcQ",
                                    DurationMinutes = 45,
                                    OrderIndex = 2,
                                    IsFreePreview = false
                                }
                            }
                        }
                    }
                },

                // Course 2
                new Course
                {
                    CategoryId = catData.Id,
                    Title = "Python for Data Science and Machine Learning Bootcamp",
                    Slug = "python-data-science-machine-learning",
                    ShortDescription = "Analyze data, create stunning visualizations, and build intelligent machine learning algorithms.",
                    FullDescription = "From exploratory data analysis with Pandas and NumPy to scikit-learn models, this course teaches practical, real-world data science workflows.",
                    InstructorName = "Marcus Vance",
                    InstructorTitle = "Lead Data Scientist",
                    InstructorBio = "Former researcher at AI Labs, passionate about machine learning education.",
                    InstructorAvatarUrl = "https://images.unsplash.com/photo-1507003211169-0a1dd7228f2d?w=150&auto=format&fit=crop&q=80",
                    ThumbnailUrl = "https://images.unsplash.com/photo-1551288049-bebda4e38f71?w=600&auto=format&fit=crop&q=80",
                    Level = CourseLevel.Beginner,
                    DurationMinutes = 300,
                    IsFeatured = true,
                    IsPublished = true,
                    IsArchived = false,
                    CreatedDate = DateTime.UtcNow.AddMonths(-1),
                    Modules = new List<Module>
                    {
                        new Module
                        {
                            Title = "1.0 Data Wrangling with Pandas & NumPy",
                            Description = "Data manipulation, cleaning, and preparation.",
                            OrderIndex = 1,
                            Lessons = new List<Lesson>
                            {
                                new Lesson
                                {
                                    Title = "1.1 Introduction to DataFrames & Series",
                                    Content = "Understanding tabular data structures and indexing techniques in Pandas.",
                                    VideoUrl = "https://www.youtube.com/embed/dQw4w9WgXcQ",
                                    DurationMinutes = 20,
                                    OrderIndex = 1,
                                    IsFreePreview = true
                                },
                                new Lesson
                                {
                                    Title = "1.2 Handling Missing Values & Data Transformation",
                                    Content = "Imputing nulls, one-hot encoding, and standardizing numerical distributions.",
                                    VideoUrl = "https://www.youtube.com/embed/dQw4w9WgXcQ",
                                    DurationMinutes = 30,
                                    OrderIndex = 2,
                                    IsFreePreview = false
                                }
                            }
                        },
                        new Module
                        {
                            Title = "2.0 Supervised Machine Learning Algorithms",
                            Description = "Regression, classification, and model evaluation.",
                            OrderIndex = 2,
                            Lessons = new List<Lesson>
                            {
                                new Lesson
                                {
                                    Title = "2.1 Linear & Logistic Regression Explained",
                                    Content = "Mathematical intuition and practical implementation using scikit-learn.",
                                    VideoUrl = "https://www.youtube.com/embed/dQw4w9WgXcQ",
                                    DurationMinutes = 35,
                                    OrderIndex = 1,
                                    IsFreePreview = false
                                },
                                new Lesson
                                {
                                    Title = "2.2 Random Forests & Gradient Boosting",
                                    Content = "Ensemble learning methods and hyperparameter tuning with GridSearchCV.",
                                    VideoUrl = "https://www.youtube.com/embed/dQw4w9WgXcQ",
                                    DurationMinutes = 40,
                                    OrderIndex = 2,
                                    IsFreePreview = false
                                }
                            }
                        }
                    }
                },

                // Course 3
                new Course
                {
                    CategoryId = catCloud.Id,
                    Title = "Cloud Architecture and Kubernetes for Production",
                    Slug = "cloud-architecture-kubernetes-production",
                    ShortDescription = "Design resilient cloud infrastructure and orchestrate microservices at scale with Kubernetes.",
                    FullDescription = "Master container orchestration, automated ingress routing, zero-downtime rolling updates, and monitoring in cloud environments.",
                    InstructorName = "Elena Rostova",
                    InstructorTitle = "Cloud Solutions Architect",
                    InstructorBio = "Kubernetes certified engineer specialized in multi-region infrastructure.",
                    InstructorAvatarUrl = "https://images.unsplash.com/photo-1580489944761-15a19d654956?w=150&auto=format&fit=crop&q=80",
                    ThumbnailUrl = "https://images.unsplash.com/photo-1451187580459-43490279c0fa?w=600&auto=format&fit=crop&q=80",
                    Level = CourseLevel.Advanced,
                    DurationMinutes = 240,
                    IsFeatured = true,
                    IsPublished = true,
                    IsArchived = false,
                    CreatedDate = DateTime.UtcNow.AddDays(-20),
                    Modules = new List<Module>
                    {
                        new Module
                        {
                            Title = "1.0 Containerization & Docker Essentials",
                            Description = "Multi-stage builds, caching layers, and minimal production images.",
                            OrderIndex = 1,
                            Lessons = new List<Lesson>
                            {
                                new Lesson
                                {
                                    Title = "1.1 Docker Multi-Stage Optimization",
                                    Content = "Reducing image size by 90% using Alpine bases and compiled artifacts.",
                                    VideoUrl = "https://www.youtube.com/embed/dQw4w9WgXcQ",
                                    DurationMinutes = 20,
                                    OrderIndex = 1,
                                    IsFreePreview = true
                                },
                                new Lesson
                                {
                                    Title = "1.2 Container Networking & Volumes",
                                    Content = "Configuring bridge networks, persistent mounts, and secrets injection.",
                                    VideoUrl = "https://www.youtube.com/embed/dQw4w9WgXcQ",
                                    DurationMinutes = 25,
                                    OrderIndex = 2,
                                    IsFreePreview = false
                                }
                            }
                        },
                        new Module
                        {
                            Title = "2.0 Kubernetes Deployments & Ingress",
                            Description = "Pods, Services, Deployments, and Helm charts.",
                            OrderIndex = 2,
                            Lessons = new List<Lesson>
                            {
                                new Lesson
                                {
                                    Title = "2.1 Zero-Downtime Rolling Updates",
                                    Content = "Configuring readiness and liveness probes to prevent traffic blackholes.",
                                    VideoUrl = "https://www.youtube.com/embed/dQw4w9WgXcQ",
                                    DurationMinutes = 35,
                                    OrderIndex = 1,
                                    IsFreePreview = false
                                }
                            }
                        }
                    }
                },

                // Course 4
                new Course
                {
                    CategoryId = catDesign.Id,
                    Title = "UI/UX Masterclass: Designing High-Converting Digital Products",
                    Slug = "ui-ux-masterclass-design-systems",
                    ShortDescription = "Create design systems, intuitive user flows, and accessible web interfaces that users love.",
                    FullDescription = "Learn design theory, color psychology, typography scale, responsive layout grids, and interactive Figma prototypes.",
                    InstructorName = "Chloe Bennett",
                    InstructorTitle = "Lead Product Designer",
                    InstructorBio = "Crafting digital experiences for Fortune 500 brands and high-growth startups.",
                    InstructorAvatarUrl = "https://images.unsplash.com/photo-1534528741775-53994a69daeb?w=150&auto=format&fit=crop&q=80",
                    ThumbnailUrl = "https://images.unsplash.com/photo-1507238691740-187a5b1d37b8?w=600&auto=format&fit=crop&q=80",
                    Level = CourseLevel.AllLevels,
                    DurationMinutes = 180,
                    IsFeatured = false,
                    IsPublished = true,
                    IsArchived = false,
                    CreatedDate = DateTime.UtcNow.AddDays(-15),
                    Modules = new List<Module>
                    {
                        new Module
                        {
                            Title = "1.0 Design Systems & Foundations",
                            Description = "Tokens, scales, and component libraries.",
                            OrderIndex = 1,
                            Lessons = new List<Lesson>
                            {
                                new Lesson
                                {
                                    Title = "1.1 Typography Hierarchy & Spacing Systems",
                                    Content = "Establishing consistent 8pt grids, optical line heights, and font weights.",
                                    VideoUrl = "https://www.youtube.com/embed/dQw4w9WgXcQ",
                                    DurationMinutes = 18,
                                    OrderIndex = 1,
                                    IsFreePreview = true
                                },
                                new Lesson
                                {
                                    Title = "1.2 Accessible Color Palettes & Contrast Ratios",
                                    Content = "Ensuring WCAG AAA compliance across dark and light interface themes.",
                                    VideoUrl = "https://www.youtube.com/embed/dQw4w9WgXcQ",
                                    DurationMinutes = 22,
                                    OrderIndex = 2,
                                    IsFreePreview = false
                                }
                            }
                        }
                    }
                },

                // Course 5
                new Course
                {
                    CategoryId = catWeb.Id,
                    Title = "Advanced ASP.NET MVC and Enterprise Architecture",
                    Slug = "advanced-aspnet-mvc-enterprise",
                    ShortDescription = "Deep dive into ASP.NET MVC internals, custom filters, security, and scalability.",
                    FullDescription = "Learn how the MVC pipeline really works under the hood. Master action filters, OWIN middleware, dependency injection, and advanced caching.",
                    InstructorName = "Dr. Sarah Jenkins",
                    InstructorTitle = "Principal Software Architect",
                    InstructorBio = "15+ years architecting enterprise distributed systems and cloud platforms.",
                    InstructorAvatarUrl = "https://images.unsplash.com/photo-1573496359142-b8d87734a5a2?w=150&auto=format&fit=crop&q=80",
                    ThumbnailUrl = "https://images.unsplash.com/photo-1461749280684-dccba630e2f6?w=600&auto=format&fit=crop&q=80",
                    Level = CourseLevel.Advanced,
                    DurationMinutes = 210,
                    IsFeatured = false,
                    IsPublished = true,
                    IsArchived = false,
                    CreatedDate = DateTime.UtcNow.AddDays(-10),
                    Modules = new List<Module>
                    {
                        new Module
                        {
                            Title = "1.0 MVC Pipeline Internals & Action Filters",
                            Description = "Lifecycle of an HTTP request from IIS to Razor rendering.",
                            OrderIndex = 1,
                            Lessons = new List<Lesson>
                            {
                                new Lesson
                                {
                                    Title = "1.1 Routing & Action Invocation Deep Dive",
                                    Content = "Understanding RouteData, VirtualPathProviders, and controller factory execution.",
                                    VideoUrl = "https://www.youtube.com/embed/dQw4w9WgXcQ",
                                    DurationMinutes = 25,
                                    OrderIndex = 1,
                                    IsFreePreview = true
                                }
                            }
                        }
                    }
                },

                // Course 6
                new Course
                {
                    CategoryId = catData.Id,
                    Title = "Deep Learning Fundamentals: Neural Networks & Computer Vision",
                    Slug = "deep-learning-neural-networks-vision",
                    ShortDescription = "Build deep neural networks and convolutional networks from scratch for image recognition.",
                    FullDescription = "Practical hands-on guide to backpropagation, activation functions, CNNs, transfer learning, and deploying PyTorch models.",
                    InstructorName = "Marcus Vance",
                    InstructorTitle = "Lead Data Scientist",
                    InstructorBio = "Former researcher at AI Labs, passionate about machine learning education.",
                    InstructorAvatarUrl = "https://images.unsplash.com/photo-1507003211169-0a1dd7228f2d?w=150&auto=format&fit=crop&q=80",
                    ThumbnailUrl = "https://images.unsplash.com/photo-1620712943543-bcc4688e7485?w=600&auto=format&fit=crop&q=80",
                    Level = CourseLevel.Advanced,
                    DurationMinutes = 260,
                    IsFeatured = false,
                    IsPublished = true,
                    IsArchived = false,
                    CreatedDate = DateTime.UtcNow.AddDays(-5),
                    Modules = new List<Module>
                    {
                        new Module
                        {
                            Title = "1.0 Artificial Neural Networks Architecture",
                            Description = "Feedforward networks, gradient descent, and backprop.",
                            OrderIndex = 1,
                            Lessons = new List<Lesson>
                            {
                                new Lesson
                                {
                                    Title = "1.1 Perceptrons & Multi-Layer Perceptrons",
                                    Content = "From simple linear decision boundaries to universal function approximators.",
                                    VideoUrl = "https://www.youtube.com/embed/dQw4w9WgXcQ",
                                    DurationMinutes = 30,
                                    OrderIndex = 1,
                                    IsFreePreview = true
                                }
                            }
                        }
                    }
                }
            };

            context.Courses.AddRange(courses);
            context.SaveChanges();
        }

        public static void SeedSampleEnrollments(ApplicationDbContext context, string studentUserId)
        {
            if (string.IsNullOrEmpty(studentUserId)) return;
            if (context.Enrollments.Any(e => e.UserId == studentUserId)) return;

            var courses = context.Courses.Include("Modules.Lessons").Take(3).ToList();
            if (!courses.Any()) return;

            // Course 1: Partial progress (e.g. 50% completed)
            var c1 = courses[0];
            var allLessonsC1 = c1.Modules.SelectMany(m => m.Lessons).OrderBy(l => l.Module.OrderIndex).ThenBy(l => l.OrderIndex).ToList();
            int c1CompletedCount = Math.Max(1, allLessonsC1.Count / 2);

            for (int i = 0; i < c1CompletedCount; i++)
            {
                context.LessonCompletions.Add(new LessonCompletion
                {
                    UserId = studentUserId,
                    LessonId = allLessonsC1[i].Id,
                    CompletedDate = DateTime.UtcNow.AddDays(-i - 1)
                });
            }

            int c1Progress = allLessonsC1.Count > 0 ? (int)Math.Round((double)c1CompletedCount * 100 / allLessonsC1.Count) : 0;
            context.Enrollments.Add(new Enrollment
            {
                UserId = studentUserId,
                CourseId = c1.Id,
                EnrolledDate = DateTime.UtcNow.AddDays(-7),
                LastAccessedDate = DateTime.UtcNow.AddHours(-3),
                Status = c1Progress >= 100 ? EnrollmentStatus.Completed : EnrollmentStatus.Active,
                CompletionPercentage = c1Progress
            });

            // Course 2: 100% completed
            if (courses.Count > 1)
            {
                var c2 = courses[1];
                var allLessonsC2 = c2.Modules.SelectMany(m => m.Lessons).ToList();
                foreach (var lesson in allLessonsC2)
                {
                    context.LessonCompletions.Add(new LessonCompletion
                    {
                        UserId = studentUserId,
                        LessonId = lesson.Id,
                        CompletedDate = DateTime.UtcNow.AddDays(-2)
                    });
                }

                context.Enrollments.Add(new Enrollment
                {
                    UserId = studentUserId,
                    CourseId = c2.Id,
                    EnrolledDate = DateTime.UtcNow.AddDays(-14),
                    LastAccessedDate = DateTime.UtcNow.AddDays(-1),
                    Status = EnrollmentStatus.Completed,
                    CompletionPercentage = 100
                });
            }

            // Course 3: 0% progress (newly enrolled)
            if (courses.Count > 2)
            {
                var c3 = courses[2];
                context.Enrollments.Add(new Enrollment
                {
                    UserId = studentUserId,
                    CourseId = c3.Id,
                    EnrolledDate = DateTime.UtcNow.AddHours(-12),
                    LastAccessedDate = DateTime.UtcNow.AddHours(-12),
                    Status = EnrollmentStatus.Active,
                    CompletionPercentage = 0
                });
            }

            context.SaveChanges();
        }
    }
}
