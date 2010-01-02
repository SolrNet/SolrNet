#region license
// Copyright (c) 2007-2010 Mauricio Scheffer
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