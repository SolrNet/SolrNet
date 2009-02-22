namespace SolrNet.Commands.Parameters {
    /// <summary>
    /// Spell checking parameters
    /// </summary>
    public class SpellCheckingParameters {
        /// <summary>
        /// The query to spellcheck. 
        /// If spellcheck.q is defined, then it is used, otherwise the original input query is used. 
        /// The spellcheck.q parameter is intended to be the original query, minus any extra markup like field names, boosts, etc. 
        /// If the q parameter is specified, then the SpellingQueryConverter class is used to parse it into tokens, otherwise the WhitesepaceTokenizer is used. 
        /// The choice of which one to use is up to the application. 
        /// Essentially, if you have a spelling "ready" version in your application, then it is probably better to send spellcheck.q, otherwise, 
        /// if you just want Solr to do the job, use the q parameter
        /// </summary>
        public string Query { get; set; }

        /// <summary>
        /// Create the dictionary for use by the SolrSpellChecker. 
        /// In typical applications, one needs to build the dictionary before using it. 
        /// However, it may not always be necessary as it is possible to setup the spellchecker with a dictionary that already exists.
        /// </summary>
        public bool? Build { get; set; }

        /// <summary>
        /// Reload the spell checker. 
        /// Depends on the implementation of SolrSpellChecker.reload() but usually means reloading the dictionary
        /// </summary>
        public bool? Reload { get; set; }

        /// <summary>
        /// The maximum number of suggestions to return
        /// </summary>
        public int? Count { get; set; }

        /// <summary>
        /// Only return suggestions that result in more hits for the query than the existing query.
        /// </summary>
        public bool? OnlyMorePopular { get; set; }
        
        //public bool? ExtendedResults { get; set; }

        /// <summary>
        /// Take the best suggestion for each token (if it exists) and construct a new query from the suggestions
        /// </summary>
        public bool? Collate { get; set; }

        /// <summary>
        /// The name of the spellchecker to use. 
        /// This defaults to "default". 
        /// Can be used to invoke a specific spellchecker on a per request basis.
        /// </summary>
        public string Dictionary { get; set; }
    }
}