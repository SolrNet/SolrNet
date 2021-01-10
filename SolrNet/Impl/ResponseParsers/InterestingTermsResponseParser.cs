using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using SolrNet.Impl.FieldParsers;
using SolrNet.Utils;

namespace SolrNet.Impl.ResponseParsers {
    public class InterestingTermsResponseParser<T> : ISolrMoreLikeThisHandlerResponseParser<T> {
        /// <inheritdoc />
        public void Parse(XDocument xml, AbstractSolrQueryResults<T> results) {
            results.Switch(query: F.DoNothing,
                           moreLikeThis: r => Parse(xml, r));
        }

        public static IEnumerable<KeyValuePair<string,float>> ParseList(XDocument xml) {
            var root = 
                xml.Element("response")
                    .Elements("arr")
                    .FirstOrDefault(e => e.Attribute("name").Value == "interestingTerms");
            if (root == null)
                return Enumerable.Empty<KeyValuePair<string, float>>();
            return root.Elements()
                .Select(x => new KeyValuePair<string, float>(x.Value.Trim(), 0.0f));
        }

        public static IEnumerable<KeyValuePair<string, float>> ParseDetails(XDocument xml) {
            var root =
                xml.Element("response")
                .Elements("lst")
                .FirstOrDefault(e => e.Attribute("name").Value == "interestingTerms");
            if (root == null)
                return Enumerable.Empty<KeyValuePair<string, float>>();
            return root.Elements()
                .Select(x => new KeyValuePair<string, float>(x.Attribute("name").Value, FloatFieldParser.Parse(x)));
        }

        public static IList<KeyValuePair<string, float>> ParseListOrDetails(XDocument xml) {
            var list = ParseList(xml).ToList();
            if (list.Count > 0)
                return list;
            return ParseDetails(xml).ToList();
        }

        /// <inheritdoc />
        public void Parse(XDocument xml, SolrMoreLikeThisHandlerResults<T> results) {
            results.InterestingTerms = ParseListOrDetails(xml);
        }
    }
}
