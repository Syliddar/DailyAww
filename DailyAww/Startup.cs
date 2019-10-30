using Microsoft.Owin;
using Owin;
using System.Web.Http;

[assembly: OwinStartup(typeof(DailyAww.Startup))]
namespace DailyAww
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            //GlobalConfiguration.Configuration.sql.UseSqlServerStorage(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());
            ConfigureAuth(app);
        }
    }
}
