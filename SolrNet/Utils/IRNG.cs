using System;

namespace SolrNet.Utils {
    /// <summary>
    /// Random number generator abstraction
    /// </summary>
	public interface IRNG {
        /// <summary>
        /// See <see cref="Random.Next()"/>
        /// </summary>
        /// <returns></returns>
		int Next();

        /// <summary>
        /// See <see cref="Random.Next(int,int)"/>
        /// </summary>
        /// <param name="minValue"></param>
        /// <param name="maxValue"></param>
        /// <returns></returns>
		int Next(int minValue, int maxValue);

        /// <summary>
        /// See <see cref="Random.Next(int)"/>
        /// </summary>
        /// <param name="maxValue"></param>
        /// <returns></returns>
		int Next(int maxValue);

        /// <summary>
        /// See <see cref="Random.NextDouble"/>
        /// </summary>
        /// <returns></returns>
		double NextDouble();

        /// <summary>
        /// See <see cref="Random.NextBytes"/>
        /// </summary>
        /// <param name="buffer"></param>
		void NextBytes(byte[] buffer);
	}
}