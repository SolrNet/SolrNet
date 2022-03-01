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

using SolrNet;

namespace LightInject.SolrNet {
    public interface ISolrInjectedConnection<T> {
        ISolrConnection Connection { get; set; }
    }

    /// <summary>
    /// Internal class that deal with storing the connection and saving for use later down the line with other dependency
    /// injected classes.  This is mainly a wrapper to allow for multiple cores to be used if the developer wants to use them.
    /// </summary>
    /// <typeparam name="T">The type of connection we need to use.</typeparam>
    public class BasicInjectionConnection<T> : ISolrInjectedConnection<T> {
        public ISolrConnection Connection { get; set; }
        
        public BasicInjectionConnection(ISolrConnection connection) {
            Connection = connection;
        }
    }
}
