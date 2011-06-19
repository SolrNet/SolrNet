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


using System.Collections.Generic;

namespace SolrNet.Commands.Parameters {
    /// <summary>
    /// More-like-this parameters
    /// See http://wiki.apache.org/solr/MoreLikeThis
    /// </summary>
    public class MoreLikeThisParameters {
        static private readonly string DefaultMoreLikeThisHandler = "/mlt";
        private readonly IEnumerable<string> fields;

        /// <summary>
        /// Creates more-like-this parameters
        /// </summary>
        /// <param name="fields">The fields to use for similarity. </param>
        public MoreLikeThisParameters(IEnumerable<string> fields) {
            this.fields = fields;
            this.UseMoreLikeThisHandler = false;
            this.Handler = DefaultMoreLikeThisHandler;
        }

        /// <summary>
        /// The fields to use for similarity. 
        /// NOTE: if possible, these should have a stored TermVector
        /// </summary>
        public IEnumerable<string> Fields { 
            get { return fields; }
        }

        /// <summary>
        /// Minimum Term Frequency - the frequency below which terms will be ignored in the source doc.
        /// </summary>
        public int? MinTermFreq { get; set; }

        /// <summary>
        /// Minimum Document Frequency - the frequency at which words will be ignored which do not occur in at least this many docs.
        /// </summary>
        public int? MinDocFreq { get; set; }

        /// <summary>
        /// Minimum word length below which words will be ignored.
        /// </summary>
        public int? MinWordLength { get; set; }

        /// <summary>
        /// Maximum word length above which words will be ignored.
        /// </summary>
        public int? MaxWordLength { get; set; }

        /// <summary>
        /// Maximum number of query terms that will be included in any generated query.
        /// </summary>
        public int? MaxQueryTerms { get; set; }

        /// <summary>
        /// Maximum number of tokens to parse in each example doc field that is not stored with TermVector support.
        /// </summary>
        public int? MaxTokens { get; set; }

        /// <summary>
        /// Set if the query will be boosted by the interesting term relevance.
        /// </summary>
        public bool? Boost { get; set; }

        /// <summary>
        /// Query fields and their boosts using the same format as that used in DisMaxRequestHandler. 
        /// These fields must also be specified in <see cref="Fields"/>
        /// </summary>
        public ICollection<string> QueryFields { get; set; }

        /// <summary>
        /// The number of similar documents to return for each result.
        /// </summary>
        public int? Count { get; set; }

        /// <summary>
        /// This parameter is used to indicate that the MoreLikeThisHandler should be used along with the 
        /// MoreListThisHandlerParameters that include the specific configuration for the handler in Solr
        /// </summary>
        public bool UseMoreLikeThisHandler { set; get; }
        
        /// <summary>
        /// The name of the MoreLikeThisHandler as configured in Solr
        /// </summary>
        public string Handler { get; set; }

        /// <summary>
        /// Indicates whether the matched document should be included in the normal/select response
        /// </summary>
        public bool? IncludeMatch { get; set; }

        /// <summary>
        /// The offset into the matched documents that will be used to find MoreLikeThis items
        /// </summary>
        public int? MatchOffset { get; set; }

        public enum InterestingTerms
        {
            list,
            details,
            none
        }

        /// <summary>
        /// This will set what interesting terms are to be shown as part of the MoreLikeThis response
        /// </summary>
        public InterestingTerms? ShowTerms { get; set; }
    }
}