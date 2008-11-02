using System.Collections.Generic;
using SolrNet.Commands.Parameters;

namespace SolrNet {
	/// <summary>
	/// Executable query
	/// </summary>
	/// <typeparam name="T">Document type</typeparam>
	public interface ISolrQueryExecuter<T>  {
		/// <summary>
		/// Connection to use
		/// </summary>
		ISolrConnection Connection { get; set; }

		ISolrQuery Query { get; set; }

		QueryOptions Options { get; set; }

		/// <summary>
		/// Executes the query and returns results
		/// </summary>
		/// <returns>query results</returns>
		ISolrQueryResults<T> Execute();
	}
}