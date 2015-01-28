namespace SolrNet {
    /// <summary>
    /// Contains the Fields to index along with the rich documents
    /// </summary>
    public class ExtractField {

        /// <summary>
        /// The name of the field to index
        /// </summary>
        public string FieldName { get; private set; }

        /// <summary>
        /// The value to index
        /// </summary>
        public string Value { get; private set; }

        /// <summary>
        /// Boost to apply to this field
        /// </summary>
        public string Boost { get; set; }

        /// <summary>
        /// Constructs a new ExtractField with required values
        /// </summary>
        /// <param name="fieldName">The name of the field to index</param>
        /// <param name="value">The value to index</param>
        public ExtractField(string fieldName, string value) {
            FieldName = fieldName;
            Value = value;
        }
    }
}