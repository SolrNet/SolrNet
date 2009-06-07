#region license
// Copyright (c) 2007-2009 Mauricio Scheffer
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
using System.Threading;

namespace SolrNet.Tests {
    public static class ThreadSettings {
        private class DisposableCultureInfo : IDisposable {
            private readonly CultureInfo original;

            public DisposableCultureInfo(CultureInfo culture) {
                original = Thread.CurrentThread.CurrentCulture;
                Thread.CurrentThread.CurrentCulture = culture;
            }

            public void Dispose() {
                Thread.CurrentThread.CurrentCulture = original;
            }
        }

        public static IDisposable Culture(CultureInfo culture) {
            return new DisposableCultureInfo(culture);
        }

        public static IDisposable Culture(string culture) {
            return Culture(CultureInfo.GetCultureInfo(culture));
        }
    }
}