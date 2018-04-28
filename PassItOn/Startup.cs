using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(PassItOn.Startup))]
namespace PassItOn
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
