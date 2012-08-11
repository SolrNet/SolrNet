using System;
using System.Net;
using HttpWebAdapters.Adapters;

namespace HttpWebAdapters {
    public class BasicAuthHttpWebRequestFactory : IHttpWebRequestFactory
    {
        private string username;
        private string password;

        public BasicAuthHttpWebRequestFactory(string username, string password)
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