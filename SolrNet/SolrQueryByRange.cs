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
using SolrNet.Impl;

namespace SolrNet {
    /// <summary>
    /// Queries a field for a range
    /// </summary>
    /// <typeparam name="RT"></typeparam>
    public class SolrQueryByRange<RT> : AbstractSolrQuery, ISolrQueryByRange {
        private readonly string fieldName;
        private readonly RT from;
        private readonly RT to;
        private readonly bool inclusiveFrom;
        private readonly bool inclusiveTo;

        public SolrQueryByRange(string fieldName, RT from, RT to) : this(fieldName, from, to, true) {}

        public SolrQueryByRange(string fieldName, RT @from, RT to, bool inclusive) : this(fieldName, from, to, inclusive, inclusive) {}

        public SolrQueryByRange(string fieldName, RT @from, RT to, bool inclusiveFrom, bool inclusiveTo) {
            this.fieldName = fieldName;
            this.from = from;
            this.to = to;
            this.inclusiveFrom = inclusiveFrom;
            this.inclusiveTo = inclusiveTo;
        }

        public string FieldName {
            get { return fieldName; }
        }

        public RT From {
            get { return from; }
        }

        object ISolrQueryByRange.From {
            get { return from; }
        }

        public RT To {
            get { return to; }
        }

        object ISolrQueryByRange.To {
            get { return to; }
        }

        [Obsolete("Use InclusiveFrom and InclusiveTo", false)]
        public bool Inclusive {
            get { return inclusiveFrom && inclusiveTo; }
        }

        /// <summary>
        /// Is lower bound <see cref="From"/> inclusive
        /// </summary>
        public bool InclusiveFrom {
            get { return inclusiveFrom; }
        }

        /// <summary>
        /// Is upper bound <see cref="To"/> inclusive
        /// </summary>
        public bool InclusiveTo {
            get { return inclusiveTo; }
        }

    }
}