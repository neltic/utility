namespace neltic.environ
{
    using neltic.utility.builder.javascript;

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data.Common;
    using System.Data.Linq;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Xml.Linq;

    public static partial class Label
    {
        public static string Apply = "Aplicar";
        public static string Change = "Cambiar";
        public static string ConfirmAction = "¿Desea llevar acabo la acción?";
        public static string ConfirmClear = "¿Desea limpiar todos los campos del formulario?";
        public static string Create = "Crear";
        public static string Go = "Ir";
        public static string Import = "Importar";
        public static string New = "Nuevo";
        public static string Next = "Siguiente";
        public static string NotEditable = "Este registro no es editable";
        public static string PageNotFound = "¡La pagina no se encuentra disponible!";

        private static string __JS_ABSOLUTE_PATH__ = string.Empty;
        private static string __JS_SCRIPT_PATH__ = string.Empty;
        private static string __JS_SCRIPT_NAME__ = string.Empty;
		private static string __SQL_QUOTE_START__ = "`";
		private static string __SQL_QUOTE_END__ = "`";
		 
        public static void SetAbsoluteScriptPath(string absolutePath)
        {
            __JS_ABSOLUTE_PATH__ = absolutePath;
        }

        public static void SetScriptPath(string relativePath)
        {
            __JS_SCRIPT_PATH__ = relativePath;
        }

        public static string GetScriptPath()
        {
            return __JS_SCRIPT_PATH__ + __JS_SCRIPT_NAME__;
        }

		public static void SetSqlQuote(string start, string end)
        {
            __SQL_QUOTE_START__ = start;
			__SQL_QUOTE_END__ = end;
        }

        public static string GetScriptName()
        {
            return "label" + "." + DateTime.Now.ToString("HHmmssyyddMM") + ".js";
        }

        public static void Reload(string xmlPath, bool createScript = true)
        {
            XDocument doc = XDocument.Load(xmlPath);
            UpdateProperties(LabelController.Instance.GetElements(doc.Root.Elements("add")));
            UpdateScript(createScript, xmlPath);
        }

        public static void Reload(string connectionString, string provider, bool createScript = true)
        {
            DbProviderFactory factory = DbProviderFactories.GetFactory(provider);
            using (DbConnection connection = factory.CreateConnection())
            {
                connection.ConnectionString = connectionString;
                DataContext context = new DataContext(connection);
                UpdateProperties(context.ExecuteQuery<KeyValue<string, string>>("select " + __SQL_QUOTE_START__ + "Key" + __SQL_QUOTE_END__ + ", " + __SQL_QUOTE_START__ + "Value" + __SQL_QUOTE_END__ + " from " + __SQL_QUOTE_START__ + "label" + __SQL_QUOTE_END__ + "").ToList());
                UpdateScript(createScript, connectionString, provider);
            }
        }

        public static void UpdateProperties(List<KeyValue<string, string>> elements)
        {
            Type type = typeof(Label);
            var properties = type.GetFields(BindingFlags.Public | BindingFlags.Static);
            for (int i = 0; i < properties.Length; i++)
            {
                var element = (from e in elements
                               where e.Key.Equals(properties[i].Name)
                               select e).FirstOrDefault();
                if (element != null)
                {
                    properties[i].SetValue(type, element.Value);
                }
            }
        }

        private static void UpdateScript(bool createScript, params string[] args)
        {
            if (createScript)
            {
                if (!string.IsNullOrWhiteSpace(__JS_SCRIPT_NAME__))
                {
                    if (File.Exists(__JS_ABSOLUTE_PATH__ + __JS_SCRIPT_NAME__))
                    {
                        File.Delete(__JS_ABSOLUTE_PATH__ + __JS_SCRIPT_NAME__);
                    }
                }
                __JS_SCRIPT_NAME__ = GetScriptName();
                string filePath = __JS_ABSOLUTE_PATH__ + __JS_SCRIPT_NAME__;
                string text = string.Empty;
                switch (args.Length)
                {
                    case 1:
                        text = LabelController.Instance.Build("Label", args[0]).ToString();
                        break;
                    case 2:
					    LabelController.Instance.SetSqlQuote(__SQL_QUOTE_START__, __SQL_QUOTE_END__);
                        text = LabelController.Instance.Build("Label", args[0], args[1]).ToString();
                        break;
                }
                if (!string.IsNullOrWhiteSpace(text))
                {
                    File.WriteAllText(filePath, text, System.Text.Encoding.UTF8);
                }
            }
        }
    }
}
