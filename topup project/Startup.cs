using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(topup_project.Startup))]
namespace topup_project
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
