using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Xml;
using HttpWebAdapters;
using HttpWebAdapters.Adapters;
using SolrNet.Exceptions;
using SolrNet.Utils;

namespace SolrNet {
	public class SolrConnection : ISolrConnection {
		private string serverURL;
		private IHttpWebRequestFactory httpWebRequestFactory = new HttpWebRequestFactory();
		private Encoding xmlEncoding = Encoding.UTF8;
		private string version = "2.2";

		public SolrConnection(string serverURL) {
			ServerURL = serverURL;
		}

		public SolrConnection(string serverURL, IHttpWebRequestFactory httpWebRequestFactory) {
			ServerURL = serverURL;
			this.httpWebRequestFactory = httpWebRequestFactory;
		}

		public string ServerURL {
			get { return serverURL; }
			set {
				try {
					Uri u = new Uri(value);
					if (u.Scheme != Uri.UriSchemeHttp && u.Scheme != Uri.UriSchemeHttps)
						throw new InvalidURLException("Only HTTP or HTTPS protocols are supported");
				} catch (ArgumentException e) {
					throw new InvalidURLException(e);
				} catch (UriFormatException e) {
					throw new InvalidURLException(e);
				}
				serverURL = value;
			}
		}

		public string Version {
			get { return version; }
			set { version = value; }
		}

		public Encoding XmlEncoding {
			get { return xmlEncoding; }
			set { xmlEncoding = value; }
		}

		public static byte[] ReadFully(Stream stream) {
			byte[] buffer = new byte[32768];
			using (MemoryStream ms = new MemoryStream()) {
				while (true) {
					int read = stream.Read(buffer, 0, buffer.Length);
					if (read <= 0)
						return ms.ToArray();
					ms.Write(buffer, 0, read);
				}
			}
		}

		public string Post(string relativeUrl, string s) {
			IHttpWebRequest request = httpWebRequestFactory.Create(serverURL);
			request.Method = HttpWebRequestMethod.POST;
			request.ContentType = "text/xml; charset=utf-8";
			request.ContentLength = s.Length;
			request.ProtocolVersion = HttpVersion.Version10;
			try {
				using (Stream postParams = request.GetRequestStream()) {
					postParams.Write(xmlEncoding.GetBytes(s), 0, s.Length);
					using (IHttpWebResponse response = request.GetResponse()) {
						using (Stream rStream = response.GetResponseStream()) {
							string r = xmlEncoding.GetString(ReadFully(rStream));
							//Console.WriteLine(r);
							return r;
						}
					}
				}
			} catch (WebException e) {
				throw new SolrConnectionException(e);
			}
		}

		public XmlDocument PostXml(string relativeUrl, XmlDocument xml) {
			string xmlResponse = Post(relativeUrl, xml.ToString());
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(xmlResponse);
			return doc;
		}

		public string Get(string relativeUrl, IDictionary<string, string> parameters) {
			UriBuilder u = new UriBuilder(serverURL);
			u.Path += relativeUrl;
			if (parameters == null)
				parameters = new Dictionary<string, string>();
			parameters["version"] = version;
			u.Query = Func.Reduce(
				Func.Map<KeyValuePair<string, string>, string>(parameters,
				                                               delegate(KeyValuePair<string, string> input) {
				                                               	return
				                                               		string.Format("{0}={1}", HttpUtility.UrlEncode(input.Key),
				                                               		              HttpUtility.UrlEncode(input.Value));
				                                               }), "?",
				delegate(string x, string y) {
					return string.Format("{0}&{1}", x, y);
				});
			//Console.WriteLine(u.Uri);
			IHttpWebRequest request = httpWebRequestFactory.Create(u.Uri);
			request.Method = HttpWebRequestMethod.GET;
			request.ProtocolVersion = HttpVersion.Version10;
			try {
				using (IHttpWebResponse response = request.GetResponse()) {
					using (Stream rStream = response.GetResponseStream()) {
						return xmlEncoding.GetString(ReadFully(rStream));
					}
				}
			} catch (WebException e) {
				if (e.Response != null) {
					IHttpWebResponse r = new HttpWebResponseAdapter(e.Response);
					if (r.StatusCode == HttpStatusCode.BadRequest) {
						throw new InvalidFieldException(r.StatusDescription, e);
					}					
				}
				throw new SolrConnectionException(e);
			}
		}
	}
}