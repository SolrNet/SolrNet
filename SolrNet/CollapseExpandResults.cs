using System.Collections.Generic;

namespace SolrNet
{
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class CollapseExpandResults<T>
    {
        /// <summary>
        /// Grouped documents 
        /// </summary>
        public ICollection<Group<T>> Groups { get; set; }

        /// <summary>
        /// Constructor for CollapseExpandResults
        /// </summary>
        public CollapseExpandResults()
        {
            Groups = new List<Group<T>>();
        }
    }
}
