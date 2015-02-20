namespace neltic.utility.builder
{
    using System;
    using System.Text;

    public class JavaScriptBuilder : DocumentBuilder
    {
        protected void AddKeyString(string key, string value, string endWith = ",")
        {
            AddLine("{0}: \"{1}\"{2}", key, value, endWith);
        }
        protected void AddKeyValue(string key, string value, string endWith = ",")
        {
            AddLine("{0}: {1}{2}", key, value, endWith);
        }

        protected void StartClass(string className)
        {
            if (className.IndexOf('<') > -1)
            {
                var entityHolder = className.Split('<', '>');
                var classHolder = entityHolder[0];
                entityHolder = entityHolder[1].Split(',');
                var fulltype = new string[entityHolder.Length];
                for (int i = 0; i < entityHolder.Length; i++)
                {
                    fulltype[i] = GetSystemTypeBy(entityHolder[i]);
                }
                className = classHolder + string.Join("", fulltype);
            }
            AddLine("var {0} = function (properties) {{", className);
        }

        protected void StartObject(string objectName)
        {
            AddLine("var {0} = {{", objectName);
        }

        protected void StartProperty(string property)
        {
            AddLine("this.{0} = {{", property);
        }
        protected void AddProperty(string property, string value)
        {
            AddLine("this.{0} = {1};", property, value);
        }

        protected void StartFields()
        {
            AddLine("fields: {{");
        }

        protected void AddFunction(string name, string param, string function)
        {
            AddLine("function {0}({1}) {{ {2} }}", name, param, function);
        }
        protected void AddFunction(string name, string function)
        {
            AddFunction(name, "", function);
        }
        protected void AddClassFunction(string name, string param, string function)
        {
            AddLine("this.{0} = function({1}) {{ {2} }}", name, param, function);
        }
        protected void AddClassFunction(string name, string function)
        {
            AddClassFunction(name, "", function);
        }
        protected void CallClassFunction(string name, string param)
        {
            AddLine("this.{0}({1});", name, param);
        }
        protected void AddOnReadyFunction(string function)
        {
            AddLine("$(function () {{ {0} }});", function);
        }

        protected string GetSystemTypeBy(string type)
        {
            switch (type)
            {
                case "bool":
                    return "Boolean";
                case "int":
                    return "Int32";
                case "long":
                    return "Int64";
                case "double":
                    return "Double";
                case "string":
                    return "String";
                case "char":
                    return "Char";
                case "date":
                case "DateTime":
                    return "DateTime";
                default:
                    return type;
            }
        }
    }
}
