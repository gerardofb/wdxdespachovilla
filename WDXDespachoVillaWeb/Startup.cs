using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WDXDespachoVillaWeb.Startup))]
namespace WDXDespachoVillaWeb
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
