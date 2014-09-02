namespace SolrNet.Commands.Parameters
{
    /// <summary>
    /// Parameters to query collapse-expand
    /// </summary>
    public class CollapseExpandParameters 
    {
        private readonly string field;
        private readonly bool expand;
        private readonly SortOrder orderBy;

        /// <summary>
        /// Field to group results by
        /// </summary>
        public string Field 
        {
            get { return field; }
        }

        /// <summary>
        /// Used to expand the results 
        /// </summary>
        public bool Expand 
        {
            get { return expand; }
        }

        /// <summary>
        /// How to sort documents within a single group.
        /// </summary>
        public SortOrder OrderBy 
        {
            get { return orderBy; }
        }

        /// <summary>
        /// CollapseExplandParameters initializer
        /// </summary>
        public CollapseExpandParameters(string field, bool expand, SortOrder orderBy) 
        {
            this.field = field;
            this.expand = expand;
            this.orderBy = orderBy;
        }
    }
}
