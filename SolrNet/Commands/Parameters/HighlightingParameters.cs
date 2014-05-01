#region license
// Copyright (c) 2007-2010 Mauricio Scheffer
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//      http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System;
using System.Collections.Generic;

namespace SolrNet.Commands.Parameters {
    /// <summary>
    /// Highlighting parameters
    /// </summary>
	public class HighlightingParameters {
		/// <summary>
		/// List of fields to generate highlighted snippets for. 
		/// If left blank, the fields highlighted for the StandardRequestHandler are the defaultSearchField (or the df param if used) 
		/// and for the DisMaxRequestHandler the qf fields are used. 
		/// </summary>
		public ICollection<string> Fields { get; set; }

		/// <summary>
		/// The maximum number of highlighted snippets to generate per field. 
		/// It is possible for any number of snippets from zero to this value to be generated.
		/// If left null, it will use whatever default Solr sets (1 for Solr 1.3)
		/// </summary>
		public int? Snippets { get; set; }

		/// <summary>
		/// The size, in characters, of fragments to consider for highlighting. 
		/// 0 indicates that the whole field value should be used (no fragmenting). 
		/// If left null, it will use whatever default Solr sets (100 for Solr 1.3)
		/// </summary>
		public int? Fragsize { get; set; }

		/// <summary>
		/// If true, then a field will only be highlighted if the query matched in this particular field (normally, terms are highlighted in all requested fields regardless of which field matched the query).
		/// If left null, it will use whatever default Solr sets (false for Solr 1.3)
		/// </summary>
		public bool? RequireFieldMatch { get; set; }

		/// <summary>
		/// If a snippet cannot be generated (due to no terms matching), you can specify a field to use as the backup/default summary.
		/// The default value is to not have a default summary. 
		/// </summary>
		public string AlternateField { get; set; }

		/// <summary>
		/// The text which appears before a highlighted term.
		/// The default value is "&lt;em&gt;"
		/// </summary>
		public string BeforeTerm { get; set; }

		/// <summary>
		/// The text which appears after a highlighted term.
        /// The default value is "&lt;/em&gt;"
		/// </summary>
		public string AfterTerm { get; set; }

        /// <summary>
        /// Set a query request to be highlighted. It overrides q parameter for highlighting. 
        /// Requires Solr 3.5+".
        /// </summary>
        public ISolrQuery Query { get; set; }

		/// <summary>
		/// Factor by which the regex fragmenter can stray from the ideal fragment size (given by hl.fragsize) to accomodate the regular expression. 
		/// For instance, a slop of 0.2 with fragsize of 100 should yield fragments between 80 and 120 characters in length. 
		/// It is usually good to provide a slightly smaller fragsize when using the regex fragmenter. 
		/// The default value is ".6"
		/// </summary>
		public double? RegexSlop { get; set; }

		/// <summary>
		/// The regular expression for fragmenting. 
		/// This could be used to extract sentences
		/// </summary>
		public string RegexPattern { get; set; }

		/// <summary>
		/// Only analyze this many characters from a field when using the regex fragmenter (after which, the fragmenter produces fixed-sized fragments). 
		/// Applying a complicated regex to a huge field is expensive.
		/// The default value is "10000". 
		/// </summary>
		public int? RegexMaxAnalyzedChars { get; set; }

        /// <summary>
        /// Collapse contiguous fragments into a single fragment. "true" indicates contiguous fragments will be collapsed into single fragment. This parameter makes sense for Highlighter only.
        /// The default value is "false", which is also the backward-compatible setting.
        /// </summary>
        public bool? MergeContiguous { get; set; }

        /// <summary>
        /// Use SpanScorer to highlight phrase terms only when they appear within the query phrase in the document. Default value is "false".
        /// </summary>
        public bool? UsePhraseHighlighter { get; set; }
        
        /// <summary>
        /// Uses FastVectorHighlighter (Solr 3.1+). FastVectorHighlighter requires the field is termVectors=on, termPositions=on and termOffsets=on.
        /// The default value is "false"
        /// </summary>
        public bool? UseFastVectorHighlighter { get; set; }

        /// <summary>
        /// If the SpanScorer is also being used, enables highlighting for range/wildcard/fuzzy/prefix queries. This parameter makes sense for Highlighter only.
        /// The default is "false"
        /// </summary>
        public bool? HighlightMultiTerm { get; set; }

        /// <summary>
        /// How many characters into a document to look for suitable snippets. This parameter makes sense for Highlighter only.
        /// The default value is "51200".
        /// </summary>
        public int? MaxAnalyzedChars { get; set; }

        /// <summary>
        /// If hl.alternateField is specified, this parameter specifies the maximum number of characters of the field to return. Any value less than or equal to 0 means unlimited.
        /// The default value is unlimited.
        /// </summary>
        public int? MaxAlternateFieldLength { get; set; }

        /// <summary>
        /// Specify a text snippet generator for highlighted text.
        /// This parameter makes sense for Highlighter only.
        /// The default value is "gap".
        /// </summary>
        public SolrHighlightFragmenter? Fragmenter { get; set; }




	}

    ///<summary>
    /// Different types of fragmenter used when highlighting.
    ///</summary>
    public enum SolrHighlightFragmenter
    {
        ///<summary>
        ///Creates fixed-sized fragments with gaps for multi-valued fields.
        ///</summary>
        Gap,
        ///<summary>
        ///Create fragments that "look like" a certain regular expression. 
        ///</summary>
        Regex
    }
}