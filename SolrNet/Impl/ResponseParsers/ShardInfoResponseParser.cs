using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using SolrNet.Utils;
using System.Globalization;
using System.Xml.XPath;

namespace SolrNet.Impl.ResponseParsers
{
    class ShardInfoResponseParser<T> : ISolrResponseParser<T>
    {
        public void Parse(XDocument xml, AbstractSolrQueryResults<T> results)
        {
            results.Switch(query: r => Parse(xml, r),
                           moreLikeThis: F.DoNothing);
        }

        public void Parse(XDocument xml, SolrQueryResults<T> results)
        {
            var rootNode = xml.XPathSelectElement("response/lst[@name='shards.Info']");
            if (rootNode != null)
                results.ShardInfoResults = ParseDocuments(rootNode).ToList();
        }


        public IEnumerable<ShardInfoResult> ParseDocuments(XElement rootNode)
        {
            var docNodes = rootNode.Elements("lst");

            foreach (var docNode in docNodes)
            {
                var docNodeName = docNode.Attribute("name").Value;

                if (docNodeName == "warnings")
                {
                    // TODO: warnings
                }
                else if (docNodeName == "uniqueKeyFieldName")
                {
                    //TODO: support for unique key field name
                }
                else
                {
                    yield return ParseDoc(docNode);
                }
            }
        }

        private ShardInfoResult ParseDoc(XElement docNode)
        {
            var fieldNodes = docNode.Elements();
            var uniqueKey = fieldNodes
                .Where(x => x.Attribute("name").ValueOrNull() == "uniqueKey")
                .Select(x => x.Value)
                .FirstOrDefault();
            var termVectorResults = fieldNodes
                .Where(x => x.Attribute("name").ValueOrNull() == "includes")
                .SelectMany(ParseField)
                .ToList();

            return new ShardInfoResult();
        }

        private IEnumerable<TermVectorResult> ParseField(XElement fieldNode)
        {
            return fieldNode.Elements()
                .Select(termNode => ParseTerm(termNode, fieldNode.Attribute("name").Value));
        }

        private TermVectorResult ParseTerm(XElement termNode, string fieldName)
        {
            var nameValues = termNode.Elements()
                .Select(e => new { name = e.Attribute("name").Value, value = e.Value })
                .ToList();

            var tf = nameValues
                .Where(x => x.name == "tf")
                .Select(x => (int?)int.Parse(x.value))
                .FirstOrDefault();

            var df = nameValues
                .Where(x => x.name == "df")
                .Select(x => (int?)int.Parse(x.value))
                .FirstOrDefault();

            var tfidf = nameValues
                .Where(x => x.name == "tf-idf")
                .Select(x => (double?)double.Parse(x.value, CultureInfo.InvariantCulture.NumberFormat))
                .FirstOrDefault();

            var offsets = termNode.Elements().SelectMany(ParseOffsets).ToList();
            var positions = termNode.Elements().SelectMany(ParsePositions).ToList();

            return new TermVectorResult(fieldName,
                term: termNode.Attribute("name").Value,
                tf: tf, df: df, tfIdf: tfidf,
                offsets: offsets, positions: positions);
        }

        private IEnumerable<int> ParsePositions(XElement valueNode)
        {
            return from e in new[] { valueNode }
                   where e.Attribute("name").Value == "positions"
                   from p in e.Elements()
                   select int.Parse(p.Value);
        }

        private IEnumerable<Offset> ParseOffsets(XElement valueNode)
        {
            return from e in valueNode.Elements()
                   where e.Attribute("name").Value == "start"
                   select new Offset(start: int.Parse(e.Value), end: int.Parse(((XElement)e.NextNode).Value));
        }
    }
}
