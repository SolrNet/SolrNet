using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
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

            var timing = new TimingResults();
            var totalTimeNode = xml.XPathSelectElement("response/lst[@name='debug']/lst[@name='timing']/double[@name='time']");
            if (totalTimeNode != null)
                timing.TotalTime = GetValue(totalTimeNode);

            var processNode = xml.XPathSelectElement("response/lst[@name='debug']/lst[@name='timing']/lst[@name='process']");
            if (processNode != null)
                timing.Process = ParseDocuments(processNode);

            var prepareNode = xml.XPathSelectElement("response/lst[@name='debug']/lst[@name='timing']/lst[@name='prepare']");
            if (prepareNode != null)
                timing.Prepare = ParseDocuments(prepareNode);

            var explainNode = xml.XPathSelectElement("response/lst[@name='debug']/lst[@name='explain']");
            var explanationStructuredField = xml.XPathSelectElement("response/lst[@name='responseHeader']/lst[@name='params']/str[@name='debug.explain.structured']");

            if (explanationStructuredField != null && bool.Parse(explanationStructuredField.Value))
            {
                results.Debug.ExplainStructured = ParseStructuredExplanations(explainNode);
            }
            else
            {
                results.Debug.Explain = ParseSimpleExplanations(explainNode);
            }

            var parsedQuery = xml.XPathSelectElement("response/lst[@name='debug']/str[@name='parsedquery']").Value;
            var parsedQueryString = xml.XPathSelectElement("response/lst[@name='debug']/str[@name='parsedquery_toString']").Value;

            results.Debug.ParsedQuery = parsedQuery;
            results.Debug.ParsedQueryString = parsedQueryString;
            results.Debug.Timing = timing;
        }

        /// <summary>
        /// Parses simple explainations from a query response
        /// </summary>
        /// <param name="rootNode">Explaination root node</param>
        /// <returns>Parsed simple explainations</returns>
        private IDictionary<string, string> ParseSimpleExplanations(XElement rootNode)
        {
            var explainResult = from no in rootNode.Elements()
                                select new { Key = no.Attribute("name").Value, no.Value };

            return explainResult.ToDictionary(i => i.Key, i => i.Value);
        }

        /// <summary>
        /// Parses structured explainations from a query response
        /// </summary>
        /// <param name="rootNode">Explaination root node</param>
        /// <returns>Parsed structured explainations</returns>
        private IEnumerable<ExplainationResult> ParseStructuredExplanations(XElement rootNode)
        {
            var desc = rootNode.XPathSelectElements("lst");
            var list = new List<ExplainationResult>();

            foreach (var item in desc)
            {
                list.Add(ParseExplainationResult(item));
            }

            return list;
        }

        /// <summary>
        /// Parses each explainations from a query response
        /// </summary>
        /// <param name="item">Explaination node</param>
        /// <returns>Parsed explaination result</returns>
        private ExplainationResult ParseExplainationResult(XElement item)
        {
            return new ExplainationResult()
            {
                Key = item.FirstAttribute.Value,
                Explaination = ParseExplainationModel(item)
            };
        }

        /// <summary>
        /// Recursively parses each explaination node from a query response
        /// </summary>
        /// <param name="item">Explaination node</param>
        /// <returns>Parsed explaination model</returns>
        private ExplainationModel ParseExplainationModel(XElement item)
        {
            var result = new ExplainationModel();
            FillExplainationModel(result, item);

            var detailsItems = item.XPathSelectElements("arr[@name='details']/lst");

            foreach (var detailsItem in detailsItems)
            {
                var innerModel = ParseExplainationModel(detailsItem);
                result.Details.Add(innerModel);
            }

            return result;
        }

        /// <summary>
        /// Fills explaination model from xml node
        /// </summary>
        /// <param name="model">Explaination model</param>
        /// <param name="item">Explaination node</param>
        private void FillExplainationModel(ExplainationModel model, XElement item)
        {
            model.Match = bool.Parse(item.XPathSelectElement("bool[@name='match']").Value);
            model.Description = item.XPathSelectElement("str[@name='description']").Value;
            model.Value = double.Parse(item.XPathSelectElement("float[@name='value']").Value, CultureInfo.InvariantCulture.NumberFormat);
        }

        /// <summary>
        /// Parses term vector results
        /// </summary>
        /// <param name="rootNode"></param>
        /// <returns>Parsed documents</returns>
        private IDictionary<string, double> ParseDocuments(XElement rootNode)
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
        private double GetValue(XElement docNode)
        {
            var value = double.Parse(docNode.Value, CultureInfo.InvariantCulture.NumberFormat);
            return value;
        }
    }
}