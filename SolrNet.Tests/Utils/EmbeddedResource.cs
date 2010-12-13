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
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace SolrNet.Tests.Utils {
    public class EmbeddedResource {
        /// <summary>
        /// Extracts an embedded file out of a given assembly.
        /// </summary>
        /// <param name="assemblyName">The namespace of you assembly.</param>
        /// <param name="fileName">The name of the file to extract.</param>
        /// <returns>A stream containing the file data.</returns>
        public static Stream GetEmbeddedFile(string assemblyName, string fileName) {
            try {
                Assembly a = Assembly.Load(assemblyName);
                Stream str = a.GetManifestResourceStream(assemblyName + "." + fileName);

                if (str == null)
                    throw new Exception("Could not locate embedded resource '" + fileName + "' in assembly '" + assemblyName + "'");
                return str;
            } catch (Exception e) {
                throw new Exception(assemblyName + ": " + e.Message);
            }
        }

        /// <summary>
        /// Extracts an embedded resource as a Stream.
        /// </summary>
        /// <param name="assembly">The assembly containing the resouce.</param>
        /// <param name="fileName">The name of the resource.</param>
        /// <returns>A Stream with the contents of the embedded resource.</returns>
        public static Stream GetEmbeddedFile(Assembly assembly, string fileName) {
            string assemblyName = assembly.GetName().Name;
            return GetEmbeddedFile(assemblyName, fileName);
        }

        /// <summary>
        /// Extracts an embedded resource as a Stream.
        /// </summary>
        /// <param name="type">A type contained in the assembly containing the resource.</param>
        /// <param name="fileName">The name of the resource.</param>
        /// <returns>A Stream with the contents of the embedded resource.</returns>
        public static Stream GetEmbeddedFile(Type type, string fileName) {
            string assemblyName = type.Assembly.GetName().Name;
            return GetEmbeddedFile(assemblyName, fileName);
        }

        /// <summary>
        /// Extracts an embedded resource as a string.
        /// </summary>
        /// <param name="type">A type contained in the assembly containing the resource.</param>
        /// <param name="fileName">The name of the resource.</param>
        /// <returns>A string with the contents of the embedded resource.</returns>
        public static string GetEmbeddedString(Type type, string fileName) {
            using (var s = GetEmbeddedFile(type, fileName))
            using (var sr = new StreamReader(s, Encoding.UTF8))
                return sr.ReadToEnd();
        }

        /// <summary>
        /// Extracts an XmlDocument out of a embedded resource.
        /// </summary>
        /// <param name="type">A type contained in the assembly containing the resource.</param>
        /// <param name="fileName">The name of the resource</param>
        /// <returns>An XmlDocument with the contents of the embedded resource.</returns>
        public static XDocument GetEmbeddedXml(Type type, string fileName) {
            Stream str = GetEmbeddedFile(type, fileName);
            var tr = new XmlTextReader(str);
            return XDocument.Load(tr);
        }
    }
}