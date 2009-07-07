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
using Environment=NHibernate.Cfg.Environment;

namespace NHibernate.SolrNet.Tests {
    public static class ConfigurationExtensions {
        private static readonly Dictionary<System.Type, ListenerType[]> listenerDict = new Dictionary<System.Type, ListenerType[]> {
            {typeof (IEvictEventListener), new[] {ListenerType.Evict}},
            {typeof (IPostInsertEventListener), new[] {ListenerType.PostInsert, ListenerType.PostCommitInsert}},
            {typeof (IPostDeleteEventListener), new[] {ListenerType.PostDelete, ListenerType.PostCommitDelete}},
            {typeof (IPostUpdateEventListener), new[] {ListenerType.PostUpdate, ListenerType.PostCommitUpdate}},
            {typeof (IPreInsertEventListener), new[] {ListenerType.PreInsert}},
            {typeof (IPreDeleteEventListener), new[] {ListenerType.PreDelete}},
            {typeof (IPreUpdateEventListener), new[] {ListenerType.PreUpdate}},
            {typeof (ILoadEventListener), new[] {ListenerType.Load}},
            {typeof (ILockEventListener), new[] {ListenerType.Lock}},
            {typeof (IRefreshEventListener), new[] {ListenerType.Refresh}},
            {typeof (IMergeEventListener), new[] {ListenerType.Merge}},
            {typeof (IFlushEventListener), new[] {ListenerType.Flush}},
            {typeof (IFlushEntityEventListener), new[] {ListenerType.FlushEntity}},
            {typeof (IAutoFlushEventListener), new[] {ListenerType.Autoflush}},
        };

        public static void SetListener(this Cfg.Configuration config, object listener) {
            if (listener == null)
                throw new ArgumentNullException("listener");
            foreach (var intf in listener.GetType().GetInterfaces())
                if (listenerDict.ContainsKey(intf))
                    foreach (var t in listenerDict[intf])
                        config.SetListener(t, listener);
        }

        public static Configuration GetNhConfig() {
            var nhConfig = new Configuration {
                Properties = new Dictionary<string, string> {
                    {Environment.ConnectionProvider, "NHibernate.Connection.DriverConnectionProvider"},
                    {Environment.ConnectionDriver, "NHibernate.Driver.SQLite20Driver"},
                    {Environment.Dialect, "NHibernate.Dialect.SQLiteDialect"},
                    {Environment.ConnectionString, "Data Source=test.db;Version=3;New=True;"},
                }
            };
            nhConfig.Register(typeof (Entity));
            return nhConfig;
        }
    }
}