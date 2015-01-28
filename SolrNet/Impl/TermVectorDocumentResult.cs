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


	public class TermVectorResult {
	    /// <summary>
	    /// Field name
	    /// </summary>
	    public readonly string Field;

	    /// <summary>
	    /// Term value
	    /// </summary>
	    public readonly string Term;

	    /// <summary>
	    /// Term frequency
	    /// </summary>
	    public readonly int? Tf;

	    /// <summary>
	    /// Document frequency
	    /// </summary>
	    public readonly int? Df;

	    /// <summary>
	    /// TF*IDF weight
	    /// </summary>
	    public readonly double? Tf_Idf;

	    /// <summary>
	    /// Term offsets
	    /// </summary>
	    public readonly ICollection<Offset> Offsets;

	    /// <summary>
	    /// Term offsets
	    /// </summary>
	    public readonly ICollection<int> Positions;

	    public TermVectorResult(string field, string term, int? tf, int? df, double? tfIdf, ICollection<Offset> offsets, ICollection<int> positions) {
	        Field = field;
	        Term = term;
	        Tf = tf;
	        Df = df;
	        Tf_Idf = tfIdf;
	        Offsets = offsets;
	        Positions = positions;
	    }
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