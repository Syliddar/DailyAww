using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DailyAww.Services
{
    public class AwwthorizationAttribute : AuthorizeAttribute
    {
        private string _key;
        public AwwthorizationAttribute()
        {
            _key = ConfigurationManager.AppSettings["AwwKey"];
        }
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            var key = filterContext.HttpContext.Request.Headers["Awwthorization"];
            if (key != _key)
            {
                filterContext.Result = new HttpUnauthorizedResult("Awwthorization Failure");
            }
        }
    }
}