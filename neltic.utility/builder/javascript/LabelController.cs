namespace neltic.utility.builder.javascript
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.Remoting;
    using System.Text;
    using System.Xml.Linq;
    using System.Linq;
    using System.Data.Common;
    using System.Data.Linq;

    public sealed class LabelController : JavaScriptBuilder
    {
        private static readonly LabelController instance = new LabelController();
        public static LabelController Instance { get { return instance; } }

        public StringBuilder Build(string className, string xmlPath)
        {
            XDocument doc = XDocument.Load(xmlPath);
            return Build(className, doc.Root.Elements("add"));
        }
        public StringBuilder Build(string className, IEnumerable<XElement> elements)
        {
            return Build(className, GetElements(elements));
        }
        public StringBuilder Build(string className, string connectionString, string provider)
        {
            DbProviderFactory factory = DbProviderFactories.GetFactory(provider);
            using (DbConnection connection = factory.CreateConnection())
            {
                connection.ConnectionString = connectionString;
                DataContext context = new DataContext(connection);
                return Build(className, context.ExecuteQuery<KeyValue<string, string>>("select " + __SQL_QUOTE_START__ + "Key" + __SQL_QUOTE_END__ + ", " + __SQL_QUOTE_START__ + "Value" + __SQL_QUOTE_END__ + " from " + __SQL_QUOTE_START__ + "" + className + "" + __SQL_QUOTE_END__ + "").ToList());
            }
        }
        public StringBuilder Build(string className, List<KeyValue<string, string>> elements)
        {
            builder = new StringBuilder();
            indent = string.Empty;
            int count = elements.Count();
            string ontl = className.ToLower();

            StartObject(className);
            AddIndent();
            foreach (var element in elements)
            {
                AddKeyString(element.Key, element.Value, (--count > 0 ? "," : ""));
            }
            RemoveIndent();
            CloseBracket();
            AddFunction("apply" + className, "$context", "$('*', $context).filter(function () { return $(this).data('" + ontl + "') !== undefined; }).each(function () { $(this).html(" + className + "[$(this).data('" + ontl + "')]); });");
            AddOnReadyFunction("apply" + className + "($('body'));");
            return builder;
        }

    }
}
