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

namespace SolrNet {
    /// <summary>
    /// Abstract Solr query, used to define operator overloading
    /// </summary>
    public abstract class AbstractSolrQuery : ISolrQuery {
        /// <summary>
        /// Negates this query
        /// </summary>
        /// <returns></returns>
        public AbstractSolrQuery Not() {
            return new SolrNotQuery(this);
        }

        /// <summary>
        /// Negates this query
        /// </summary>
        /// <returns></returns>
        public AbstractSolrQuery Required() {
            return new SolrRequiredQuery(this);
        }

        /// <summary>
        /// Boosts this query
        /// </summary>
        /// <param name="factor"></param>
        /// <returns></returns>
        public AbstractSolrQuery Boost(double factor) {
            return new SolrQueryBoost(this, factor);
        }

        public static AbstractSolrQuery operator & (AbstractSolrQuery a, AbstractSolrQuery b) {
            if (a == null)
                throw new ArgumentNullException("a");
            if (b == null)
                throw new ArgumentNullException("b");
            return new SolrMultipleCriteriaQuery(new[] {a, b}, "AND");
        }

        public static AbstractSolrQuery operator | (AbstractSolrQuery a, AbstractSolrQuery b) {
            if (a == null)
                throw new ArgumentNullException("a");
            if (b == null)
                throw new ArgumentNullException("b");
            return new SolrMultipleCriteriaQuery(new[] { a, b }, "OR");
        }

        public static AbstractSolrQuery operator + (AbstractSolrQuery a, AbstractSolrQuery b) {
            if (a == null)
                throw new ArgumentNullException("a");
            if (b == null)
                throw new ArgumentNullException("b");
            return new SolrMultipleCriteriaQuery(new[] { a, b });
        }

        public static AbstractSolrQuery operator - (AbstractSolrQuery a, AbstractSolrQuery b) {
            if (a == null)
                throw new ArgumentNullException("a");
            if (b == null)
                throw new ArgumentNullException("b");
            return new SolrMultipleCriteriaQuery(new[] { a, b.Not() });
        }

        public static bool operator false (AbstractSolrQuery a) {
            return false;
        }

        public static bool operator true (AbstractSolrQuery a) {
            return false;
        }

        public static AbstractSolrQuery operator !(AbstractSolrQuery a) {
            if (a == null)
                throw new ArgumentNullException("a");
            return a.Not();
        }
    }
}