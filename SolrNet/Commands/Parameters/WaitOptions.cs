namespace SolrNet.Commands.Parameters {
	public class WaitOptions {
        /// <summary>
        /// Block until a new searcher is opened and registered as the main query searcher, making the changes visible. 
        /// Default is true
        /// </summary>
		public bool? WaitSearcher { get; set; }

        /// <summary>
        /// Block until index changes are flushed to disk
        /// Default is true
        /// </summary>
		public bool? WaitFlush { get; set; }
	}
}