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
using System.Globalization;
using System.Linq;

namespace SolrNet.Impl.QuerySerializers {
    public class RangeQuerySerializer : ISolrQuerySerializer {
        private readonly ISolrFieldSerializer fieldSerializer;

        public RangeQuerySerializer(ISolrFieldSerializer fieldSerializer) {
            this.fieldSerializer = fieldSerializer;
        }

        /// <inheritdoc />
        public bool CanHandleType(Type t) {
            return typeof (ISolrQueryByRange).IsAssignableFrom(t);
        }

        public static string BuildRange(string fieldName, string @from, string @to, bool inclusive) {
            return BuildRange(fieldName, @from, @to, inclusive, inclusive);
        }

        public static string BuildRange(string fieldName, string @from, string @to, bool inclusiveFrom, bool inclusiveTo) {
            return "$field:$ii$from TO $to$if"
                            .Replace("$field", QueryByFieldSerializer.EscapeSpaces(fieldName))
                            .Replace("$ii", inclusiveFrom ? "[" : "{")
                            .Replace("$if", inclusiveTo ? "]" : "}")
                            .Replace("$from", @from)
                            .Replace("$to", to);
        }

        public string SerializeValue(object o) {
            if (o == null)
                return "*";
            return fieldSerializer.Serialize(o).First().FieldValue;
        }

        /// <inheritdoc />
        public string Serialize(object q) {
            var query = (ISolrQueryByRange) q;
            return BuildRange(query.FieldName, 
                SerializeValue(query.From),
                SerializeValue(query.To),
                query.InclusiveFrom,
                query.InclusiveTo
                );
        }
    }
}
