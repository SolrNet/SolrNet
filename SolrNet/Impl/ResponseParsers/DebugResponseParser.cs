using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using SolrNet.Mapping.Validation;
using SolrNet.Utils;

namespace SolrNet.Impl.ResponseParsers
{
    /// <summary>
    /// Parses debug results from a query response
    /// </summary>
    /// <typeparam name="T">Document type</typeparam>
    public class DebugResponseParser<T> : ISolrResponseParser<T>
    {
        /// <summary>
        /// Parses debug results from a query response
        /// </summary>
        /// <param name="xml">Solr xml response</param>
        /// <param name="results">Solr query results</param>
        public void Parse(XDocument xml, AbstractSolrQueryResults<T> results)
        {
            results.Switch(query: r => Parse(xml, r),
                           moreLikeThis: F.DoNothing);
        }

        /// <summary>
        /// Parses debug results from a query response
        /// </summary>
        /// <param name="xml">Solr xml response</param>
        /// <param name="results">Solr query results</param>
        public void Parse(XDocument xml, SolrQueryResults<T> results)
        {
            results.Debug = ParseDebugResults(xml);
        }

        private static DebugResults ParseDebugResults(XDocument xml) {
            var debugNode = xml.XPathSelectElement("response/lst[@name='debug']");
            if (debugNode == null)
                return null;

            var totalTimeNode = xml.XPathSelectElement("response/lst[@name='debug']/lst[@name='timing']/double[@name='time']");
            var totalTime = GetValue(totalTimeNode);

            var processNode = xml.XPathSelectElement("response/lst[@name='debug']/lst[@name='timing']/lst[@name='process']");
            var processingTime = ParseDocuments(processNode);

            var prepareNode = xml.XPathSelectElement("response/lst[@name='debug']/lst[@name='timing']/lst[@name='prepare']");
            var preparingTime = ParseDocuments(prepareNode);

            var parsedQuery = xml.XPathSelectElement("response/lst[@name='debug']/str[@name='parsedquery']").Value;
            var parsedQueryString = xml.XPathSelectElement("response/lst[@name='debug']/str[@name='parsedquery_toString']").Value;
            var timing = new TimingResults(totalTime, preparingTime, processingTime);

            var explainNode = xml.XPathSelectElement("response/lst[@name='debug']/lst[@name='explain']");

            var debugResults = F.Func<DebugResults>(() => {
                var structuredExplanation = TryParseStructuredExplanations(explainNode);
                if (structuredExplanation != null) {
                    return new DebugResults.StructuredDebugResults(timing, parsedQuery, parsedQueryString, structuredExplanation);
                }
                var plainExplanation = ParseSimpleExplanations(explainNode);
                return new DebugResults.PlainDebugResults(timing, parsedQuery, parsedQueryString, plainExplanation);
            })();

            return debugResults;
        }

        /// <summary>
        /// Parses simple explanations from a query response
        /// </summary>
        /// <param name="rootNode">Explanation root node</param>
        /// <returns>Parsed simple explanations</returns>
        private static IDictionary<string, string> ParseSimpleExplanations(XElement rootNode)
        {
            return rootNode.Elements().ToDictionary(x => x.Attribute("name").Value, x => x.Value);
        }

        /// <summary>
        /// Parses structured explanations from a query response
        /// </summary>
        /// <param name="rootNode">Explanation root node</param>
        /// <returns>Parsed structured explanations</returns>
        private static IDictionary<string, ExplanationModel> TryParseStructuredExplanations(XElement rootNode)
        {
            if (rootNode == null)
                return null;

            var desc = rootNode.XPathSelectElements("lst");

            if (!desc.Any())
                return null;

            var result = desc.ToDictionary(
                keySelector: x => x.FirstAttribute.Value,
                elementSelector: ParseExplanationModel);

            return result;
        }

        /// <summary>
        /// Recursively parses each explaination node from a query response
        /// </summary>
        /// <param name="item">Explanation node</param>
        /// <returns>Parsed explanation model</returns>
        private static ExplanationModel ParseExplanationModel(XElement item)
        {
            var detailsItems = item.XPathSelectElements("arr[@name='details']/lst");
            var detailsResult = detailsItems.Select(ParseExplanationModel).ToList();
            var result = CreateExplanationModel(item, detailsResult);
            return result;
        }

        /// <summary>
        /// Fills explaination model from xml node
        /// </summary>
        /// <param name="item">Explanation node</param>
        /// <param name="details">Explanation details</param>
        private static ExplanationModel CreateExplanationModel(XElement item, ICollection<ExplanationModel> details)
        {
            var match = bool.Parse(item.XPathSelectElement("bool[@name='match']").Value);
            var description = item.XPathSelectElement("str[@name='description']").Value;
            var value = double.Parse(item.XPathSelectElement("float[@name='value']").Value, CultureInfo.InvariantCulture);

            return new ExplanationModel(match, value, description, details);
        }

        /// <summary>
        /// Parses term vector results
        /// </summary>
        /// <param name="rootNode"></param>
        /// <returns>Parsed documents</returns>
        private static IDictionary<string, double> ParseDocuments(XElement rootNode)
        {
            if (rootNode == null)
                return new Dictionary<string, double>();

            var docNodes = rootNode.Elements("lst");

            return docNodes.ToDictionary(
                keySelector: docNode => docNode.Attribute("name").Value,
                elementSelector: docNode => GetValue(docNode.Elements().FirstOrDefault()));
        }

        /// <summary>
        /// Parses double from xml node
        /// </summary>
        /// <param name="docNode">Xml item</param>
        /// <returns>Parsed double</returns>
        private static double GetValue(XElement docNode)
        {
            if (docNode == null)
                return 0;
            var value = double.Parse(docNode.Value, CultureInfo.InvariantCulture);
            return value;
        }
    }
}