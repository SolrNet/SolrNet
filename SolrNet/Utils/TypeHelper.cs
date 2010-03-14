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
using System.ComponentModel;

namespace SolrNet.Utils {
    /// <summary>
    /// <see cref="Type"/>-related helper functions
    /// </summary>
    public static class TypeHelper {
        // From http://davidhayden.com/blog/dave/archive/2006/11/26/IsTypeNullableTypeConverter.aspx
        public static Type GetUnderlyingNullableType(Type t) {
            if (!IsNullableType(t))
                return t;
            var nc = new NullableConverter(t);
            return nc.UnderlyingType;
        }

        public static Type MakeNullable(Type t) {
            return typeof (Nullable<>).MakeGenericType(t);
        }


        // From http://davidhayden.com/blog/dave/archive/2006/11/26/IsTypeNullableTypeConverter.aspx
        public static bool IsNullableType(Type theType) {
            return theType.IsGenericType && theType.GetGenericTypeDefinition().Equals(typeof (Nullable<>));
        }

        public static bool IsGenericAssignableFrom(Type t, Type other) {
            if (other.GetGenericArguments().Length != t.GetGenericArguments().Length)
                return false;
            var genericT = t.MakeGenericType(other.GetGenericArguments());
            return genericT.IsAssignableFrom(other);
        }
    }
}