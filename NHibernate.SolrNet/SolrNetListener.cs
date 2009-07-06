#region license
// Copyright (c) 2007-2009 Mauricio Scheffer
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
using SolrNet;

namespace NHibernate.SolrNet {
    public class SolrNetListener<T> : IEvictEventListener, IAutoFlushEventListener, IFlushEventListener, IPostInsertEventListener, IPostDeleteEventListener, IPostUpdateEventListener where T : class {
        private readonly ISolrOperations<T> solr;
        private readonly IDictionary<ISession, List<T>> entitiesToAdd = new Dictionary<ISession, List<T>>();
        private readonly IDictionary<ISession, List<T>> entitiesToDelete = new Dictionary<ISession, List<T>>();
        private readonly object addLock = new object();
        private readonly object deleteLock = new object();

        public bool Commit { get; set; }

        public SolrNetListener(ISolrOperations<T> solr) {
            if (solr == null)
                throw new ArgumentNullException("solr");
            this.solr = solr;
        }

        private void Add(ISession s, T entity) {
            lock (addLock) {
                if (!entitiesToAdd.ContainsKey(s))
                    entitiesToAdd[s] = new List<T>();
                entitiesToAdd[s].Add(entity);
            }
        }

        private void Delete(ISession s, T entity) {
            lock (deleteLock) {
                if (!entitiesToDelete.ContainsKey(s))
                    entitiesToDelete[s] = new List<T>();
                entitiesToDelete[s].Add(entity);
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
            if (DeferAction(e.Session))
                Add(e.Session, entity);
            else
                solr.Add(entity);
        }


        public virtual void OnPostDelete(PostDeleteEvent e) {
            if (e.Entity.GetType() != typeof (T))
                return;
            if (DeferAction(e.Session))
                Delete(e.Session, (T) e.Entity);
            else
                solr.Delete((T) e.Entity);
        }

        public bool DoWithEntities(IDictionary<ISession, List<T>> entities, ISession s, Action<T> action) {
            var hasToDo = entities.ContainsKey(s);
            if (hasToDo)
                foreach (var i in entities[s])
                    action(i);
            entities.Remove(s);
            return hasToDo;
        }

        public void OnFlush(FlushEvent e) {
            OnFlushInternal(e);
        }

        public void OnFlushInternal(AbstractEvent e) {
            var added = DoWithEntities(entitiesToAdd, e.Session, d => solr.Add(d));
            var deleted = DoWithEntities(entitiesToDelete, e.Session, d => solr.Delete(d));
            if (Commit && (added || deleted))
                solr.Commit();
        }

        public void OnEvict(EvictEvent e) {
            entitiesToAdd.Remove(e.Session);
            entitiesToDelete.Remove(e.Session);
        }

        public void OnAutoFlush(AutoFlushEvent e) {
            OnFlushInternal(e);
        }
    }
}