using System;
using System.Diagnostics;
using Hangfire;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(DailyAww.Startup))]
namespace DailyAww
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            GlobalConfiguration.Configuration
               .UseSqlServerStorage(
                System.Web.Configuration.WebConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()
                );

            app.UseHangfireDashboard();
            app.UseHangfireServer();
            
            ConfigureAuth(app);
        }
    }
}
