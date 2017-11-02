using System;
using System.Xml.Linq;
using SolrNet.Impl;

namespace SolrNet.Tests.Mocks {
    public class MSolrFieldParser : ISolrFieldParser {
        public Func<string, bool> canHandleSolrType;
        public Func<Type, bool> canHandleType;
        public Func<XElement, Type, object> parse;

        public bool CanHandleSolrType(string solrType) {
            return canHandleSolrType(solrType);
        }

        public bool CanHandleType(Type t) {
            return canHandleType(t);
        }

        public object Parse(XElement field, Type t) {
            return parse(field, t);
        }
    }
}