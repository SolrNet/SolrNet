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
using System.Runtime.Serialization;
using Xunit;
using SolrNet.Exceptions;

namespace SolrNet.Tests {
    
    public class ExceptionTests {
        [Fact]
        public void All_exceptions_are_serializable() {
            var allExceptions = typeof (SolrNetException).Assembly.GetTypes().Where(t => typeof (SolrNetException).IsAssignableFrom(t));
            foreach (var e in allExceptions) {
                Assert.True(typeof(ISerializable).IsAssignableFrom(e), $"{e} is not serializable");
            }
        }
    }
}
