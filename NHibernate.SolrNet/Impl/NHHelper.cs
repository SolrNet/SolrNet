using System;
using System.Collections.Generic;
using NHibernate.Cfg;
using NHibernate.Event;
using SolrNet.Utils;

namespace NHibernate.SolrNet.Impl {
    public class NHHelper {
        private struct NHListenerInfo {
            public System.Type Intf;
            public ListenerType ListenerType;
            public Converter<EventListeners, Array> ListenerCollection;
        }

        private static readonly ICollection<NHListenerInfo> ListenerInfo = new[] {
            new NHListenerInfo {
                Intf = typeof (IEvictEventListener),
                ListenerType = ListenerType.Evict,
                ListenerCollection = l => l.EvictEventListeners,
            },
            new NHListenerInfo {
                Intf = typeof (IPostInsertEventListener),
                ListenerType = ListenerType.PostInsert,
                ListenerCollection = l => l.PostInsertEventListeners,
            },
            new NHListenerInfo {
                Intf = typeof (IPostDeleteEventListener),
                ListenerType = ListenerType.PostDelete,
                ListenerCollection = l => l.PostDeleteEventListeners,
            },
            new NHListenerInfo {
                Intf = typeof (IPostUpdateEventListener),
                ListenerType = ListenerType.PostUpdate,
                ListenerCollection = l => l.PostUpdateEventListeners,
            },
            new NHListenerInfo {
                Intf = typeof (IPreInsertEventListener),
                ListenerType = ListenerType.PreInsert,
                ListenerCollection = l => l.PreInsertEventListeners,
            },
            new NHListenerInfo {
                Intf = typeof (IPreDeleteEventListener),
                ListenerType = ListenerType.PreDelete,
                ListenerCollection = l => l.PreDeleteEventListeners,
            },
            new NHListenerInfo {
                Intf = typeof (IPreUpdateEventListener),
                ListenerType = ListenerType.PreUpdate,
                ListenerCollection = l => l.PreUpdateEventListeners,
            },
            new NHListenerInfo {
                Intf = typeof (ILoadEventListener),
                ListenerType = ListenerType.Load, // preload, postload?
                ListenerCollection = l => l.LoadEventListeners,
            },
            new NHListenerInfo {
                Intf = typeof (ILockEventListener),
                ListenerType = ListenerType.Lock,
                ListenerCollection = l => l.LockEventListeners,
            },
            new NHListenerInfo {
                Intf = typeof (IRefreshEventListener),
                ListenerType = ListenerType.Refresh,
                ListenerCollection = l => l.RefreshEventListeners,
            },
            new NHListenerInfo {
                Intf = typeof (IMergeEventListener),
                ListenerType = ListenerType.Merge,
                ListenerCollection = l => l.MergeEventListeners,
            },
            new NHListenerInfo {
                Intf = typeof (IFlushEventListener),
                ListenerType = ListenerType.Flush,
                ListenerCollection = l => l.FlushEventListeners,
            },
            new NHListenerInfo {
                Intf = typeof (IFlushEntityEventListener),
                ListenerType = ListenerType.FlushEntity,
                ListenerCollection = l => l.FlushEntityEventListeners,
            },
            new NHListenerInfo {
                Intf = typeof (IAutoFlushEventListener),
                ListenerType = ListenerType.Autoflush,
                ListenerCollection = l => l.AutoFlushEventListeners,
            },
        };


        public static void SetListener(Configuration config, object listener) {
            if (listener == null)
                throw new ArgumentNullException("listener");
            foreach (var intf in listener.GetType().GetInterfaces()) {
                var intf1 = intf;
                var listenerInfo = Func.Filter(ListenerInfo, i => i.Intf == intf1);
                foreach (var i in listenerInfo) {
                    var currentListeners = i.ListenerCollection(config.EventListeners);
                    config.SetListeners(i.ListenerType, AppendToArray(currentListeners, listener));
                }
            }
        }

        public static object[] AppendToArray(Array currentListeners, object listener) {
            var elemType = currentListeners.GetType().GetElementType();
            var newListeners = Array.CreateInstance(elemType, currentListeners.Length + 1);
            currentListeners.CopyTo(newListeners, 0);
            newListeners.SetValue(listener, currentListeners.Length);
            return (object[]) newListeners;
        }
    }
}