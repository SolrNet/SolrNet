using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SolrNet.Commands.Parameters
{


	/// <summary>
	/// Controls the output format of the grouping 
	/// </summary>
	public enum GroupingFormat
	{
		/// <summary>
		/// The documents are presented within their groups
		/// </summary>
		Grouped,
		/// <summary>
		/// Simple : the grouped documents are presented in a single flat list.
		/// Note : The start and rows parameters refer to numbers of documents instead of numbers of groups.
		/// </summary>
		Simple
	}

	/// <summary>
	/// Parameters to query grouping / collapsing 
	/// (Only SOLR 4.0)
	/// 
	/// group.func  AND group.query parameters are missing
	/// See:
	/// http://wiki.apache.org/solr/FieldCollapsing
	/// http://wiki.apache.org/solr/FieldCollapsing#parameters
	/// </summary>
	public class GroupingParameters
	{
		/// <summary>
		/// Fields to group the results by.
		/// Each field will return it's own group.
		/// Group based on the unique values of a field(s). 
		/// The field must currently be single-valued and must be either indexed, or be another field type that has a value source and works in a function query - such as ExternalFileField
		/// </summary>
		public ICollection<string> Fields { get; set; }

		/// <summary>
		/// The number of results (documents) to return for each group. Defaults to 1.
		/// </summary>
		public int? Limit { get; set; }

		/// <summary>
		/// The offset into the document list of each group.
		/// </summary>
		public int? Offset { get; set; }

		/// <summary>
		/// How to sort documents within a single group. Defaults to the same value as the sort parameter.
		/// </summary>
		public ICollection<SortOrder> OrderBy { get; set; }

		/// <summary>
		/// If true, the result of the first field grouping command is used as the main result list in the response, using group.format=simple
		/// default is false
		/// </summary>
		public bool? Main { get; set; }

		/// <summary>
		/// Return a single group of documents that also match the given query.
		/// </summary>
		public ICollection<ISolrQuery> Query { get; set; }

		/// <summary>
		/// Group based on the unique values of a function query.
		/// </summary>
		public string Func { get; set; }

        /// <summary>
        /// Includes the number of groups that have matched the query.
        /// Default is false.
        /// </summary>
        public bool? Ngroups { get; set; }
	
		/// <summary>
		/// Controls the way the group is formatted in the output
		/// See http://wiki.apache.org/solr/FieldCollapsing#parameters
		/// </summary>
		public GroupingFormat Format { get; set; }

        /// <summary>
        /// If true, facet counts are based on the most relevant document of each group matching the query. Same applies for StatsComponent. Default is false. 
        /// Requires Solr 3.4+
        /// See http://wiki.apache.org/solr/FieldCollapsing#parameters
        /// </summary>
        public bool? Truncate { get; set; }

        /// <summary>
        /// Determines whether to compute grouped facets for the specified field facets. Grouped facets are computed based on the first specified group. As with normal field faceting, fields shouldn't be tokenized (otherwise counts are computed for each token). Grouped faceting supports single and multivalued fields. Default is false.
        /// Requires Solr 4.0+
        /// See http://wiki.apache.org/solr/FieldCollapsing#parameters
        /// </summary>
        public bool? Facet { get; set; }

        /// <summary>
        /// If > 0 enables grouping cache. Grouping is executed actual two searches. This option caches the second search. A value of 0 disables grouping caching. Default is 0. 
        /// See http://wiki.apache.org/solr/FieldCollapsing#parameters
        /// </summary>
        public int? CachePercent { get; set; }

		/// <summary>
		/// Constructor for GroupingParameters
		/// </summary>
		public GroupingParameters()
		{
			Format = GroupingFormat.Grouped;
		}
	}
}
