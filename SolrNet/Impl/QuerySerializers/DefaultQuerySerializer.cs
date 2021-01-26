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

namespace SolrNet.Impl.QuerySerializers {
    public class DefaultQuerySerializer : ISolrQuerySerializer {
        private readonly AggregateQuerySerializer serializer;

        public DefaultQuerySerializer(ISolrFieldSerializer fieldSerializer) {
            serializer = new AggregateQuerySerializer(new ISolrQuerySerializer[] {
                new QueryByFieldSerializer(),
                new QueryByFieldRegexSerializer(),
                new LocalParamsSerializer(this),
                new BoostQuerySerializer(this),
                new ConstantScoreQuerySerializer(this),
                new HasValueQuerySerializer(this),
                new NotQuerySerializer(this),
                new RequiredQuerySerializer(this),
                new QueryInListSerializer(this),
                new NullableDateTimeRangeQuerySerializer(fieldSerializer),
                new DateTimeRangeQuerySerializer(fieldSerializer),
                new RangeQuerySerializer(fieldSerializer),
                new MultipleCriteriaQuerySerializer(this),
                new SelfSerializingQuerySerializer(),
            });
        }

        /// <inheritdoc />
        public bool CanHandleType(Type t) {
            return serializer.CanHandleType(t);
        }

        /// <inheritdoc />
        public string Serialize(object q) {
            return serializer.Serialize(q);
        }
    }
}
