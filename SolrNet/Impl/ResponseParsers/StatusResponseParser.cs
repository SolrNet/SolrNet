using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SolrNet.Impl.ResponseParsers
{
    public class StatusResponseParser<T> : ISolrResponseParser<T>
    {
        public void Parse( System.Xml.Linq.XDocument xml, SolrQueryResults<T> results ) {
        }

        public void Parse( System.Xml.Linq.XDocument xml, AbstractSolrQueryResults<T> results ) {
        }
    }
}
