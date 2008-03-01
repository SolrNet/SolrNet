using System.Collections;
using System.Collections.Generic;

namespace SolrNet {
	public class SolrQueryResults<T> : ISolrQueryResults<T> where T : ISolrDocument {
		private readonly IList<T> innerList = new List<T>();
		///<summary>
		///Determines the index of a specific item in the <see cref="T:System.Collections.Generic.IList`1"></see>.
		///</summary>
		///
		///<returns>
		///The index of item if found in the list; otherwise, -1.
		///</returns>
		///
		///<param name="item">The object to locate in the <see cref="T:System.Collections.Generic.IList`1"></see>.</param>
		public int IndexOf(T item) {
			return innerList.IndexOf(item);
		}

		///<summary>
		///Inserts an item to the <see cref="T:System.Collections.Generic.IList`1"></see> at the specified index.
		///</summary>
		///
		///<param name="item">The object to insert into the <see cref="T:System.Collections.Generic.IList`1"></see>.</param>
		///<param name="index">The zero-based index at which item should be inserted.</param>
		///<exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.IList`1"></see> is read-only.</exception>
		///<exception cref="T:System.ArgumentOutOfRangeException">index is not a valid index in the <see cref="T:System.Collections.Generic.IList`1"></see>.</exception>
		public void Insert(int index, T item) {
			innerList.Insert(index, item);
		}

		///<summary>
		///Removes the <see cref="T:System.Collections.Generic.IList`1"></see> item at the specified index.
		///</summary>
		///
		///<param name="index">The zero-based index of the item to remove.</param>
		///<exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.IList`1"></see> is read-only.</exception>
		///<exception cref="T:System.ArgumentOutOfRangeException">index is not a valid index in the <see cref="T:System.Collections.Generic.IList`1"></see>.</exception>
		public void RemoveAt(int index) {
			innerList.RemoveAt(index);
		}

		///<summary>
		///Gets or sets the element at the specified index.
		///</summary>
		///
		///<returns>
		///The element at the specified index.
		///</returns>
		///
		///<param name="index">The zero-based index of the element to get or set.</param>
		///<exception cref="T:System.ArgumentOutOfRangeException">index is not a valid index in the <see cref="T:System.Collections.Generic.IList`1"></see>.</exception>
		///<exception cref="T:System.NotSupportedException">The property is set and the <see cref="T:System.Collections.Generic.IList`1"></see> is read-only.</exception>
		public T this[int index] {
			get { return innerList[index]; }
			set { innerList[index] = value; }
		}

		///<summary>
		///Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1"></see>.
		///</summary>
		///
		///<param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1"></see>.</param>
		///<exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"></see> is read-only.</exception>
		public void Add(T item) {
			innerList.Add(item);
		}

		///<summary>
		///Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1"></see>.
		///</summary>
		///
		///<exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"></see> is read-only. </exception>
		public void Clear() {
			innerList.Clear();
		}

		///<summary>
		///Determines whether the <see cref="T:System.Collections.Generic.ICollection`1"></see> contains a specific value.
		///</summary>
		///
		///<returns>
		///true if item is found in the <see cref="T:System.Collections.Generic.ICollection`1"></see>; otherwise, false.
		///</returns>
		///
		///<param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1"></see>.</param>
		public bool Contains(T item) {
			return innerList.Contains(item);
		}

		///<summary>
		///Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1"></see> to an <see cref="T:System.Array"></see>, starting at a particular <see cref="T:System.Array"></see> index.
		///</summary>
		///
		///<param name="array">The one-dimensional <see cref="T:System.Array"></see> that is the destination of the elements copied from <see cref="T:System.Collections.Generic.ICollection`1"></see>. The <see cref="T:System.Array"></see> must have zero-based indexing.</param>
		///<param name="arrayIndex">The zero-based index in array at which copying begins.</param>
		///<exception cref="T:System.ArgumentOutOfRangeException">arrayIndex is less than 0.</exception>
		///<exception cref="T:System.ArgumentNullException">array is null.</exception>
		///<exception cref="T:System.ArgumentException">array is multidimensional.-or-arrayIndex is equal to or greater than the length of array.-or-The number of elements in the source <see cref="T:System.Collections.Generic.ICollection`1"></see> is greater than the available space from arrayIndex to the end of the destination array.-or-Type T cannot be cast automatically to the type of the destination array.</exception>
		public void CopyTo(T[] array, int arrayIndex) {
			innerList.CopyTo(array, arrayIndex);
		}

		///<summary>
		///Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1"></see>.
		///</summary>
		///
		///<returns>
		///true if item was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1"></see>; otherwise, false. This method also returns false if item is not found in the original <see cref="T:System.Collections.Generic.ICollection`1"></see>.
		///</returns>
		///
		///<param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1"></see>.</param>
		///<exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"></see> is read-only.</exception>
		public bool Remove(T item) {
			return innerList.Remove(item);
		}

		///<summary>
		///Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"></see>.
		///</summary>
		///
		///<returns>
		///The number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"></see>.
		///</returns>
		///
		public int Count {
			get { return innerList.Count; }
		}

		///<summary>
		///Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1"></see> is read-only.
		///</summary>
		///
		///<returns>
		///true if the <see cref="T:System.Collections.Generic.ICollection`1"></see> is read-only; otherwise, false.
		///</returns>
		///
		public bool IsReadOnly {
			get { return innerList.IsReadOnly; }
		}

		///<summary>
		///Returns an enumerator that iterates through a collection.
		///</summary>
		///
		///<returns>
		///An <see cref="T:System.Collections.IEnumerator"></see> object that can be used to iterate through the collection.
		///</returns>
		///<filterpriority>2</filterpriority>
		public IEnumerator GetEnumerator() {
			return ((IEnumerable) innerList).GetEnumerator();
		}

		public int NumFound { get; set; }

		public IDictionary<string, int> FacetQueries { get; set; }
		public IDictionary<string, int> FacetFields { get; set; }

		///<summary>
		///Returns an enumerator that iterates through the collection.
		///</summary>
		///
		///<returns>
		///A <see cref="T:System.Collections.Generic.IEnumerator`1"></see> that can be used to iterate through the collection.
		///</returns>
		///<filterpriority>1</filterpriority>
		IEnumerator<T> IEnumerable<T>.GetEnumerator() {
			return innerList.GetEnumerator();
		}
	}
}