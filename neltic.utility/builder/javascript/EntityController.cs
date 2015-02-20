namespace neltic.utility.builder.javascript
{
    using System;
    using System.ComponentModel;
    using System.Runtime.Remoting;
    using System.Text;

    public sealed class EntityController : JavaScriptBuilder
    {
        private static readonly EntityController instance = new EntityController();
        public static EntityController Instance { get { return instance; } }

        public StringBuilder Build(string fullEntityName, string primaryKey, out string fileName)
        {
            string assembly = fullEntityName.Substring(0, fullEntityName.LastIndexOf('.'));
            string entity = fullEntityName.Substring(assembly.Length + 1);
            return Build(assembly, entity, primaryKey, out fileName);
        }
        public StringBuilder Build(string assembly, string entity, string primaryKey, out string fileName)
        {
            builder = new StringBuilder();
            indent = string.Empty;
            fileName = entity.Replace('<', '-').Replace(">", "") + ".js";

            AddHeader(assembly);
            StartClass(entity);
            AddIndent();
            StartProperty("model");
            AddIndent();
            AddKeyString("id", primaryKey);
            StartFields();
            AddIndent();
            GetPropertiesType(assembly, entity);
            RemoveIndent();
            CloseBracket();
            RemoveIndent();
            CloseBracket(";");
            StartProperty("entity");
            AddIndent();
            GetProperties(assembly, entity);
            RemoveIndent();
            CloseBracket(";");
            AddProperty("prepare", "[]");
            AddClassFunction("emptyEntity", "return $.extend({}, this.entity);");
            AddClassFunction("getJSON", "transport, properties, firstLetterToLowerCase", "var data = {}; if (typeof (properties) === \"undefined\") { data = this.entity; } else { if (typeof (firstLetterToLowerCase) !== \"boolean\") firstLetterToLowerCase = true; for (var i = 0; i < properties.length; i++) { var property = properties[i]; data[!firstLetterToLowerCase ? property : (property.charAt(0).toLowerCase() + property.slice(1))] = this.entity[property]; } } var json = { __item__: data }; if (transport !== null && typeof (transport) === \"string\") { return JSON.stringify(json).replace(\"__item__\", transport); } else { return JSON.stringify(json.__item__); }");
            AddClassFunction("getPreparedJSON", "firstLetterToLowerCase", "return this.getJSON(null, this.prepare, firstLetterToLowerCase);");
            AddClassFunction("setProperties", "properties", "if (typeof (properties) === \"object\") { for (var property in properties) { if (typeof (this.entity[property]) !== \"undefined\") { var value = properties[property]; if (this.model.fields[property].type === \"date\") { if (typeof (value) == \"string\") { if (value.indexOf(\"/Date(\") > -1) { value = new Date(parseInt(value.replace(/\\/Date\\((-?\\d+)\\)\\//, '$1'))); } } } this.entity[property] = value; this.prepare.push(property); } } }");
            CallClassFunction("setProperties", "properties");
            RemoveIndent();
            CloseBracket(";");

            return builder;
        }

        private void GetPropertiesType(string assembly, string entity)
        {
            ObjectHandle handle = GetEntityInstance(assembly, entity);
            Object p = handle.Unwrap();
            Type t = p.GetType();
            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(p.GetType());
            for (int i = 0; i < props.Count; i++)
            {
                AddKeyValue(props[i].Name, GetPropertyType(props[i]), i < props.Count - 1 ? "," : "");
            }
        }
        private string GetPropertyType(PropertyDescriptor prop)
        {
            string pattern = "{{ type: \"{0}\" }}";
            string type = "string";
            // TODO: contemplar todos los tipos posibles
            var propertyType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
            switch (propertyType.FullName)
            {
                case "System.Net.HttpStatusCode":
                case "System.Int16":
                case "System.Int32":
                case "System.Int64":
                case "System.Double":
                case "System.Decimal":
                    type = "number";
                    break;
                case "System.DateTime":
                    type = "date";
                    break;
                case "System.Boolean":
                    type = "boolean";
                    break;
            }
            return string.Format(pattern, type);
        }

        private void GetProperties(string assembly, string entity)
        {
            ObjectHandle handle = GetEntityInstance(assembly, entity);
            Object p = handle.Unwrap();
            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(p.GetType());
            for (int i = 0; i < props.Count; i++)
            {
                AddKeyValue(props[i].Name, GetProperty(props[i]), i < props.Count - 1 ? "," : "");
            }
        }
        private string GetProperty(PropertyDescriptor prop)
        {
            string pattern = "{0}";
            string value = "\"\"";
            if (Nullable.GetUnderlyingType(prop.PropertyType) == null)
            {
                // TODO: contemplar todos los tipos posibles
                switch (prop.PropertyType.FullName)
                {
                    case "System.Int16":
                    case "System.Int32":
                    case "System.Int64":
                        value = "0";
                        break;
                    case "System.Decimal":
                    case "System.Double":
                        value = "0.0";
                        break;
                    case "System.DateTime":
                        value = "new Date()";
                        break;
                    case "System.Boolean":
                        value = "true";
                        break;
                    case "System.Net.HttpStatusCode":
                        value = "200";
                        break;
                }
            }
            else
            {
                value = "null";
            }
            return string.Format(pattern, value);
        }

        private ObjectHandle GetEntityInstance(string assembly, string entity)
        {
            ObjectHandle handle;
            // modo tradicional
            try
            {
                handle = Activator.CreateInstance(assembly, assembly + "." + entity);
            }
            catch
            {
                // modo reducido
                try
                {
                    handle = Activator.CreateInstance(assembly, entity);
                }
                catch
                {
                    try
                    {
                        // vamos a probar si es un tipo T
                        if (entity.Contains("<") && entity.Contains(">"))
                        {
                            var entityHolder = entity.Split('<', '>');
                            var classHolder = entityHolder[0];
                            entityHolder = entityHolder[1].Split(',');
                            var fulltype = new string[entityHolder.Length];
                            for (int i = 0; i < entityHolder.Length; i++)
                            {
                                var st = Type.GetType("System." + GetSystemTypeBy(entityHolder[i]));
                                fulltype[i] = "[" + st.AssemblyQualifiedName + "]";
                            }
                            entity = classHolder + "`" + fulltype.Length + "[" + string.Join(",", fulltype) + "]";
                            handle = GetEntityInstance(assembly, entity);
                        }
                        else
                        {
                            throw;
                        }
                    }
                    catch
                    {
                        throw;
                    }
                }
            }
            return handle;
        }

    }
}
