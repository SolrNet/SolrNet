// 
namespace SolrNet {
    /// <summary>
    /// Queries a field for a value
    /// </summary>
    public class SolrQueryByFieldRegex : AbstractSolrQuery
    {
        private readonly string fieldName;
        private readonly string expression;

        /// <summary>
        /// Queries a field based on a regular expression
        /// </summary>
        /// <param name="fieldName">Field name</param>
        /// <param name="regularExpression">The regular expression.</param>
        public SolrQueryByFieldRegex(string fieldName, string regularExpression)
        {
            this.fieldName = fieldName;
            this.expression = regularExpression;
        }

        /// <summary>
        /// Gets the name of the field.
        /// </summary>
        /// <value>
        /// The name of the field.
        /// </value>
        public string FieldName
        {
            get { return fieldName; }
        }

        /// <summary>
        /// Gets the regular expression.
        /// </summary>
        /// <value>
        /// The regular expression to be used.
        /// </value>
        public string Expression
        {
            get { return expression; }
        }
    }
}