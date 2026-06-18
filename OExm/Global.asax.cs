using System;
using System.Web;

namespace OExm
{
    public class Global : System.Web.HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            Application["TotalVisitors"] = 0;
        }
        protected void Application_BeginRequest()
        {
            HttpContext.Current.Response.Cache.SetCacheability(
                HttpCacheability.NoCache);

            HttpContext.Current.Response.Cache.SetNoStore();
        }
    }
}