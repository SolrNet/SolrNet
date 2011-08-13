using System;

namespace SolrNet.Commands.Parameters
{
    /// <summary>
    /// Constants for the choices of algorithms 
    /// </summary>
    public class Algorithms
    {
        public const string Lingo = "org.carrot2.clustering.lingo.LingoClusteringAlgorithm";
        public const string STC = "org.carrot2.clustering.stc.STCClusteringAlgorithm";
        public const string KMeans = "org.carrot2.clustering.kmeans.BisectingKMeansClusteringAlgorithm";
    }

    public class ClusteringParameters
    {
        /// <summary>
        /// Engine to use for clustering
        /// </summary>
        public string Engine { get; set; }

        /// <summary>
        /// When true, the compenent performs a clustering of search results only
        /// </summary>
        public bool? Results { get; set; }

        /// <summary>
        /// When true, the entire document index is clustered. This is not fully implemented in Solr as of 3.3, so it should be false
        /// </summary>
        public bool? Collection { get; set; }

        /// <summary>
        /// Algorithm to use for clustering. Lingo, STC, or KMeans. 
        /// see http://download.carrot2.org/stable/manual/#section.advanced-topics.fine-tuning.choosing-algorithm for details
        /// </summary>
        public string Algorithm { get; set; }

        /// <summary>
        /// Reuqired field that Solr returns as the search result's title. Must be a stored field.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Optional field that Solr delivers as search result's content. Must be a stored field. 
        /// </summary>
        public string Snippet { get; set; }

        /// <summary>
        /// Opional field that Solr delivers as the search result's url. Must be a stored field.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// When true, the snippet field (or title field if there is no snippet) will be highlighted and the highlighted text
        /// will be used for clustering.
        /// </summary>
        public bool? ProduceSummary { get; set; }

        /// <summary>
        /// Only mattes when ProduceSummary is true. Decides highlighting fragment size. Default is 100.
        /// </summary>
        public int? FragSize { get; set; }

        /// <summary>
        /// The number of cluster labels to produce
        /// </summary>
        public int? NumDescriptions { get; set; }

        /// <summary>
        /// When true, output sub-clusters. There is no support for this in Carrot2 currently, should always be false.
        /// </summary>
        public bool? SubClusters { get; set; }

        /// <summary>
        /// Specifies where Carrot2 should get its lexical resources from. 
        /// </summary>
        public string LexicalResources { get; set; }

    }
}