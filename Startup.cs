using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(LMSPortal.Startup))]
namespace LMSPortal
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
