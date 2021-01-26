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
using System.Collections.Generic;
using System.Globalization;

namespace SolrNet.Impl.FieldSerializers {
    /// <summary>
    /// Serializes objects that implement <see cref="IFormattable"/>
    /// </summary>
    public class FormattableFieldSerializer : ISolrFieldSerializer {
        /// <inheritdoc />
        public bool CanHandleType(Type t) {
            return typeof (IFormattable).IsAssignableFrom(t);
        }

        /// <inheritdoc />
        public IEnumerable<PropertyNode> Serialize(object obj) {
            if (obj == null)
                yield break;
            var v = (IFormattable) obj;
            yield return new PropertyNode {
                FieldValue = v.ToString(null, CultureInfo.InvariantCulture)
            };
        }
    }
}
