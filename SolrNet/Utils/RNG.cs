using System;

namespace SolrNet.Utils {
	public class RNG : IRNG {
		private readonly Random r = new Random();

		public int Next() {
			return r.Next();
		}

		public int Next(int minValue, int maxValue) {
			return r.Next(minValue, maxValue);
		}

		public int Next(int maxValue) {
			return r.Next(maxValue);
		}

		public double NextDouble() {
			return r.NextDouble();
		}

		public void NextBytes(byte[] buffer) {
			r.NextBytes(buffer);
		}
	}
}