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
using System.Linq;

namespace SolrNet.Impl.QuerySerializers {
    public class QueryInListSerializer : SingleTypeQuerySerializer<SolrQueryInList> {
        private readonly ISolrQuerySerializer serializer;

        public QueryInListSerializer(ISolrQuerySerializer serializer) {
            this.serializer = serializer;
        }

        public override string Serialize(SolrQueryInList q) {
            if (string.IsNullOrEmpty(q.FieldName) || q.List == null || !q.List.Any())
                return null;

            var array = q.List.Select(x =>"(" + QueryByFieldSerializer.Quote(x) + ")").ToArray();
            return "(" + serializer.Serialize(new SolrQueryByField(QueryByFieldSerializer.EscapeSpaces(q.FieldName),string.Join(" OR ",array)){Quoted = false}) + ")";
        }
    }
}