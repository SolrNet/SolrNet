using System;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using HttpWebAdapters.Adapters;

namespace HttpWebAdapters {
    /// <summary>
    ///     Creates a web request that uses client certificates
    /// </summary>
    public class ClientCertificateHttpWebRequestFactory : IHttpWebRequestFactory {
        private readonly X509Certificate2 certificate;

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
	        
        /// <summary>
        ///     Creates a web request that uses 509 Client Certificates
        /// </summary>
        /// <param name="certificate">X5092 Certificate with private key</param>
        public ClientCertificateHttpWebRequestFactory(X509Certificate2 certificate) {
            this.certificate = certificate;
        }

        public IHttpWebRequest Create(Uri url) {
            var req = (HttpWebRequest) WebRequest.Create(url);
            req.ClientCertificates.Add(certificate);
            if (this.Proxy != null) {
		            req.Proxy = this.Proxy;
		    }
		    if (this.UserAgent != null) {
		            req.UserAgent = this.UserAgent;
		    }
            return new HttpWebRequestAdapter(req);
        }

        public IHttpWebRequest Create(string url) {
            return Create(new Uri(url));
        }
    }
}
