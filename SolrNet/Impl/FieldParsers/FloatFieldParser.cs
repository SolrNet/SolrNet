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
using System.Globalization;
using System.Xml.Linq;

namespace SolrNet.Impl.FieldParsers {
    /// <summary>
    /// Parses float values
    /// </summary>
    public class FloatFieldParser : ISolrFieldParser {
        public bool CanHandleSolrType(string solrType) {
            return solrType == "float" || solrType == "int";
        }

        public bool CanHandleType(Type t) {
            return t == typeof (float);
        }

        public static float Parse(string value) {
            return float.Parse(value, CultureInfo.InvariantCulture.NumberFormat);
        }

        public static float Parse(XElement field) {
            if (field == null)
                throw new ArgumentNullException("field");
            return Parse(field.Value);
        }

        public object Parse(XElement field, Type t) {
            return Parse(field);
        }
    }
}