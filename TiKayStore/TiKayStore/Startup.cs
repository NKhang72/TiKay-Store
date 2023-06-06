using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TiKayStore.Startup))]
namespace TiKayStore
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
