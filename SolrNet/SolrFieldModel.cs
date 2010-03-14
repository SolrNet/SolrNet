using System.Reflection;

namespace SolrNet {
    ///<summary>
    /// Represents the Solr Field to be serialized into XML.
    ///</summary>
    public class SolrFieldModel {
        /// <summary>
        /// Class property where the field value is stored
        /// </summary>
        public PropertyInfo Property { get; set; }

        /// <summary>
        /// Named of the field in the Solr schema
        /// </summary>
        public string FieldName { get; set; }

        /// <summary>
        /// Optional index-time field boosting
        /// </summary>
        public int? Boost { get; set; }
    }
}