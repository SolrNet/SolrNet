using System;
using System.IO;
using System.Net;
using HttpWebAdapters;

namespace SolrNet.Tests.Mocks {
    public class HttpWebResponse : IHttpWebResponse {
        public Action dispose;

        public void Dispose() {
            dispose();
        }

        public string GetResponseHeader(string headerName) {
            throw new NotImplementedException();
        }

        public CookieCollection Cookies {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public string ContentEncoding {
            get { throw new NotImplementedException(); }
        }

        public string CharacterSet {
            get { throw new NotImplementedException(); }
        }

        public string Server {
            get { throw new NotImplementedException(); }
        }

        public DateTime LastModified {
            get { throw new NotImplementedException(); }
        }

        public HttpStatusCode StatusCode {
            get { throw new NotImplementedException(); }
        }

        public string StatusDescription {
            get { throw new NotImplementedException(); }
        }

        public Version ProtocolVersion {
            get { throw new NotImplementedException(); }
        }

        public string Method {
            get { throw new NotImplementedException(); }
        }

        public void Close() {
            throw new NotImplementedException();
        }

        public Func<Stream> getResponseStream;

        public Stream GetResponseStream() {
            return getResponseStream();
        }

        public bool IsFromCache {
            get { throw new NotImplementedException(); }
        }

        public bool IsMutuallyAuthenticated {
            get { throw new NotImplementedException(); }
        }

        public long ContentLength {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public string ContentType {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public Uri ResponseUri {
            get { throw new NotImplementedException(); }
        }

        public Func<WebHeaderCollection> headers;

        public WebHeaderCollection Headers {
            get { return headers(); }
        }
    }
}