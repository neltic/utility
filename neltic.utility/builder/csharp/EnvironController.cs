namespace neltic.utility.builder.csharp
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Configuration;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using System.Xml.Linq;

    public sealed class EnvironController : CSharpBuilder
    {
        private static readonly EnvironController instance = new EnvironController();
        public static EnvironController Instance { get { return instance; } }

        private char[] __DELIMITER_ARRAY__ = { ';', '|', '\n' };
        private string[] BooleanValues = { "true", "false" };
        private string[] __EXCLUDE_CS__ = { "LocalSqlServer" };
        private string[] __EXCLUDE_AS__ = { "" };

        public StringBuilder Build(string _namespace, string className, string configurationPath, bool constructor = true)
        {
            ExeConfigurationFileMap configFileMap = new ExeConfigurationFileMap() { ExeConfigFilename = configurationPath };
            Configuration configuration = ConfigurationManager.OpenMappedExeConfiguration(configFileMap, ConfigurationUserLevel.None);
            return Build(_namespace, className, configuration, constructor);
        }
        public StringBuilder Build(string _namespace, string className, Configuration configuration, bool constructor = true)
        {
            builder = new StringBuilder();
            indent = string.Empty;

            AddNamespace(_namespace);
            AddIndent();
            AddUsing("System");
            AddUsing("System.Collections.Generic");
            AddUsing("System.Configuration");
            AddUsing("System.Globalization");
            AddUsing("System.Linq");
            AddEmptyLine();
            AddStaticClass(className);
            AddIndent();
            /* propiedades ConnectionStrings */
            for (int i = 0; i < configuration.ConnectionStrings.ConnectionStrings.Count; i++)
            {
                string key = configuration.ConnectionStrings.ConnectionStrings[i].Name;
                if (!__EXCLUDE_CS__.Any(s => s.Equals(key)))
                {
                    string provider = configuration.ConnectionStrings.ConnectionStrings[i].ProviderName;
                    if (provider.Equals("System.Data.EntityClient"))
                    {
                        AddProperty("public static", "string", key + "_");
                        AddProperty("public static", "string", key + "Provider_");
                    }
                    AddProperty("public static", "string", key);
                    if (!string.IsNullOrWhiteSpace(provider))
                    {
                        AddProperty("public static", "string", key + "Provider");
                    }
                }
            }
            /* propiedades AppSettings */
            foreach (var key in configuration.AppSettings.Settings.AllKeys)
            {
                if (!__EXCLUDE_AS__.Any(s => s.Equals(key)))
                {
                    string value = configuration.AppSettings.Settings[key].Value;
                    string type = GetTypeBy(value);
                    AddProperty("public static", type, key);
                }
            }
            AddEmptyLine();
            AddProperty("private static", "char[]", "__DELIMITER_ARRAY__", "{ '" + string.Join("', '", __DELIMITER_ARRAY__).Replace("\n", "\\n") + "' }");
            AddEmptyLine();
            if (constructor)
            {   // inicializador, para agregar datos a la clase parcial :)... no olvidar llamar Reload()
                AddFunction("static", "", className, "", "Reload();");
                AddEmptyLine();
            }
            StartFunction("public static", "void", "Reload", "");
            AddLine("ConfigurationManager.RefreshSection(\"appSettings\");");
            for (int i = 0; i < configuration.ConnectionStrings.ConnectionStrings.Count; i++)
            {
                string key = configuration.ConnectionStrings.ConnectionStrings[i].Name;
                if (!__EXCLUDE_CS__.Any(s => s.Equals(key)))
                {
                    AddLine(GetSettingBy(key, "string", "ConnectionStrings"));
                    string provider = configuration.ConnectionStrings.ConnectionStrings[i].ProviderName;
                    if (!string.IsNullOrWhiteSpace(provider))
                    {
                        AddLine(GetSettingBy(key, "string", "ProviderName"));
                        if (provider.Equals("System.Data.EntityClient"))
                        {
                            AddLine(GetEntityContextBy(key, "Entity"));
                            AddLine(GetEntityContextBy(key, "EntityProvider"));
                        }
                    }
                }
            }
            foreach (var key in configuration.AppSettings.Settings.AllKeys)
            {
                if (!__EXCLUDE_AS__.Any(s => s.Equals(key)))
                {
                    string value = configuration.AppSettings.Settings[key].Value;
                    string type = GetTypeBy(value);
                    AddLine(GetSettingBy(key, type));
                }
            }
            EndFunction();
            AddEmptyLine();
            StartFunction("private static", "string", "GetEntityContext", "string entityContext");
            AddLine("return entityContext.Substring(entityContext.IndexOf(\"string=\\\"\") + 8).TrimEnd('\"');");
            EndFunction();
            AddEmptyLine();
            StartFunction("private static", "string", "GetEntityProviderContext", "string entityContext");
            AddLine("var provider = entityContext.Substring(entityContext.IndexOf(\"provider=\") + 9);");
            AddLine("return provider.Substring(0, provider.IndexOf(';'));");
            EndFunction();
            RemoveIndent();
            CloseBracket();
            RemoveIndent();
            CloseBracket();
            return builder;
        }

        public void SetDelimiterArray(char[] delimiter)
        {
            __DELIMITER_ARRAY__ = delimiter;
        }
        public void SetExcludedConnectionStrings(string[] exclude)
        {
            __EXCLUDE_CS__ = exclude;
        }
        public void SetExcludedAppSettings(string[] exclude)
        {
            __EXCLUDE_AS__ = exclude;
        }

        private string GetTypeBy(string value)
        {
            string type = "string";
            // primero que tenga algo, sino por default es un string
            if (!string.IsNullOrWhiteSpace(value))
            {
                if (Regex.IsMatch(value, @"^([-+])?[0-9]*(?:\.[0-9]*)?$")) // number
                {
                    // vemos si lo tratamos como entero o como double
                    if (value.IndexOf('.') == -1)
                    {
                        // si inicia con 0 lo forzamos a un string
                        if (!value.StartsWith("0"))
                        {
                            long longValue = 0;
                            long.TryParse(value, out longValue);
                            if (longValue > int.MaxValue || -longValue < int.MinValue) // long or int                     
                            {
                                type = "long";
                            }
                            else
                            {
                                type = "int";
                            }
                        }
                    }
                    else
                    {
                        type = "double";
                    }
                }
                else if (Regex.IsMatch(value, @"^\d{4}-((0\d)|(1[012]))-(([012]\d)|3[01])$")) // date
                {
                    DateTime dateValue;
                    if (DateTime.TryParseExact(value, "yyyy-MM-dd", CultureInfo.CurrentCulture, DateTimeStyles.None, out dateValue))
                    {
                        type = "DateTime";
                    }
                }
                else if (BooleanValues.Any(b => value.Equals(b, StringComparison.InvariantCultureIgnoreCase))) // boolean
                {
                    type = "bool";
                }
                else if (value.Length == 1) // char
                {
                    type = "char";
                }
                else if (__DELIMITER_ARRAY__.Any(separator => value.IndexOf(separator) > -1)) // array
                {
                    string subtype = "string";
                    // ahora vemos de que tipo de array se trata
                    string[] values = value.Split(__DELIMITER_ARRAY__, StringSplitOptions.RemoveEmptyEntries);
                    if (values.Length > 0)
                    {
                        subtype = GetTypeBy(values[0]);
                    }
                    type = subtype + "[]";
                }
            }
            return type;

        }
        private string GetSettingBy(string key, string type, string configuration = "AppSettings")
        {
            string prop = string.Empty, convert = string.Empty, transform = string.Empty, subtype = string.Empty, subfix = string.Empty;
            string pattern = "{0}{4} = {2}(ConfigurationManager.{1}[\"{0}\"]){3};";
            if (type.EndsWith("[]"))
            {
                subtype = type.Replace("[]", "");
                type = "[]";
            }
            if (configuration.Equals("ConnectionStrings", StringComparison.InvariantCultureIgnoreCase))
            {
                type = "string";
                transform = ".ConnectionString";
            }
            else if (configuration.Equals("ProviderName", StringComparison.InvariantCultureIgnoreCase))
            {
                type = "string";
                subfix = "Provider";
                transform = ".ProviderName";
                configuration = "ConnectionStrings";
            }
            switch (type)
            {
                case "int":
                case "long":
                case "double":
                case "char":
                    convert = GetConvertBy(type);
                    break;
                case "bool":
                    transform = GetTransformBy(type);
                    break;
                case "DateTime":
                    convert = GetConvertBy(type);
                    transform = GetTransformBy(type);
                    break;
                case "[]":
                    string array = ".Split(__DELIMITER_ARRAY__).Select(v => {0}(v){1}).ToArray()";
                    transform = string.Format(array, GetConvertBy(subtype), GetTransformBy(subtype));
                    break;
                default:

                    break;
            }
            return string.Format(pattern, key, configuration, convert, transform, subfix);
        }
        private string GetEntityContextBy(string key, string type)
        {
            string provider = string.Empty;
            string pattern = "{0}{1}_ = Get{2}Context({0});";
            switch (type)
            {
                case "EntityProvider":
                    provider = "Provider";
                    break;
                default:
                    break;
            }
            return string.Format(pattern, key, provider, type);
        }
        private string GetConvertBy(string type)
        {
            switch (type)
            {
                case "int":
                    return "Convert.ToInt32";
                case "long":
                    return "Convert.ToInt64";
                case "double":
                    return "Convert.ToDouble";
                case "char":
                    return "Convert.ToChar";
                case "DateTime":
                    return "DateTime.ParseExact(";
                default:
                    return "";
            }
        }
        private string GetTransformBy(string type)
        {
            switch (type)
            {
                case "bool":
                    return ".Equals(\"true\", StringComparison.InvariantCultureIgnoreCase)";
                case "DateTime":
                    return ",\"yyyy-MM-dd\",CultureInfo.CurrentCulture)";
                default:
                    return "";
            }
        }

    }
}
