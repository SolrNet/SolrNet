using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace SolrNet.Cloud {
    /// <summary>
    /// Represents cloud state
    /// </summary>
    public class SolrCloudState {
        /// <summary>
        /// State collections
        /// </summary>
        public IReadOnlyDictionary<string, SolrCloudCollection> Collections { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        public SolrCloudState(IReadOnlyDictionary<string, SolrCloudCollection> collections) {
            Collections = collections;
        }

        /// <summary>
        /// Returns merged cloud states
        /// </summary>
        [Pure]
        public SolrCloudState Merge(SolrCloudState state)
        {
            if (state == null || state.Collections == null || !state.Collections.Any())
                return this;

            var r = Collections.ToDictionary(kv => kv.Key, kv => kv.Value);
            foreach (var element in state.Collections)
            {
                r.Add(element.Key, element.Value);
            }

            return new SolrCloudState(r);
        }
    }
}
