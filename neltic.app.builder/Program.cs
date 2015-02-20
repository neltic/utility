namespace neltic.app.builder
{
    using neltic.utility;
    using neltic.utility.builder.csharp;
    using neltic.environ;

    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data.Common;
    using System.Data.Linq;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using System.Runtime.Remoting;

    using neltic.utility.builder.javascript;


    class Program
    {

        static void Main(string[] args)
        {
            EnvironController.Instance.SetExcludedConnectionStrings(new string[] { "LocalSqlServer", "LocalMySqlServer" });
            var result = EnvironController.Instance.Build("neltic.environ", "Environ", @"C:\deloitte\mx\neltic.utilities\neltic.web.test\Web.config");

        }
    }
}
