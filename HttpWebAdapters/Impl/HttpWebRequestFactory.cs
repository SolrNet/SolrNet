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
using System.Net;
using HttpWebAdapters.Adapters;

namespace HttpWebAdapters {
	public class HttpWebRequestFactory : IHttpWebRequestFactory {
	        static private IWebProxy proxy = null;
	        public IWebProxy Proxy
	        {
	            get { return proxy; }
	            set { proxy = value; }
	        }
	
	        static private string userAgent = null;
	        public string UserAgent
	        {
	            get { return userAgent; }
	            set { userAgent = value; }
	        }

		public IHttpWebRequest Create(string url) {
			return this.Create(new Uri(url));
		}

		public IHttpWebRequest Create(Uri url) {
			var request = new HttpWebRequestAdapter((HttpWebRequest) WebRequest.Create(url));
		        if (this.Proxy != null) {
		                request.Proxy = this.Proxy;
		        }
		        if (this.UserAgent != null) {
		                request.UserAgent = this.UserAgent;
		        }
		        return request;
		}
	}
}
