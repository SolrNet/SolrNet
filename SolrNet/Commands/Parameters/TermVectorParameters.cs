using System.Collections.Generic;

namespace SolrNet.Commands.Parameters
{
	/// <summary>
	/// TermsVectorComponent parameters
	/// </summary>
	public class TermVectorParameters
	{
		/// <summary>
        /// Provides the list of fields to get term vectors for (defaults to fl)
		/// (tv.fl)
		/// </summary>
		public IEnumerable<string> Fields { get; set; }

		/// <summary>
		/// Return document term frequency info per term in the document.
		/// (tv.tf)
		/// </summary>
		public bool? Tf { get; set; }

		/// <summary>
		/// Return the Document Frequency (DF) of the term in the collection. 
		/// This can be expensive.
		/// (tv.df)
		/// </summary>
		public bool? Df { get; set; }

		/// <summary>
		/// Calculates tf*idf for each term. Requires the parameters tv.tf and tv.df to be "true".
		/// This can be expensive.
		/// (tv.tf_idf)
		/// </summary>
		public bool? Tf_Idf { get; set; }

		/// <summary>
		/// If true, turn on extra information (tv.tf, tv.df, etc)
		/// (tv.all )
		/// </summary>
		public bool? All { get; set; }
	}
}