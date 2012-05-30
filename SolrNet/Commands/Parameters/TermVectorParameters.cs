using System.Collections.Generic;

namespace SolrNet.Commands.Parameters
{
	/// <summary>
	/// TermsComponent parameters
	/// </summary>
	public class TermVectorParameters
	{
		/// <summary>
		/// TermsComponent parameters
		/// </summary>
		/// <param name="field">The name of the field to get the terms from.</param>
		public TermVectorParameters(string field)
		{
			Fields = new List<string> { field };
		}

		/// <summary>
		/// TermsComponent parameters
		/// </summary>
		/// <param name="fields">The list of names of the fields to get the terms from.</param>
		public TermVectorParameters(IEnumerable<string> fields)
		{
			Fields = fields;
		}

		/// <summary>
		/// The name of the field to get the terms from. Required.
		/// (terms.fl)
		/// </summary>
		public IEnumerable<string> Fields { get; set; }

		/// <summary>
		/// Return document term frequency info per term in the document.
		/// (tv.tf)
		/// </summary>
		public bool? Tf { get; set; }

		/// <summary>
		/// Return the Document Frequency (DF) of the term in the collection. This can be expensive.
		/// (tv.df)
		/// </summary>
		public bool? Df { get; set; }

		/// <summary>
		/// Calculates tf*idf for each term. Requires the parameters tv.tf and tv.df to be "true".This can be expensive.
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