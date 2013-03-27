using System.Reflection;

namespace SolrNet {
    ///<summary>
    /// Represents a Solr field mapping.
    ///</summary>
    public class SolrFieldModel {

		public SolrFieldModel(PropertyInfo property, string fieldName, float? boost = null) {
			Property = property;
			FieldName = fieldName;
			Boost = boost;
		}

        /// <summary>
        /// Class property where the field value is stored
        /// </summary>
        public PropertyInfo Property { get; private set; }

        /// <summary>
        /// Named of the field in the Solr schema
        /// </summary>
		public string FieldName { get; private set; }

        /// <summary>
        /// Optional index-time field boosting
        /// </summary>
		public float? Boost { get; private set; }
    }
}