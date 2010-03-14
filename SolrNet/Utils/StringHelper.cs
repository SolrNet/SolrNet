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

namespace SolrNet.Utils {
    public class StringHelper {
        /// <summary>
        /// Converts an object to null string if null or executes ToString() on it.
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static string ToNullOrString(object o) {
            if (o == null)
                return null;
            return o.ToString();
        }
    }
}