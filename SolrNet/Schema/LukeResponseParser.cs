using System.Xml.Linq;
using System.Xml.XPath;

namespace SolrNet.Schema {
    public class LukeResponseParser {


        /// <summary>
        /// Creates new SolrFields in the LukeResponse based on the /admin/luke response
        /// for those <see cref="SolrField"/>s which not occur in the <see cref="SolrSchema"/>.
        /// </summary>
        /// <param name="lukeResponse"></param>
        /// <param name="schema"></param>
        /// <returns></returns>
        public LukeResponse Parse(XDocument lukeResponse, SolrSchema schema)
        {
            var result = new LukeResponse();

            foreach (var fieldNode in lukeResponse.XPathSelectElements("/response/lst[@name='fields']/lst"))
            {
                string typeName = GetType(fieldNode);
                string dynamicBase = GetDynamicBase(fieldNode);

                var attributeFieldName = fieldNode.Attribute("name");
                if (attributeFieldName != null)
                {
                    string fieldName = attributeFieldName.Value;
                    bool typeExists = schema.SolrFieldTypes.Exists(x => x.Name.Equals(typeName)) && typeName != null;
                    bool fieldExists = schema.SolrFields.Exists(x => x.Name.Equals(fieldName));
                    if (typeExists && !fieldExists)
                    {
                        SolrFieldType solrFieldType = schema.SolrFieldTypes.Find(x => x.Name.Equals(typeName));
                        var field = new SolrField(fieldName, solrFieldType);
                        result.SolrFields.Add(field);
                    }

                }
            }

            return result;
        }

        private static string GetType(XElement fieldNode)
        {
            var elements = fieldNode.Elements("str");

            foreach (XElement xElement in elements)
            {
                var xAttribute = xElement.Attribute("name");
                if (xAttribute != null && xAttribute.Value.Equals("type"))
                {
                    return xElement.Value;
                }
            }
            return null;
        }

        private string GetDynamicBase(XElement fieldNode)
        {
            var elements = fieldNode.Elements("str");

            foreach (XElement xElement in elements)
            {
                var xAttribute = xElement.Attribute("name");
                if (xAttribute != null && xAttribute.Value.Equals("dynamicBase"))
                {
                    return xElement.Value;
                }
            }
            return null;
        }
    }
}
