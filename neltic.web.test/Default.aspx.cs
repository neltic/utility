namespace neltic.web.test
{
    using Neltic = neltic.environ;

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Services;
    using System.Web.UI;
    using System.Web.Hosting;

    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            DisplayDemoData.InnerText = Neltic.Label.Go + " | " + Neltic.Label.NotEditable;

            if (Request.QueryString["reload"] != null && Request.QueryString["reload"].Equals("true", StringComparison.InvariantCultureIgnoreCase))
            {
                if (string.IsNullOrWhiteSpace(Neltic.Label.GetScriptPath()))
                {
                    Neltic.Label.SetScriptPath(Neltic.Environ.ScriptPath);
                    Neltic.Label.SetAbsoluteScriptPath(Server.MapPath(Neltic.Environ.ScriptPath));
                }
                Neltic.Label.Reload(Neltic.Environ.SchoolContext, Neltic.Environ.SchoolContextProvider);
            }
        }

        [WebMethod]
        public static bool Reload()
        {
            Neltic.Label.Reload(Neltic.Environ.SchoolContext, Neltic.Environ.SchoolContextProvider);
            return true;
        }
    }
}