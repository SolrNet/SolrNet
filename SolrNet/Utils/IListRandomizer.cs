using System.Collections;
using System.Collections.Generic;

namespace SolrNet.Utils {
    /// <summary>
    /// Randomizes a list in place
    /// </summary>
	public interface IListRandomizer {
        /// <summary>
        /// Randomizes a list in place
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
		void Randomize<T>(IList<T> list);
	}
}