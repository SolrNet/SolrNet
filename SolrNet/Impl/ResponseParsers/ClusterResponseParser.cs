using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using SolrNet.Utils;

namespace SolrNet.Impl.ResponseParsers {
    public class ClusterResponseParser<T> : ISolrResponseParser<T> {
        /// <inheritdoc/>
        public void Parse(XDocument xml, AbstractSolrQueryResults<T> results) {
            results.Switch(query: r => Parse(xml, r), 
                           moreLikeThis: F.DoNothing);
        }

        /// <summary>
        /// Parse the xml document returned by solr 
        /// </summary>
        /// <param name="xml"></param>
        /// <param name="results"></param>
        public void Parse(XDocument xml, SolrQueryResults<T> results) {
            var clusterNode = xml.Element("response")
                .Elements("arr")
                .FirstOrDefault(X.AttrEq("name", "clusters"));
            if (clusterNode != null)
                results.Clusters = ParseClusterNode(clusterNode);
        }

        /// <summary>
        /// Grab a list of the documents from a cluster 
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private static ICollection<string> GetDocumentList(XElement node) {
            return node.Elements().Select(d => d.Value).ToList();
        }

        /// <summary>
        /// Assign Title, Score, and documents to a cluster. Adds each cluster
        /// to and returns a ClusterResults 
        /// </summary>
        /// <param name="n"> Node to parse into a Cluster </param>
        /// <returns></returns>
        public ClusterResults ParseClusterNode(XElement n) {
            var c = new ClusterResults();
            foreach (var v in n.Elements()) {
                var cluster = new Cluster();
                foreach (var x in v.Elements()) {
                    var name = x.Attribute("name").Value;
                    if (name == "labels")
                        cluster.Label = Convert.ToString(x.Value, CultureInfo.InvariantCulture);
                    else if (name == "score")
                        cluster.Score = Convert.ToDouble(x.Value, CultureInfo.InvariantCulture);
                    else if (name == "docs")
                        cluster.Documents = GetDocumentList(x);
                }
                c.Clusters.Add(cluster);
            }
            return c;
        }
    }
}
