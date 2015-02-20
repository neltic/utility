namespace neltic.utility.builder
{
    using System;
    using System.Text;

    public class CSharpBuilder : DocumentBuilder
    {
        protected void AddNamespace(string _namespace)
        {
            AddLine("namespace {0}", _namespace);
            AddLine("{{");
        }
        protected void AddUsing(string _using)
        {
            AddLine("using {0};", _using);
        }
        protected void AddStaticClass(string className)
        {
            AddClass("public static", className);
        }
        protected void AddClass(string member, string className, bool isPartial = true)
        {
            AddLine("{1} {2}class {0}", className, member, isPartial ? "partial " : "");
            AddLine("{{");
        }

        protected void AddFunction(string member, string returnValue, string name, string param, string function)
        {
            StartFunction(member, returnValue, name, param);
            AddLine("{0}", function);
            EndFunction();
        }
        protected void StartFunction(string member, string returnValue, string name, string param)
        {
            AddLine("{0}{4}{1} {2}({3})", member, returnValue, name, param, !string.IsNullOrWhiteSpace(returnValue) ? " " : "");
            AddLine("{{");
            AddIndent();
        }
        protected void EndFunction()
        {
            RemoveIndent();
            AddLine("}}");
        }

        protected void AddProperty(string member, string type, string property, string value, string endWith = ";")
        {
            AddLine("{0} {1} {2} = {3}{4}", member, type, property, GetProperty(type, value), endWith);
        }
        protected void AddStaticProperty(string type, string property, string value)
        {
            AddProperty("public static", type, property, value);
        }
        protected void AddProperty(string member, string type, string property)
        {
            AddLine("{0} {1} {2} {{ get; set; }}", member, type, property);
        }
        private string GetProperty(string type, string value)
        {
            // TODO: contemplar todos los tipos posibles
            switch (type)
            {
                case "date":
                    value = "new Date(" + value + ")";
                    break;
                case "string":
                    value = "\"" + value + "\"";
                    break;
            }
            return value;
        }
    }
}
