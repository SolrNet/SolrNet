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
using System.Linq;
using SolrNet.Exceptions;

namespace SolrNet.Impl.QuerySerializers {
    public class AggregateQuerySerializer : ISolrQuerySerializer {
        private readonly ISolrQuerySerializer[] serializers;

        public AggregateQuerySerializer(ISolrQuerySerializer[] serializers) {
            this.serializers = serializers;
        }

        /// <inheritdoc />
        public bool CanHandleType(Type t) {
            return serializers.Any(s => s.CanHandleType(t));
        }

        /// <inheritdoc />
        public string Serialize(object q) {
            if (q == null)
                return string.Empty;
            var t = q.GetType();
            foreach (var s in serializers)
                if (s.CanHandleType(t))
                    return s.Serialize(q);
            throw new SolrNetException(string.Format("Couldn't serialize query '{0}' of type '{1}'", q, t));
        }
    }
}
