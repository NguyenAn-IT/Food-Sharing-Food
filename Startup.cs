using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Food_Sharing_Food.Startup))]
namespace Food_Sharing_Food
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
