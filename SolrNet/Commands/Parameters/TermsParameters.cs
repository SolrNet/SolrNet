using System.Collections.Generic;

namespace SolrNet.Commands.Parameters {
    /// <summary>
    /// TermsComponent parameters
    /// </summary>
    public class TermsParameters {

        /// <summary>
        /// TermsComponent parameters
        /// </summary>
        /// <param name="field">The name of the field to get the terms from.</param>
        public TermsParameters(string field) {
            Fields = new List<string> { field };
        }

        /// <summary>
        /// TermsComponent parameters
        /// </summary>
        /// <param name="fields">The list of names of the fields to get the terms from.</param>
        public TermsParameters(IEnumerable<string> fields)
        {
            Fields = fields;
        }

        /// <summary>
        /// The name of the field to get the terms from. Required.
        /// (terms.fl)
        /// </summary>
        public IEnumerable<string> Fields { get; set; }

        /// <summary>
        /// Lower bound term to start at.
        /// Optional. If not specified, the empty string is used, meaning start at the beginning of the field.
        /// (terms.lower)
        /// </summary>
        public string Lower { get; set; }
        
        /// <summary>
        /// Include the lower bound term in the result set. Default is true.
        /// (terms.lower.incl)
        /// </summary>
        public bool? LowerInclude { get; set;}

        /// <summary>
        /// The term to stop at. Either upper or terms.limit must be set.
        /// (terms.upper)
        /// </summary>
        public string Upper { get; set; }

        /// <summary>
        /// Include the upper bound term in the result set. Default is false.
        /// (terms.upper.incl)
        /// </summary>
        public bool? UpperInclude { get; set; }

        /// <summary>
        /// The minimum doc frequency to return in order to be included. Results are inclusive of the mincount (i.e. >= mincount)
        /// (terms.mincount)
        /// </summary>
        public int? MinCount { get; set; }

        /// <summary>
        /// The maximum doc frequency. Default is -1 to have no upper bound. Results are inclusive of the maxcount (i.e. less than maxcount)
        /// (terms.maxcount)
        /// </summary>
        public int? MaxCount { get; set;} 

        /// <summary>
        /// Restrict matches to terms that start with the prefix. Optional.
        /// Use this for implementing AutoComplete!
        /// (terms.prefix)
        /// </summary>
        public string Prefix { get; set; }

        /// <summary>
        /// Restrict matches to terms that match the regular expression. Optional. Requires Solr 3.1+
        /// (terms.regex)
        /// </summary>
        public string Regex { get; set; }
         
        /// <summary>
        /// Flags to be used when evaluating the regular expression defined in the "terms.regex" parameter 
        /// (see http://java.sun.com/j2se/1.5.0/docs/api/java/util/regex/Pattern.html#compile%28java.lang.String,%20int%29 for more details).
        /// This parameter can be defined multiple times (each time with different flag). Requires Solr 3.1+
        /// (terms.regex.flag)
        /// </summary>
        public ICollection<RegexFlag> RegexFlag { get; set; }

        /// <summary>
        /// The maximum number of terms to return. The default is 10. If less than 0, then include all terms.
        /// (terms.limit)
        /// </summary>
        public int? Limit { get; set; }

        /// <summary>
        /// If true, return the raw characters of the indexed term, regardless of if it is human readable. 
        /// For instance, the indexed form of numeric numbers is not human readable. The default is false.
        /// (terms.raw)
        /// </summary>
        public bool? Raw { get; set; }

        /// <summary>
        /// If count, sorts the terms by the term frequency (highest count first). If index, returns the terms in index order. 
        /// Default is to sort by count.
        /// (terms.sort)
        /// </summary>
        public TermsSort Sort { get; set; }
       
    }
}