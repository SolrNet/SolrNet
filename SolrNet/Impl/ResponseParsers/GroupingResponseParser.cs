using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace SolrNet.Impl.ResponseParsers
{
	/// <summary>
	/// Parses group.fields from query response
	/// </summary>
	/// <typeparam name="T">Document type</typeparam>
	public class GroupingResponseParser<T> : ISolrResponseParser<T>
	{
		private readonly ISolrDocumentResponseParser<T> docParser;

		public GroupingResponseParser(ISolrDocumentResponseParser<T> docParser)
		{
            this.docParser = docParser;
        }

		/// <summary>
		/// Parses the grouped elements
		/// </summary>
		/// <param name="xml"></param>
		/// <param name="results"></param>
		public void Parse(XDocument xml, SolrQueryResults<T> results)
		{
			var mainGroupingNode = xml.XPathSelectElement("response/lst[@name='grouped']");
			if (mainGroupingNode != null)
			{
				foreach (var groupNode in mainGroupingNode.Elements())
				{
					string groupName = groupNode.Attribute("name").Value;

					GroupedResults<T> g = ParseGroupedResults(groupNode);
					results.Grouping.Add(groupName, g);
				}
			}
		}

		/// <summary>
		/// Parses collapsed document.ids and their counts
		/// </summary>
        /// <param name="groupNode"></param>
		/// <returns></returns>
		public GroupedResults<T> ParseGroupedResults(XElement groupNode)
		{

			var g = new GroupedResults<T> {
				Matches = Convert.ToInt32(groupNode.XPathSelectElement("int[@name='matches']").Value),
			};

            var ngroupNode = groupNode.XPathSelectElement("int[@name='ngroups']");

            if (ngroupNode != null)
                g.Ngroups = int.Parse(ngroupNode.Value);

			foreach (var gg in ParseGroup(groupNode)) {
				g.Groups.Add(gg);
			}

			return g;

		}

		/// <summary>
		/// Parses collapsed document.ids and their counts
		/// </summary>
		/// <param name="node"></param>
		/// <returns></returns>
		public IEnumerable<Group<T>> ParseGroup(XElement node)
		{
			foreach (var docNode in node.XPathSelectElement("arr[@name='groups']").Elements())
			{
				XElement groupValueNode = docNode.XPathSelectElements("*[@name='groupValue']").First();
				string groupValue = "";
				if (groupValueNode == null)
				{
					continue;
				}
				if (groupValueNode.Name == "null")
				{
					//These are the results that do not match the grouping
					groupValue = "UNMATCHED";
				}
				else
				{
					groupValue = groupValueNode.Value;
				}

				var g = new Group<T>
				{
					GroupValue = groupValue
				};

				Parsedoclist(docNode, g);

				yield return g;

			}
		}

		private void Parsedoclist(XElement xml, Group<T> results)
		{
			var resultNode = xml.XPathSelectElement("result[@name='doclist']");

			results.NumFound = Convert.ToInt32(resultNode.Attribute("numFound").Value);
			
			foreach (var result in docParser.ParseResults(resultNode))
				results.Documents.Add(result);
		}
	}
}
