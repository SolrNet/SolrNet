using NHibernate.Event;
using System;

namespace NHibernate.SolrNet.Tests {
    public class MPostInsertEventListener : IPostInsertEventListener {
        public void OnPostInsert(PostInsertEvent @event) {
            throw new NotImplementedException();
        }
    }
}