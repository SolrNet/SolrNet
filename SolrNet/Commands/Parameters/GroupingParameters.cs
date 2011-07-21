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
	/// <see cref="http://wiki.apache.org/solr/FieldCollapsing"/>
	/// <see cref="http://wiki.apache.org/solr/FieldCollapsing#parameters"/>
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
		public string Query { get; set; }

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
		/// <see cref="http://wiki.apache.org/solr/FieldCollapsing#parameters"/>
		/// </summary>
		public GroupingFormat Format { get; set; }

		/// <summary>
		/// Constructor for GroupingParameters
		/// </summary>
		public GroupingParameters()
		{
			Format = GroupingFormat.Grouped;
		}
	}
}
