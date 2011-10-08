using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SolrNet.Impl;

namespace SolrNet.Impl.ResponseParsers
{
    public abstract class AbstractResponseParser<T> : ISolrAbstractResponseParser<T>
    {
        public abstract void Parse(System.Xml.Linq.XDocument xml, IAbstractSolrQueryResults<T> results);
    }
}
