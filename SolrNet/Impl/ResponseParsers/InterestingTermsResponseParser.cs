using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;

namespace SolrNet.Impl.ResponseParsers {
    public class InterestingTermsResponseParser<T> : ISolrMoreLikeThisHandlerResponseParser<T> {
        private static readonly Func<XElement, KeyValuePair<string, float>> extractList =
            x => new KeyValuePair<string, float>(x.Value.Trim(), 0.0f);

        private static readonly Func<XElement, KeyValuePair<string, float>> extractDetails =
            x => new KeyValuePair<string, float>((string) x.Attribute("name"), float.Parse(x.Value, CultureInfo.InvariantCulture.NumberFormat));

        public void Parse(XDocument xml, AbstractSolrQueryResults<T> results) {
            results.Switch(_ => {}, x => Parse(xml, x));
        }

        public void Parse(XDocument xml, SolrMoreLikeThisHandlerResults<T> results) {
            Func<XElement, KeyValuePair<string, float>> extract;

            var it = xml.Element("response").Elements("arr").FirstOrDefault(e => (string) e.Attribute("name") == "interestingTerms");

            if (it == null) {
                it = xml.Element("response").Elements("lst").FirstOrDefault(e => (string) e.Attribute("name") == "interestingTerms");

                if (it == null) {
                    results.InterestingTerms = new List<KeyValuePair<string, float>>();
                    return;
                }

                extract = extractDetails;
            } else {
                extract = extractList;
            }

            results.InterestingTerms = it.Elements().Select(x => extract(x)).ToList();
        }
    }
}