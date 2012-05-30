using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SolrNet.Impl
{
	/// <summary>
	/// Terms Results
	/// </summary>
	public class TermVectorDocumentResult
	{
		/// <summary>
		/// terms field
		/// </summary>
		public string DocumentID { get; set; }

		/// <summary>
		/// Spelling suggestions
		/// </summary>
		public ICollection<TermResult> TermVector { get; set; }

		public TermVectorDocumentResult() {
			TermVector = new Collection<TermResult>();
		}
	}


	public class TermResult
	{
		public TermResult()
		{
			Positions = new List<int>();
			Offsets = new List<Offset>();
		}
		/// <summary>
		/// Field name
		/// </summary>
		public string Field { get; set; }

		/// <summary>
		/// Term value
		/// </summary>
		public string Term { get; set; }

		/// <summary>
		/// Term frequency
		/// </summary>
		public int? Tf { get; set; }

		/// <summary>
		/// Document frequency
		/// </summary>
		public int? Df { get; set; }

		/// <summary>
		/// TF*IDF weight
		/// </summary>
		public double? Tf_Idf { get; set; }

		/// <summary>
		/// Term offsets
		/// </summary>
		public IList<Offset> Offsets { get; set; }

		/// <summary>
		/// Term offsets
		/// </summary>
		public IList<int> Positions { get; set; }
	}

	public class Offset {
		public int Start;
		public int End;
	}
}