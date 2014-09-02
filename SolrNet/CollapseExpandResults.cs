using System.Collections.Generic;

namespace SolrNet
{
    /// <summary>
    /// Collapse/expand results model
    /// </summary>
    public class CollapseExpandResults<T> 
    {
        private readonly ICollection<Group<T>> groups;

        /// <summary>
        /// Grouped documents 
        /// </summary>
        public ICollection<Group<T>> Groups 
        {
            get { return groups; }
        }

        /// <summary>
        /// Constructor for CollapseExpandResults
        /// </summary>
        public CollapseExpandResults(ICollection<Group<T>> groups) 
        {
            this.groups = groups;
        }
    }
}
