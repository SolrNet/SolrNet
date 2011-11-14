using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NHibernate.SolrNet.Impl {
    public class WeakDictionary<K, V> : IDictionary<K, V> {
        private readonly IList<WeakReference> store = new List<WeakReference>();

        public IEnumerator<KeyValuePair<K, V>> GetEnumerator() {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        public void Add(KeyValuePair<K, V> item) {
            // doesn't look for duplicates
            store.Add(new WeakReference(new KeyValuePair<K,V>(item.Key, item.Value)));
        }

        public void Clear() {
            store.Clear();
        }

        public bool Contains(KeyValuePair<K, V> item) {
            throw new NotImplementedException();
        }

        public void CopyTo(KeyValuePair<K, V>[] array, int arrayIndex) {
            throw new NotImplementedException();
        }

        public bool Remove(KeyValuePair<K, V> item) {
            throw new NotImplementedException();
        }

        public int Count {
            get { return store.Where(x => x.IsAlive).Count(); }
        }

        public bool IsReadOnly {
            get { return false; }
        }

        private IEnumerable<KeyValuePair<V,int>> TryGet(K key) {
            return store
                .Select((x,i) => new {Ref = x,i})
                .Where(x => x.Ref.IsAlive)
                .Select(x => new {Ref = (KeyValuePair<K,V>) x.Ref.Target, x.i})
                .Where(x => x.Ref.Key.GetHashCode() == key.GetHashCode())
                .Select(x => new KeyValuePair<V,int>(x.Ref.Value, x.i));
        }

        public bool ContainsKey(K key) {
            return TryGet(key).Any();
        }

        public void Add(K key, V value) {
            Add(new KeyValuePair<K, V>(key, value));
        }

        public bool Remove(K key) {
            var v = TryGet(key).ToArray();
            if (v.Length == 0)
                return false;
            store.RemoveAt(v[0].Value);
            return true;
        }

        public bool TryGetValue(K key, out V value) {
            throw new NotImplementedException();
        }

        public V this[K key] {
            get { return TryGet(key).First().Key; }
            set { Add(key, value); }
        }

        public ICollection<K> Keys {
            get { throw new NotImplementedException(); }
        }

        public ICollection<V> Values {
            get { throw new NotImplementedException(); }
        }
    }
}