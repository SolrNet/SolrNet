using System;

namespace SolrNet.Exceptions {
    /// <summary>
    /// No unique key found (either mapping or value) when one was required.
    /// </summary>
    public class NoUniqueKeyException : SolrNetException {
        private readonly Type _t;

        public Type Type {
            get { return _t; }
        }

        public NoUniqueKeyException(Type t) : this(t, string.Format("Type '{0}' has no unique key defined", t)) {}

        public NoUniqueKeyException(Type t, string message) : base(message) {
            _t = t;
        }
    }
}