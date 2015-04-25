using System;
using System.Collections.Generic;
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
            var debugNode = xml.XPathSelectElement("response/lst[@name='debug']");
            if (debugNode == null)
                return;

            double totalTime = 0;
            IDictionary<string, double> processingTime = null;
            IDictionary<string, double> preparingTime = null;

            var totalTimeNode = xml.XPathSelectElement("response/lst[@name='debug']/lst[@name='timing']/double[@name='time']");
            if (totalTimeNode != null)
                totalTime = GetValue(totalTimeNode);

            var processNode = xml.XPathSelectElement("response/lst[@name='debug']/lst[@name='timing']/lst[@name='process']");
            if (processNode != null)
                processingTime = ParseDocuments(processNode);

            var prepareNode = xml.XPathSelectElement("response/lst[@name='debug']/lst[@name='timing']/lst[@name='prepare']");
            if (prepareNode != null)
                preparingTime = ParseDocuments(prepareNode);

            var parsedQuery = xml.XPathSelectElement("response/lst[@name='debug']/str[@name='parsedquery']").Value;
            var parsedQueryString = xml.XPathSelectElement("response/lst[@name='debug']/str[@name='parsedquery_toString']").Value;
            var timing = new TimingResults(totalTime, preparingTime, processingTime);

            var explainNode = xml.XPathSelectElement("response/lst[@name='debug']/lst[@name='explain']");

            DebugResults debugResults;

            var structuredExplanation = TryParseStructuredExplanations(explainNode);
            if (structuredExplanation != null) 
            {
                debugResults = new DebugResults.StructuredDebugResults(timing, parsedQuery, parsedQueryString, structuredExplanation);
            } 
            else 
            {
                var plainExplanation = ParseSimpleExplanations(explainNode);
                debugResults = new DebugResults.PlainDebugResults(timing, parsedQuery, parsedQueryString, plainExplanation);
            }

            results.Debug = debugResults;
        }

        /// <summary>
        /// Parses simple explanations from a query response
        /// </summary>
        /// <param name="rootNode">Explanation root node</param>
        /// <returns>Parsed simple explanations</returns>
        private static IDictionary<string, string> ParseSimpleExplanations(XElement rootNode)
        {
            var explainResult = from no in rootNode.Elements()
                                select new { Key = no.Attribute("name").Value, no.Value };

            return explainResult.ToDictionary(i => i.Key, i => i.Value);
        }

        /// <summary>
        /// Parses structured explanations from a query response
        /// </summary>
        /// <param name="rootNode">Explanation root node</param>
        /// <returns>Parsed structured explanations</returns>
        private static IDictionary<string, ExplanationModel> TryParseStructuredExplanations(XElement rootNode)
        {
            var desc = rootNode.XPathSelectElements("lst");

            if (!desc.Any())
                return null;

            var result = new Dictionary<string, ExplanationModel>();

            foreach (var item in desc)
            {
                var key = item.FirstAttribute.Value;
                var explanationModel = ParseExplanationModel(item);
                result.Add(key, explanationModel);
            }

            return result;
        }

        /// <summary>
        /// Recursively parses each explaination node from a query response
        /// </summary>
        /// <param name="item">Explanation node</param>
        /// <returns>Parsed explanation model</returns>
        private static ExplanationModel ParseExplanationModel(XElement item)
        {
            var detailsResult = new List<ExplanationModel>();
            var detailsItems = item.XPathSelectElements("arr[@name='details']/lst");

            foreach (var detailsItem in detailsItems)
            {
                var innerModel = ParseExplanationModel(detailsItem);
                detailsResult.Add(innerModel);
            }

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
            var value = double.Parse(item.XPathSelectElement("float[@name='value']").Value, CultureInfo.InvariantCulture.NumberFormat);

            return new ExplanationModel(match, value, description, details);
        }

        /// <summary>
        /// Parses term vector results
        /// </summary>
        /// <param name="rootNode"></param>
        /// <returns>Parsed documents</returns>
        private static IDictionary<string, double> ParseDocuments(XElement rootNode)
        {
            var docNodes = rootNode.Elements("lst");

            var dic = new Dictionary<string, double>();
            foreach (var docNode in docNodes)
            {
                var value = GetValue(docNode.Elements().FirstOrDefault());

                var docNodeName = docNode.Attribute("name").Value;
                dic.Add(docNodeName, value);
            }

            return dic;
        }

        /// <summary>
        /// Parses double from xml node
        /// </summary>
        /// <param name="docNode">Xml item</param>
        /// <returns>Parsed double</returns>
        private static double GetValue(XElement docNode)
        {
            var value = double.Parse(docNode.Value, CultureInfo.InvariantCulture.NumberFormat);
            return value;
        }
    }
}