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

<<<<<<< HEAD
using System;
using System.Globalization;
using SolrNet.Impl.FieldSerializers;
=======
using SolrNet.Impl;
>>>>>>> 92e142f4b04ba0a2a3db7b39dfc16b3e0f190002

namespace SolrNet {
    /// <summary>
    /// Queries a field for a range
    /// </summary>
    /// <typeparam name="RT"></typeparam>
<<<<<<< HEAD
	public class SolrQueryByRange<RT> : AbstractSolrQuery {
        private readonly DateTimeFieldSerializer dateSerializer = new DateTimeFieldSerializer();

		private readonly string q;
		public SolrQueryByRange(string fieldName, RT from, RT to) : this(fieldName, from, to, true) {}

		public SolrQueryByRange(string fieldName, RT from, RT to, bool inclusive) {
			q = "$field:$ii$from TO $to$if"
				.Replace("$field", fieldName)
				.Replace("$ii", inclusive ? "[" : "{")
				.Replace("$if", inclusive ? "]" : "}")
				.Replace("$from", Serialize(from))
				.Replace("$to", Serialize(to));
		}

        private string Serialize(RT o) {
            if (typeof(RT) == typeof(DateTime))
                return dateSerializer.SerializeDate((DateTime)(object)o);
            return Convert.ToString(o, CultureInfo.InvariantCulture);
=======
    public class SolrQueryByRange<RT> : AbstractSolrQuery, ISolrQueryByRange {
        private readonly string fieldName;
        private readonly RT from;
        private readonly RT to;
        private readonly bool inclusive;

        public SolrQueryByRange(string fieldName, RT from, RT to) : this(fieldName, from, to, true) {}

        public SolrQueryByRange(string fieldName, RT @from, RT to, bool inclusive) {
            this.fieldName = fieldName;
            this.from = from;
            this.to = to;
            this.inclusive = inclusive;
        }

        public string FieldName {
            get { return fieldName; }
        }

        public RT From {
            get { return from; }
>>>>>>> 92e142f4b04ba0a2a3db7b39dfc16b3e0f190002
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

        public bool Inclusive {
            get { return inclusive; }
        }
    }
}