using System.Collections.Generic;
using SolrNet.Commands.Parameters;

namespace SolrNet
{
	/// <summary>
	/// GroupedResults<typeparamref name="T"/> contains all the results for one group
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class GroupedResults<T>
	{
		/// <summary>
		/// Returns the number of matching (unique!!) documents that are grouped. 
		/// </summary>
		public int Matches { get; set; }
		/// <summary>
		/// Grouped documents 
		/// </summary>
		public ICollection<Group<T>> Groups { get; set; }

        /// <summary>
        /// Number of groups that have matched the query.
        /// Only available if <see cref="GroupingParameters.Ngroups"/> is true
        /// </summary>
        public int? Ngroups { get; set; }

		/// <summary>
		/// Constructur for Groups
		/// </summary>
		public GroupedResults()
		{
			Groups = new List<Group<T>>();
		}
	}

	/// <summary>
	/// A Single group of documents
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class Group<T>
	{
		/// <summary>
		/// The groupvalue for this group of documents
		/// </summary>
		public string GroupValue { get; set; }

		/// <summary>
		/// Returns the number of matching documents that are found for this groupValue
		/// </summary>
		public int NumFound { get; set; }

		/// <summary>
		/// The actual documents in the group.
		/// You can control the amount of documents in this collection by using the Limit property of the GroupingParameters
		/// </summary>
		public ICollection<T> Documents { get; set; }

		/// <summary>
		/// Default constructor
		/// </summary>
		public Group()
		{
			Documents = new List<T>();
		}
	}
}
