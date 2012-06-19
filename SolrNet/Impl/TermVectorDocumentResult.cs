using System;
using System.Collections.Generic;

namespace SolrNet.Impl
{
	/// <summary>
	/// Terms Results
	/// </summary>
	public class TermVectorDocumentResult {
	    /// <summary>
	    /// Unique key of document
	    /// </summary>
	    public readonly string UniqueKey;

	    /// <summary>
	    /// Term Vectors
	    /// </summary>
	    public readonly ICollection<TermVectorResult> TermVector;

	    public TermVectorDocumentResult(string uniqueKey, ICollection<TermVectorResult> termVector) {
            if (termVector == null)
                throw new ArgumentNullException("termVector");
	        UniqueKey = uniqueKey;
	        TermVector = termVector;
	    }
	}


	public class TermVectorResult
	{
		public TermVectorResult()
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
		public readonly int Start;
		public readonly int End;

	    public Offset(int start, int end) {
	        Start = start;
	        End = end;
	    }
	}
}