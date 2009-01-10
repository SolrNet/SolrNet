using System;
using System.Collections;
using System.Collections.Generic;

namespace SolrNet.Utils {
	/// <summary>
	/// Randomizes a list in place
	/// Adapted from http://bytes.com/forum/post415826-2.html
	/// </summary>
	public class ListRandomizer : IListRandomizer {
		public IRNG Rng { get; set; }

		public ListRandomizer() {
			Rng = new RNG();
		}

		//public void Randomize(IList list) {
		//  for (int i = list.Count - 1; i > 0; i--) {
		//    int swapIndex = Rng.Next(i + 1);
		//    if (swapIndex != i) {
		//      var tmp = list[swapIndex];
		//      list[swapIndex] = list[i];
		//      list[i] = tmp;
		//    }
		//  }
		//}

		public void Randomize<T>(IList<T> list) {
			for (int i = list.Count - 1; i > 0; i--) {
				int swapIndex = Rng.Next(i + 1);
				if (swapIndex != i) {
					var tmp = list[swapIndex];
					list[swapIndex] = list[i];
					list[i] = tmp;
				}
			}
		}

	}
}