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

using System.IO;
using System.Net;
using MbUnit.Framework;

namespace SolrNet.Tests {
    [TestFixture]
    public class HttpTests {
        private const int Reps = 100;

        [Test]
        [Ignore("performance test")]
        public void ReadToEnd() {
            for (int i = 0; i < Reps; i++) {
                var req = WebRequest.Create("http://www.google.com");
                using (var r = req.GetResponse())
                using (var rs = r.GetResponseStream())
                using (var sr = new StreamReader(rs))
                    sr.ReadToEnd();
            }
        }

        [Test]
        [Ignore("performance test")]
        public void ReadFully() {
            for (int i = 0; i < Reps; i++) {
                var req = WebRequest.Create("http://www.google.com");
                using (var r = req.GetResponse())
                using (var rs = r.GetResponseStream())
                    ReadFully(rs);
            }
        }

        /// <summary>
        /// Reads data from a stream until the end is reached. The
        /// data is returned as a byte array. An IOException is
        /// thrown if any of the underlying IO calls fail.
        /// From http://www.yoda.arachsys.com/csharp/readbinary.html
        /// </summary>
        /// <param name="stream">The stream to read data from</param>
        public static byte[] ReadFully(Stream stream) {
            var buffer = new byte[32768];
            using (var ms = new MemoryStream()) {
                while (true) {
                    int read = stream.Read(buffer, 0, buffer.Length);
                    if (read <= 0)
                        return ms.ToArray();
                    ms.Write(buffer, 0, read);
                }
            }
        }
    }
}