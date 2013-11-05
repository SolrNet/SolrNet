using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace SolrNet.Impl {
    public class SolrDocumentSerializer : ISolrDocumentSerializer<SolrDocument> {
        public XElement SerializeDocOrField(SolrDocumentOrField docOrField) {
            return docOrField.Match(doc => Serialize(doc, null), SerializeField);
        }

        private static XElement SerializeField(SolrField field) {
            var content = new XObject[][] {
                GetBoostAttribute(field.Boost).ToArray(),
                new[] {new XAttribute("name", field.Name)},
                new[] {new XText(RemoveControlCharacters(field.Value)), },
            }.SelectMany(x => x);
            var fieldNode = new XElement("field", content.ToArray());
            return fieldNode;
        }

        private static IEnumerable<XAttribute> GetBoostAttribute(double? boost) {
            if (boost.HasValue)
                yield return new XAttribute("boost", boost.Value.ToString(CultureInfo.InvariantCulture.NumberFormat));
        }

        /// <summary>
        /// Serialize a <see cref="SolrDocument"/> to Solr XML
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="boost">Used only if SolrDocument's boost is not defined</param>
        /// <returns></returns>
        public XElement Serialize(SolrDocument doc, double? boost) {
            boost = doc.Boost ?? boost;
            var subElements = doc.Fields.Select(SerializeDocOrField).Select(x => (XObject)x);
            var boostAttr = GetBoostAttribute(boost).Select(x => (XObject)x);
            var docNode = new XElement("doc", boostAttr.Concat(subElements).ToArray());
            return docNode;
        }

        private static readonly Regex ControlCharacters =
            new Regex(@"[^\x09\x0A\x0D\x20-\uD7FF\uE000-\uFFFD\u10000-u10FFFF]", RegexOptions.Compiled);

        // http://stackoverflow.com/a/14323524/21239
        public static string RemoveControlCharacters(string xml) {
            if (xml == null)
                return null;
            return ControlCharacters.Replace(xml, "");
        }
    }
}