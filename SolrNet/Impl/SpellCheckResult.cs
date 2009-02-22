using System.Collections.Generic;

namespace SolrNet.Impl {
    public class SpellCheckResult {
        public string Query { get; set;}
        public int NumFound { get; set;}
        public int StartOffset { get; set;}
        public int EndOffset { get; set;}
        public ICollection<string> Suggestions { get; set;}
    }
}