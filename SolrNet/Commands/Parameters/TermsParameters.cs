using System.Collections.Generic;

namespace SolrNet.Commands.Parameters {

    /// <summary>
    /// Constants for the choices of Regex Flags 
    /// </summary>
    public class RegexFlags
    {
        public const string CaseInsensitive = "case_insensitive";
        public const string Comments = "comments";
        public const string MultiLine = "multiline";
        public const string Literal = "literal";
        public const string Multiline = "multiline";
        public const string DotAll = "dotall";
        public const string UnicodeCase = "unicode_case";
        public const string CanonEq = "canon_eq";
        public const string UnixLines = "unix_lines";
    }

    /// <summary>
    /// Spell checking parameters
    /// </summary>
    public class TermsParameters {

        /// <summary>
        /// The field to get the terms from
        /// terms.fl={FIELD NAME} - Required. The name of the field to get the terms from.
        /// </summary>
        public string Field { get; set; }

        /// <summary>
        /// Lower bound term to start at.
        /// terms.lower={The lower bound term} - Optional. The term to start at. If not specified, the empty string is used, meaning start at the beginning of the field.
        /// </summary>
        public string Lower { get; set; }
        
        /// <summary>
        /// terms.lower.incl={true|false} - Optional. Include the lower bound term in the result set. Default is true.
        /// </summary>
        public bool? LowerInclude { get; set;}

        /// <summary>
        /// terms.upper={The upper bound term} - The term to stop at. Either upper or terms.limit must be set.
        /// </summary>
        public string Upper { get; set; }

        /// <summary>
        /// terms.upper.incl={true|false} - Include the upper bound term in the result set. Default is false.
        /// </summary>
        public bool? UpperInclude { get; set; }

        /// <summary>
        /// terms.mincount=Integer - Optional. The minimum doc frequency to return in order to be included. Results are inclusive of the mincount (i.e. >= mincount)
        /// </summary>
        public int? MinCount { get; set; }

        /// <summary>
        /// terms.maxcount=Integer - Optional. The maximum doc frequency. Default is -1 to have no upper bound. Results are inclusive of the maxcount (i.e. less than maxcount)
        /// </summary>
        public int? MaxCount { get; set;} 

        /// <summary>
        /// Use this for implementing AutoComplete!
        /// terms.prefix={String} - Optional. Restrict matches to terms that start with the prefix.
        /// </summary>
        public string Prefix { get; set; }

        /// <summary>
        /// terms.regex={String} - Optional. Restrict matches to terms that match the regular expression.  Solr3.1
        /// </summary>
        public string Regex { get; set; }
         
        /// <summary>
        /// terms.regex.flag={case_insensitive|comments|multiline|literal|dotall|unicode_case|canon_eq|unix_lines} - Optional. 
        /// Flags to be used when evaluating the regular expression defined in the "terms.regex" parameter (see http://java.sun.com/j2se/1.5.0/docs/api/java/util/regex/Pattern.html#compile%28java.lang.String,%20int%29 fore more details). 
        /// This parameter can be defined multiple times (each time with different flag)  Solr3.1
        /// TODO: Make this an array to allow for mutliple selections
        /// </summary>
        public List<string> RegexFlag { get; set; }       

        /// <summary>
        /// terms.limit={integer} - The maximum number of terms to return. The default is 10. If less than 0, then include all terms.
        /// </summary>
        public int? Limit { get; set; }

        /// <summary>
        /// terms.raw={true|false} - If true, return the raw characters of the indexed term, regardless of if it is human readable. For instance, the indexed form of numeric numbers is not human readable. The default is false.
        /// </summary>
        public bool? Raw { get; set; }

        /// <summary>
        /// terms.sort={count|index} - If count, sorts the terms by the term frequency (highest count first). If index, returns the terms in index order. Default is to sort by count.
        /// </summary>
        public string Sort { get; set; }
       
    }
}