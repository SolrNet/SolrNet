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
using NHibernate.Connection;
using NHibernate.Dialect;
using NHibernate.Driver;
using Environment=NHibernate.Cfg.Environment;

namespace NHibernate.SolrNet.Tests {
    public static class ConfigurationExtensions {

        public static Configuration GetEmptyNHConfig() {
            var nhConfig = new Configuration {
                Properties = new Dictionary<string, string> {
                    {Environment.ConnectionProvider, typeof(DriverConnectionProvider).FullName},
                    {Environment.ConnectionDriver, typeof(SQLite20Driver).FullName},
                    {Environment.Dialect, typeof(SQLiteDialect).FullName},
                    {Environment.ConnectionString, "Data Source=test.db;Version=3;New=True;"},
                }
            };
            return nhConfig;
        }

        public static Configuration GetNhConfig() {
            var nhConfig = GetEmptyNHConfig();
            nhConfig.Register(typeof (Entity));
            return nhConfig;
        }
    }
}