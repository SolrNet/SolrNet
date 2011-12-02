using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using SolrNet.Impl.FieldParsers;

namespace SolrNet.Impl.ResponseParsers {
    public class InterestingTermsResponseParser<T> : ISolrMoreLikeThisHandlerResponseParser<T> {
        public void Parse(XDocument xml, AbstractSolrQueryResults<T> results) {
            results.Switch(_ => {}, x => Parse(xml, x));
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

        public void Parse(XDocument xml, SolrMoreLikeThisHandlerResults<T> results) {
            results.InterestingTerms = ParseListOrDetails(xml);
        }
    }
}