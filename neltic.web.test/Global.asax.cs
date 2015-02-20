namespace neltic.web.test
{
    using neltic.environ;

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Security;
    using System.Web.SessionState;

    public class Global : System.Web.HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Label.GetScriptPath()))
            {
                Label.SetScriptPath(Environ.ScriptPath);
                Label.SetAbsoluteScriptPath(Server.MapPath(Environ.ScriptPath));
            }
            Label.Reload(Environ.SchoolContext, Environ.SchoolContextProvider);
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}