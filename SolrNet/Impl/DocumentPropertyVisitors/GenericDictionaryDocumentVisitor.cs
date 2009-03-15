using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Xml;
using SolrNet.Utils;

namespace SolrNet.Impl.DocumentPropertyVisitors {
    public class GenericDictionaryDocumentVisitor : ISolrDocumentPropertyVisitor {
        private readonly IReadOnlyMappingManager mapper;
        private readonly ISolrFieldParser parser;

        public GenericDictionaryDocumentVisitor(IReadOnlyMappingManager mapper, ISolrFieldParser parser) {
            this.mapper = mapper;
            this.parser = parser;
        }

        public bool CanHandleType(Type t) {
            return TypeHelper.IsGenericAssignableFrom(typeof(IDictionary<,>), t);
        }

        public object NewKeyValuePair(Type[] typeArgs, object key, object value) {
            var genericType = typeof(KeyValuePair<,>).MakeGenericType(typeArgs);
            return Activator.CreateInstance(genericType, key, value);
        }

        public object NewDictionary(Type[] typeArgs) {
            var genericType = typeof(Dictionary<,>).MakeGenericType(typeArgs);
            return Activator.CreateInstance(genericType);
        }

        public void SetKV(object dict, object key, object value) {
            dict.GetType().GetMethod("set_Item").Invoke(dict, new[] {key, value});
        }

        public object ConvertTo(string s, Type t) {
            var converter = TypeDescriptor.GetConverter(t);
            return converter.ConvertFrom(s);
        }

        public void Visit(object doc, string fieldName, XmlNode field) {
            var allFields = mapper.GetFields(doc.GetType());
            var thisField = Func.FirstOrDefault(allFields, p => fieldName.StartsWith(p.Value));
            if (thisField.Key == null)
                return;
            if (!CanHandleType(thisField.Key.PropertyType))
                return;
            var thisFieldName = thisField.Value;
            if (!field.Attributes["name"].InnerText.StartsWith(thisFieldName))
                return;
            var typeArgs = thisField.Key.PropertyType.GetGenericArguments();
            var keyType = typeArgs[0];
            var valueType = typeArgs[1];
            var dict = thisField.Key.GetValue(doc, null) ?? NewDictionary(typeArgs);
            var key = field.Attributes["name"].InnerText.Substring(thisFieldName.Length);
            var value = parser.Parse(field, valueType);
            SetKV(dict, ConvertTo(key, keyType), value);
            thisField.Key.SetValue(doc, dict, null);
        }
    }
}