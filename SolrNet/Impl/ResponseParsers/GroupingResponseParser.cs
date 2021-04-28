using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using SolrNet.Utils;

namespace SolrNet.Impl.ResponseParsers {
    /// <summary>
    /// Parses group.fields from query response
    /// </summary>
    /// <typeparam name="T">Document type</typeparam>
    public class GroupingResponseParser<T> : ISolrResponseParser<T> {
        private readonly ISolrDocumentResponseParser<T> docParser;

        /// <inheritdoc />
        public void Parse(XDocument xml, AbstractSolrQueryResults<T> results) {
            results.Switch(query: r => Parse(xml, r),
                           moreLikeThis: F.DoNothing);
        }

        public GroupingResponseParser(ISolrDocumentResponseParser<T> docParser) {
            this.docParser = docParser;
        }

        /// <summary>
        /// Parses the grouped elements
        /// </summary>
        /// <param name="xml"></param>
        /// <param name="results"></param>
        public void Parse(XDocument xml, SolrQueryResults<T> results) {
            var mainGroupingNode = xml.Element("response")
                .Elements("lst")
                .FirstOrDefault(X.AttrEq("name", "grouped"));
            if (mainGroupingNode == null)
                return;

            var groupings =
                from groupNode in mainGroupingNode.Elements()
                let groupName = groupNode.Attribute("name").Value
                let groupResults = ParseGroupedResults(groupNode)
                select new {groupName, groupResults};

            results.Grouping = groupings.ToDictionary(x => x.groupName, x => x.groupResults);
        }

        /// <summary>
        /// Parses collapsed document.ids and their counts
        /// </summary>
        /// <param name="groupNode"></param>
        /// <returns></returns>
        public GroupedResults<T> ParseGroupedResults(XElement groupNode) {

            var ngroupNode = groupNode.Elements("int").FirstOrDefault(X.AttrEq("name", "ngroups"));
            var matchesValue = int.Parse(groupNode.Elements("int").First(X.AttrEq("name", "matches")).Value);

            return new GroupedResults<T> {
                Groups = ParseGroup(groupNode).ToList(),
                Matches = matchesValue,
                Ngroups = ngroupNode == null ? null : (int?)int.Parse(ngroupNode.Value),
            };
        }

        /// <summary>
        /// Parses collapsed document.ids and their counts
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public IEnumerable<Group<T>> ParseGroup(XElement node) {
            return
                from docNode in node.Elements("arr").Where(X.AttrEq("name", "groups")).Elements()
                let groupValueNode = docNode.Elements().FirstOrDefault(X.AttrEq("name", "groupValue"))
                where groupValueNode != null
                let groupValue = groupValueNode.Name == "null"
                                     ? "UNMATCHED"
                                     : //These are the results that do not match the grouping
                                 groupValueNode.Value
                let resultNode = docNode.Elements("result").First(X.AttrEq("name", "doclist"))
                let numFound = Convert.ToInt64(resultNode.Attribute("numFound").Value)
                let docs = docParser.ParseResults(resultNode).ToList()
                select new Group<T> {
                    GroupValue = groupValue,
                    Documents = docs,
                    NumFound = numFound,
                };
        }
    }
}
