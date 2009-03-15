using System.Xml;
using SolrNet.Utils;

namespace SolrNet.Impl.DocumentPropertyVisitors {
    public class RegularDocumentVisitor : ISolrDocumentPropertyVisitor {
        private readonly ISolrFieldParser parser;
        private readonly IReadOnlyMappingManager mapper;

        public RegularDocumentVisitor(ISolrFieldParser parser, IReadOnlyMappingManager mapper) {
            this.parser = parser;
            this.mapper = mapper;
        }

        public void Visit(object doc, string fieldName, XmlNode field) {
            var allFields = mapper.GetFields(doc.GetType());
            var thisField = Func.FirstOrDefault(allFields, p => p.Value == fieldName);
            if (thisField.Key == null)
                return;
            if (parser.CanHandleSolrType(field.Name) &&
                parser.CanHandleType(thisField.Key.PropertyType)) {
                var v = parser.Parse(field, thisField.Key.PropertyType);
                thisField.Key.SetValue(doc, v, null);
            }
        }
    }
}