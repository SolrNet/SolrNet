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

        /// <summary>
        ///     Creates a web request that uses 509 Client Certificates
        /// </summary>
        /// <param name="certificate">X5092 Certificate with private key</param>
        public ClientCertificateHttpWebRequestFactory(X509Certificate2 certificate) {
            this.certificate = certificate;
        }

        /// <inheritdoc />
        public IHttpWebRequest Create(Uri url) {
            var req = (HttpWebRequest) WebRequest.Create(url);
            req.ClientCertificates.Add(certificate);
            return new HttpWebRequestAdapter(req);
        }

        public IHttpWebRequest Create(string url) {
            return Create(new Uri(url));
        }
    }
}
