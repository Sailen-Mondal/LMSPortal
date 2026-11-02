using System.Web.Mvc;
using System.Web.Routing;

namespace LMSPortal
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            // SEO-friendly course details route: /course/{slug}
            routes.MapRoute(
                name: "CourseDetails",
                url: "course/{slug}",
                defaults: new { controller = "Course", action = "Details" }
            );

            // SEO-friendly course catalog route: /courses
            routes.MapRoute(
                name: "CourseCatalog",
                url: "courses",
                defaults: new { controller = "Course", action = "Index" }
            );

            // Default route
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "LMSPortal.Controllers" }
            );
        }
    }
}
