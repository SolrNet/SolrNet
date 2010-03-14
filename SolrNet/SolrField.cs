using System.Reflection;

namespace SolrNet
{
    ///<summary>
    /// Represents the Solr Field to be serialized into XML.
    ///</summary>
    public class SolrField
    {
        public PropertyInfo Property { get; set; }
        public string FieldName { get; set; }
        public int? Boost { get; set; }     
    }
}