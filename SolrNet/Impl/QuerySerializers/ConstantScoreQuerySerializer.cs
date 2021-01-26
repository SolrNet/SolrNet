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

namespace SolrNet.Impl.QuerySerializers {
    public class ConstantScoreQuerySerializer : SingleTypeQuerySerializer<SolrConstantScoreQuery> {
        private readonly ISolrQuerySerializer serializer;

        public ConstantScoreQuerySerializer(ISolrQuerySerializer serializer) {
            this.serializer = serializer;
        }

        /// <inheritdoc />
        public override string Serialize(SolrConstantScoreQuery q) {
            return string.Format("({0})^={1}", serializer.Serialize(q.Query), q.Score.ToString(CultureInfo.InvariantCulture.NumberFormat));
        }
    }
}
