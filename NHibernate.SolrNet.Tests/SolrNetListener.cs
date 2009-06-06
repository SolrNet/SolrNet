using NHibernate.Event;
using SolrNet;

namespace NHibernate.SolrNet.Tests {
    public class SolrNetListener<T> : IPostInsertEventListener, IPostDeleteEventListener, IPostUpdateEventListener {
        private readonly ISolrOperations<T> solr;

        public SolrNetListener(ISolrOperations<T> solr) {
            this.solr = solr;
        }

        public virtual void OnPostInsert(PostInsertEvent e) {
            if (e.Entity.GetType() != typeof (T))
                return;
            solr.Add((T) e.Entity);
        }

        public virtual void OnPostDelete(PostDeleteEvent e) {
            if (e.Entity.GetType() != typeof (T))
                return;
            solr.Delete((T) e.Entity);
        }

        public virtual void OnPostUpdate(PostUpdateEvent e) {
            if (e.Entity.GetType() != typeof (T))
                return;
            solr.Add((T) e.Entity);
        }
    }
}