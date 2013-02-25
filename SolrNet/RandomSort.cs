﻿#region license
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

namespace SolrNet
{
    /// <summary>
    /// Random sorting of results
    /// Requires Solr 1.3+
    /// </summary>
    public class RandomSort : SortOrder
    {
        private readonly static Random Rnd = new Random();
        private const string Separator = "_";

        /// <summary>
        /// Random sorting with random seed
        /// </summary>
        /// <param name="fieldName">Random sorting field as defined in schema.xml</param>
        public RandomSort(string fieldName) : base(fieldName + Separator + Rnd.Next()) { }

        /// <summary>
        /// Random sorting with random seed, with specified order
        /// </summary>
        /// <param name="fieldName">Random sorting field as defined in schema.xml</param>
        /// <param name="order">Sort order (asc/desc)</param>
        public RandomSort(string fieldName, Order order) : base(fieldName + Separator + Rnd.Next(), order) { }

        /// <summary>
        /// Random sorting with specified seed
        /// </summary>
        /// <param name="fieldName">Random sorting field as defined in schema.xml</param>
        /// <param name="seed">Random seed</param>
        public RandomSort(string fieldName, string seed) : base(fieldName + Separator + seed) { }

        /// <summary>
        /// Random sorting with specified seed, with specified order
        /// </summary>
        /// <param name="fieldName">Random sorting field as defined in schema.xml</param>
        /// <param name="seed">Random seed</param>
        /// <param name="order">Sort order (asc/desc)</param>
        public RandomSort(string fieldName, string seed, Order order) : base(fieldName + Separator + seed, order) { }
    }
}