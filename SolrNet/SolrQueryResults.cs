using System;
using System.Collections;
using System.Collections.Generic;

namespace SolrNet.Tests {
	public class SolrQueryResults<T> : ISolrQueryResults<T> {
		public IEnumerator<T> GetEnumerator() {
			throw new NotImplementedException();
		}

		IEnumerator IEnumerable.GetEnumerator() {
			throw new NotImplementedException();
		}

		///<summary>
		///Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1"></see>.
		///</summary>
		///
		///<param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1"></see>.</param>
		///<exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"></see> is read-only.</exception>
		public void Add(T item) {
			throw new NotImplementedException();
		}

		///<summary>
		///Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1"></see>.
		///</summary>
		///
		///<exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"></see> is read-only. </exception>
		public void Clear() {
			throw new NotImplementedException();
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
			throw new NotImplementedException();
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
			throw new NotImplementedException();
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
			throw new NotImplementedException();
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
			get { throw new NotImplementedException(); }
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
			get { throw new NotImplementedException(); }
		}
	}
}