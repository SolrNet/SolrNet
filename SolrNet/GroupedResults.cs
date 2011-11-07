using System.Collections.Generic;
using SolrNet.Commands.Parameters;

namespace SolrNet {
    /// <summary>
    /// GroupedResults<typeparamref name="T"/> contains all the results for one group
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GroupedResults<T> {
        /// <summary>
        /// Returns the number of matching (unique!!) documents that are grouped. 
        /// </summary>
        public int Matches { get; set; }

        /// <summary>
        /// Grouped documents 
        /// </summary>
        public ICollection<Group<T>> Groups { get; set; }

        /// <summary>
        /// Number of groups that have matched the query.
        /// Only available if <see cref="GroupingParameters.Ngroups"/> is true
        /// </summary>
        public int? Ngroups { get; set; }

        /// <summary>
        /// Constructur for Groups
        /// </summary>
        public GroupedResults() {
            Groups = new List<Group<T>>();
        }
    }
}