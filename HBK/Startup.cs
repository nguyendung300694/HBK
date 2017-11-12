using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(HBK.Startup))]
namespace HBK
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
