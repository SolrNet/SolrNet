using System.Collections.Generic;
using SolrNet.Commands.Parameters;

namespace SolrNet {
	/// <summary>
	/// Executable query
	/// </summary>
	/// <typeparam name="T">Document type</typeparam>
	public interface ISolrQueryExecuter<T>  {
		/// <summary>
		/// Executes the query and returns results
		/// </summary>
		/// <returns>query results</returns>
		ISolrQueryResults<T> Execute(ISolrQuery q, QueryOptions options);
	}
}