using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace SolrNet.Impl.ResponseParsers
{
    public class MlthMatchResponseParser<T> : ISolrMoreLikeThisHandlerResponseParser<T>
    {
        private readonly ISolrDocumentResponseParser<T> docParser;

        public MlthMatchResponseParser(ISolrDocumentResponseParser<T> docParser)
        {
            this.docParser = docParser;
        }

        public void Parse(XDocument xml, IAbstractSolrQueryResults<T> results)
        {
            if (results is ISolrMoreLikeThisQueryResults<T>)
            {
                this.Parse(xml, (ISolrMoreLikeThisQueryResults<T>)results);
            }
        }

        public void Parse(XDocument xml, ISolrMoreLikeThisQueryResults<T> results)
        {
            var resultNode = xml.Element("response").Elements("result").FirstOrDefault(e => (string)e.Attribute("name") == "match");

            if (resultNode == null)
            {
                results.Match = default(T);
                return;
            }

            results.Match = docParser.ParseResults(resultNode).FirstOrDefault();
        }
    }
}
