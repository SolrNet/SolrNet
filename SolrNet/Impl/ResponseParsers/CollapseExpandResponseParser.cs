using System.Xml.Linq;
using System.Xml.XPath;
using SolrNet.Utils;
using System.Collections.Generic;

namespace SolrNet.Impl.ResponseParsers
{
    /// <summary>
    /// Parses collapse results from query response
    /// </summary>
    /// <typeparam name="T">Document type</typeparam>
    public class CollapseExpandResponseParser<T> : ISolrResponseParser<T>
    {
        private readonly ISolrDocumentResponseParser<T> docParser;

        /// <inheritdoc />
        public void Parse(XDocument xml, AbstractSolrQueryResults<T> results)
        {
            results.Switch(query: r => Parse(xml, r),
                           moreLikeThis: F.DoNothing);
        }

        /// <summary>
        /// Parses collapse results from query response
        /// </summary>
        public CollapseExpandResponseParser(ISolrDocumentResponseParser<T> docParser)
        {
            this.docParser = docParser;
        }

        /// <summary>
        /// Parses the collapsed elements
        /// </summary>
        /// <param name="xml"></param>
        /// <param name="results"></param>
        public void Parse(XDocument xml, SolrQueryResults<T> results)
        {
            var expandElement = xml.XPathSelectElement("response/lst[@name='expanded']");
            if (expandElement == null)
                return;

            results.CollapseExpand = ParseGroupedResults(expandElement);
        }

        /// <summary>
        /// Parses collapsed document.ids and their counts
        /// </summary>
        /// <param name="groupNode"></param>
        /// <returns></returns>
        public CollapseExpandResults<T> ParseGroupedResults(XElement expandElement)
        {
            var resultElements = expandElement.Elements();
            var groups = ParseGroup(resultElements);

            return new CollapseExpandResults<T>(groups);
        }

        /// <summary>
        /// Parses collapsed document.ids and their counts
        /// </summary>
        public ICollection<Group<T>> ParseGroup(IEnumerable<XElement> nodes)
        {
            var groups = new List<Group<T>>();

            foreach (var resultElement in nodes)
            {
                var groupName = resultElement.Attribute("name");

                if (groupName == null)
                    continue;

                var groupMatches = int.Parse(resultElement.Attribute("numFound").Value);
                var parsedDocs = docParser.ParseResults(resultElement);

                groups.Add(new Group<T>()
                {
                    Documents = parsedDocs,
                    GroupValue = groupName.Value,
                    NumFound = groupMatches
                });
            }

            return groups;
        }
    }
}
