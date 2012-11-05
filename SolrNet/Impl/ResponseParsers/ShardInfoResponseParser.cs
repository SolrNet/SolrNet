using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using SolrNet.Utils;

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
                results.TermVectorResults = ParseDocuments(rootNode).ToList();
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

            return new TermVectorDocumentResult(uniqueKey, termVectorResults);
        }
    }
}
