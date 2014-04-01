using System;
using System.Net;
using System.Text;
using HttpWebAdapters.Adapters;

namespace HttpWebAdapters {
    /// <summary>
    /// Creates a web request that does basic auth
    /// </summary>
    public class BasicAuthHttpWebRequestFactory : IHttpWebRequestFactory {
        static private IWebProxy proxy;
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

        private readonly string username;
        private readonly string password;

        /// <summary>
        /// Creates a web request that does basic auth
        /// </summary>
        /// <param name="username">HTTP username</param>
        /// <param name="password">HTTP password</param>
        public BasicAuthHttpWebRequestFactory(string username, string password) {
            this.username = username;
            this.password = password;
        }

        public IHttpWebRequest Create(string url) {
            return Create(new Uri(url));
        }

        public IHttpWebRequest Create(Uri url) {
            var req = (HttpWebRequest) WebRequest.Create(url);
            var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes(username + ":" + password));
            req.Headers.Add("Authorization", "Basic " + credentials);
            if (this.Proxy != null) {
                req.Proxy = this.Proxy;
            }
            if (this.UserAgent != null) {
                req.UserAgent = this.UserAgent;
            }
            return new HttpWebRequestAdapter(req);
        }
    }
}
