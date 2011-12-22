using System;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using HttpWebAdapters;

namespace SolrNet.Tests.Mocks {
    public class HttpWebRequest : IHttpWebRequest {
        public HttpWebRequestMethod Method { get; set; }

        public Func<IHttpWebResponse> getResponse;

        public IHttpWebResponse GetResponse() {
            return getResponse();
        }

        public Func<Stream> getRequestStream;

        public Stream GetRequestStream() {
            return getRequestStream();
        }

        public void Abort() {
            throw new NotImplementedException();
        }

        public void AddRange(int from, int to) {
            throw new NotImplementedException();
        }

        public void AddRange(int range) {
            throw new NotImplementedException();
        }

        public void AddRange(string rangeSpecifier, int from, int to) {
            throw new NotImplementedException();
        }

        public void AddRange(string rangeSpecifier, int range) {
            throw new NotImplementedException();
        }

        public bool AllowAutoRedirect {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public bool AllowWriteStreamBuffering {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public bool HaveResponse {
            get { throw new NotImplementedException(); }
        }

        public bool KeepAlive { get; set; }

        public bool Pipelined {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public bool PreAuthenticate {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public bool UnsafeAuthenticatedConnectionSharing {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public bool SendChunked {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public DecompressionMethods AutomaticDecompression { get; set; }

        public int MaximumResponseHeadersLength {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public X509CertificateCollection ClientCertificates {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public CookieContainer CookieContainer {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public Func<Uri> requestUri;

        public Uri RequestUri {
            get {
                return requestUri();
            }
        }

        public long ContentLength { get; set; }

        public int Timeout {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public int ReadWriteTimeout {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public Uri Address {
            get { throw new NotImplementedException(); }
        }

        public ServicePoint ServicePoint {
            get { throw new NotImplementedException(); }
        }

        public int MaximumAutomaticRedirections {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public ICredentials Credentials {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public bool UseDefaultCredentials {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public string ConnectionGroupName {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public WebHeaderCollection Headers { get; set; }

        public IWebProxy Proxy {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public Version ProtocolVersion { get; set; }

        public string ContentType { get; set; }

        public string MediaType {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public string TransferEncoding {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public string Connection {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public string Accept {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public string Referer {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public string UserAgent {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public string Expect {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public DateTime IfModifiedSince {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public IAsyncResult BeginGetResponse(AsyncCallback callback, object state) {
            throw new NotImplementedException();
        }

        public IHttpWebResponse EndGetResponse(IAsyncResult result) {
            throw new NotImplementedException();
        }

        public IAsyncResult BeginGetRequestStream(AsyncCallback callback, object state) {
            throw new NotImplementedException();
        }

        public Stream EndGetRequestStream(IAsyncResult result) {
            throw new NotImplementedException();
        }
    }
}