namespace SolrNet.Commands.Parameters
{
    /// <summary>
    /// Parameters to query collapse-expand
    /// </summary>
    public class CollapseExpandParameters
    {
        /// <summary>
        /// Field to group results by
        /// </summary>
        public string Field { get; set; }

        /// <summary>
        /// Used to expand the results 
        /// </summary>
        public bool Expand { get; set; }

        /// <summary>
        /// How to sort documents within a single group.
        /// </summary>
        public SortOrder OrderBy { get; set; }
    }
}
