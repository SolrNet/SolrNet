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
using System.Collections.Generic;
using SolrNet.Exceptions;
using SolrNet.Utils;

namespace SolrNet.Impl.FacetQuerySerializers {
    public class AggregateFacetQuerySerializer : ISolrFacetQuerySerializer {
        private readonly ISolrFacetQuerySerializer[] serializers;

        public AggregateFacetQuerySerializer(ISolrFacetQuerySerializer[] serializers) {
            this.serializers = serializers;
        }

        public bool CanHandleType(Type t) {
            return Func.Any(serializers, s => s.CanHandleType(t));
        }

        public IEnumerable<KeyValuePair<string, string>> Serialize(object q) {
            if (q == null)
                yield break;
            var t = q.GetType();
            foreach (var s in serializers)
                if (s.CanHandleType(t)) {
                    foreach (var k in s.Serialize(q))
                        yield return k;
                    yield break;
                }
            throw new SolrNetException(string.Format("Couldn't serialize facet query '{0}' of type '{1}'", q, t));
        }
    }
}