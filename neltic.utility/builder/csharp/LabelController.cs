namespace neltic.utility.builder.csharp
{
    using neltic.utility.Properties;

    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data.Common;
    using System.Data.Linq;
    using System.Linq;
    using System.Runtime.Remoting;
    using System.Text;
    using System.Xml.Linq;

    public sealed class LabelController : CSharpBuilder
    {
        private static readonly LabelController instance = new LabelController();
        public static LabelController Instance { get { return instance; } }

        // modo en que se esta cargando la información
        // -1 : no tiene forma de recargar la información
        //  0 : xml
        //  1 : base de datos        
        public StringBuilder Build(string _namespace, string className, string xmlPath)
        {
            XDocument doc = XDocument.Load(xmlPath);
            return Build(_namespace, className, doc.Root.Elements("add"));
        }
        public StringBuilder Build(string _namespace, string className, IEnumerable<XElement> elements)
        {
            return Build(_namespace, className, GetElements(elements));
        }
        public StringBuilder Build(string _namespace, string className, string connectionString, string provider)
        {
            DbProviderFactory factory = DbProviderFactories.GetFactory(provider);
            using (DbConnection connection = factory.CreateConnection())
            {
                connection.ConnectionString = connectionString;
                DataContext context = new DataContext(connection);
                return Build(_namespace, className, context.ExecuteQuery<KeyValue<string, string>>("select " + __SQL_QUOTE_START__ + "Key" + __SQL_QUOTE_END__ + ", " + __SQL_QUOTE_START__ + "Value" + __SQL_QUOTE_END__ + " from " + __SQL_QUOTE_START__ + "" + className + "" + __SQL_QUOTE_END__ + "").ToList());
            }
        }
        public StringBuilder Build(string _namespace, string className, List<KeyValue<string, string>> elements)
        {
            builder = new StringBuilder();
            indent = string.Empty;

            AddNamespace(_namespace);
            AddIndent();
            AddUsing("neltic.utility.builder.javascript");
            AddEmptyLine();
            AddUsing("System");
            AddUsing("System.Collections");
            AddUsing("System.Collections.Generic");
            AddUsing("System.ComponentModel");
            AddUsing("System.Data.Common");
            AddUsing("System.Data.Linq");
            AddUsing("System.IO");
            AddUsing("System.Linq");
            AddUsing("System.Reflection");
            AddUsing("System.Xml.Linq");
            AddEmptyLine();
            AddStaticClass(className);
            AddIndent();
            foreach (var element in elements)
            {
                AddStaticProperty("string", element.Key, element.Value);
            }
            AddEmptyLine();
            AddText(ReplaceName(className, Resources.LabelControllerSource));
            AddEmptyLine();
            RemoveIndent();
            CloseBracket();
            RemoveIndent();
            CloseBracket();
            return builder;
        }

        private string ReplaceName(string className, string text)
        {
            return text.Replace("__CLASS_NAME__", className).Replace("__CLASS_LOWER_NAME__", className.ToLower()).Replace("__CLASS_QUOTE_START__", __SQL_QUOTE_START__).Replace("__CLASS_QUOTE_END__", __SQL_QUOTE_END__);
        }

    }
}
