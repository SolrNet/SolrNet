using System.Collections;
using System.Collections.Generic;

namespace SolrNet.Utils {
	public interface IListRandomizer {
		//void Randomize(IList list);
		void Randomize<T>(IList<T> list);
	}
}