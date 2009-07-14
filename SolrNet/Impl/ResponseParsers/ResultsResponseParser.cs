using System;
using System.Globalization;
using System.Xml;

namespace SolrNet.Impl.ResponseParsers {
    public class ResultsResponseParser<T> : ISolrResponseParser<T> where T : new() {
        private readonly ISolrDocumentResponseParser<T> docParser;

        public ResultsResponseParser(ISolrDocumentResponseParser<T> docParser) {
            this.docParser = docParser;
        }

        public void Parse(XmlDocument xml, SolrQueryResults<T> results) {
            var resultNode = xml.SelectSingleNode("response/result");
            results.NumFound = Convert.ToInt32(resultNode.Attributes["numFound"].InnerText);
            var maxScore = resultNode.Attributes["maxScore"];
            if (maxScore != null) {
                results.MaxScore = double.Parse(maxScore.InnerText, CultureInfo.InvariantCulture.NumberFormat);
            }

            foreach (var result in docParser.ParseResults(resultNode))
                results.Add(result);
        }
    }
}