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
using System.Net;
using HttpWebAdapters.Adapters;

namespace HttpWebAdapters {
	public class HttpWebRequestFactory : IHttpWebRequestFactory {
		public IHttpWebRequest Create(string url) {
			return new HttpWebRequestAdapter((HttpWebRequest) WebRequest.Create(url));
		}

		public IHttpWebRequest Create(Uri url) {
			return new HttpWebRequestAdapter((HttpWebRequest) WebRequest.Create(url));
		}
	}

    public class SecureHttpWebRequestFactory : IHttpWebRequestFactory
    {
        private string username;
        private string password;

        public SecureHttpWebRequestFactory(string username, string password)
        {
            this.username = username;
            this.password = password;
        }

        public IHttpWebRequest Create(string url)
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            string credentials = Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(username + ":" + password));
            req.Headers.Add("Authorization", "Basic " + credentials);
            return new HttpWebRequestAdapter(req);
        }

        public IHttpWebRequest Create(Uri url)
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            string credentials = Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(username + ":" + password));
            req.Headers.Add("Authorization", "Basic " + credentials);
            return new HttpWebRequestAdapter(req);
        }
    }
}