using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Xml;
using System.Globalization;

namespace SolrNet.Impl.ResponseParsers
{
    public class ClusterResponseParser<T> : ISolrResponseParser<T>
    {
        public ClusterResponseParser() { }
        /// <summary>
        /// Parse the xml document returned by solr 
        /// </summary>
        /// <param name="xml"></param>
        /// <param name="results"></param>
        public void Parse(XDocument xml, SolrQueryResults<T> results) {
            XElement clusterNode = xml.XPathSelectElement("response/arr[@name='clusters']");
            if (clusterNode != null)
                results.Clusters = ParseClusterNode(clusterNode);
        }

        /// <summary>
        /// Grab a list of the documents from a cluster 
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private ICollection<string> GetDocumentList(XElement node) {
            List<string> coll = new List<string>();
            foreach (var d in node.Elements()) {
                coll.Add(d.Value);
            }
            return coll;
        }

        /// <summary>
        /// Assign Title, Score, and documents to a cluster. Adds each cluster
        /// to and returns a ClusterResults 
        /// </summary>
        /// <param name="n"> Node to parse into a Cluster </param>
        /// <returns></returns>
        public ClusterResults ParseClusterNode(XElement n) {
            ClusterResults c = new ClusterResults();
            foreach (var v in n.Elements()) {
                Cluster cluster = new Cluster();
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
