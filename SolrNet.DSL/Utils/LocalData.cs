#region license
// Copyright (c) 2005 - 2007 Ayende Rahien (ayende@ayende.com)
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
// 
//     * Redistributions of source code must retain the above copyright notice,
//     this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice,
//     this list of conditions and the following disclaimer in the documentation
//     and/or other materials provided with the distribution.
//     * Neither the name of Ayende Rahien nor the names of its
//     contributors may be used to endorse or promote products derived from this
//     software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
// THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion

using System;
using System.Collections;
using System.Web;
using Rhino.Commons.LocalDataImpl;

namespace Rhino.Commons
{
    internal class Local
    {
        static ILocalData current = new LocalData();
        static object LocalDataHashtableKey = new object();
        private class LocalData : ILocalData
        {
            [ThreadStatic]
            static Hashtable thread_hashtable;

            private static Hashtable Local_Hashtable
            {
                get
                {
                    if (!RunningInWeb)
                    {
                        return thread_hashtable ??
                        (
                            thread_hashtable = new Hashtable()
                        );
                    }
                    Hashtable web_hashtable = HttpContext.Current.Items[LocalDataHashtableKey] as Hashtable;
                    if(web_hashtable==null)
                    {
                        HttpContext.Current.Items[LocalDataHashtableKey] = web_hashtable = new Hashtable();
                    }
                    return web_hashtable;
                }
            }

            public object this[object key]
            {
                get { return Local_Hashtable[key]; }
                set { Local_Hashtable[key] = value; }
            }

            public void Clear()
            {
                Local_Hashtable.Clear();
            }
        }

        public static ILocalData Data
        {
            get { return current; }
        }

        public static bool RunningInWeb
        {
            get { return HttpContext.Current != null; }
        }
    }
}
