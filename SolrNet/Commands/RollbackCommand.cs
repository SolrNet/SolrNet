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
using System.Threading.Tasks;

namespace SolrNet.Commands {
    /// <summary>
    /// Rollbacks all add/deletes made to the index since the last commit.
    /// </summary>
    public class RollbackCommand : ISolrCommand {
        /// <inheritdoc />
        public string Execute(ISolrConnection connection) {
            return connection.Post("/update", "<rollback/>");
        }

        /// <inheritdoc />
        public Task<string> ExecuteAsync(ISolrConnection connection)
        {
            return connection.PostAsync("/update", "<rollback/>");
        }
    }
}
