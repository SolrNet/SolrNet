#region license
// Copyright (c) 2007-2010 Mauricio Scheffer
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//      http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System;
using System.Collections.Generic;
using NHibernate.Event;
using NHibernate.Util;
using SolrNet;

namespace NHibernate.SolrNet.Impl {
    /// <summary>
    /// NHibernate event listener for updating Solr index
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SolrNetListener<T> : IListenerSettings, IAutoFlushEventListener, IFlushEventListener, IPostInsertEventListener, IPostDeleteEventListener, IPostUpdateEventListener where T : class {
        private readonly ISolrOperations<T> solr;
        private readonly WeakHashtable entitiesToAdd = new WeakHashtable();
        private readonly WeakHashtable entitiesToDelete = new WeakHashtable();

        /// <summary>
        /// Automatically commit Solr after each update
        /// </summary>
        public bool Commit { get; set; }

        /// <summary>
        /// Gets or sets the parameters to use when adding a document to the index.
        /// </summary>
        /// <value>The parameters to use when adding a document to the index.</value>
        public AddParameters AddParameters { get; set; }

        public SolrNetListener(ISolrOperations<T> solr) {
            if (solr == null)
                throw new ArgumentNullException("solr");
            this.solr = solr;
        }

        private void Add(ITransaction s, T entity) {
            lock (entitiesToAdd.SyncRoot) {
                if (!entitiesToAdd.Contains(s))
                    entitiesToAdd[s] = new List<T>();
                var l = ((IList<T>)entitiesToAdd[s]);
                if (!l.Contains(entity))
                    l.Add(entity);
            }
        }

        private void Delete(ITransaction s, T entity) {
            lock (entitiesToDelete.SyncRoot) {
                if (!entitiesToDelete.Contains(s))
                    entitiesToDelete[s] = new List<T>();
                var l = ((IList<T>)entitiesToAdd[s]);
                if (!l.Contains(entity))
                    l.Add(entity);
            }
        }

        public virtual void OnPostInsert(PostInsertEvent e) {
            UpdateInternal(e, e.Entity as T);
        }

        public virtual void OnPostUpdate(PostUpdateEvent e) {
            UpdateInternal(e, e.Entity as T);
        }

        private readonly List<FlushMode> deferFlushModes = new List<FlushMode> {
            FlushMode.Commit, 
            FlushMode.Never,
        };

        public bool DeferAction(IEventSource e) {
            if (e.TransactionInProgress)
                return true;
            var s = (ISession) e;
            return deferFlushModes.Contains(s.FlushMode);
        }

        public void UpdateInternal(AbstractEvent e, T entity) {
            if (entity == null)
                return;
            if (entity.GetType() != typeof(T)) // strict check for type, e.g. no subtypes
                return;
            if (DeferAction(e.Session))
                Add(e.Session.Transaction, entity);
            else {
                solr.Add(entity, AddParameters);
                if (Commit)
                    solr.Commit();
            }
        }


        public virtual void OnPostDelete(PostDeleteEvent e) {
            if (e.Entity.GetType() != typeof (T))
                return;
            if (DeferAction(e.Session))
                Delete(e.Session.Transaction, (T) e.Entity);
            else {
                solr.Delete((T)e.Entity);
                if (Commit)
                    solr.Commit();
            }
        }

        public bool DoWithEntities(WeakHashtable entities, ITransaction s, Action<T> action) {
            lock (entities.SyncRoot) {
                var hasToDo = entities.Contains(s);
                if (hasToDo)
                    foreach (var i in (IList<T>)entities[s])
                        action(i);
                entities.Remove(s);
                return hasToDo;
            }
        }

        public void OnFlush(FlushEvent e) {
            OnFlushInternal(e);
        }

        public void OnFlushInternal(AbstractEvent e) {
            var added = DoWithEntities(entitiesToAdd, e.Session.Transaction, d => solr.Add(d, AddParameters));
            var deleted = DoWithEntities(entitiesToDelete, e.Session.Transaction, d => solr.Delete(d));
            if (Commit && (added || deleted))
                solr.Commit();
        }

        public void OnAutoFlush(AutoFlushEvent e) {
            OnFlushInternal(e);
        }
    }
}