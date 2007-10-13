using System;
using System.IO;
using System.Net;

namespace SolrNet.Tests {
	public class HttpWebResponseAdapter : IHttpWebResponse {
		private HttpWebResponse response;

		public HttpWebResponseAdapter(HttpWebResponse response) {
			this.response = response;
		}

		///<summary>
		///Gets the contents of a header that was returned with the response.
		///</summary>
		///
		///<returns>
		///The contents of the specified header.
		///</returns>
		///
		///<param name="headerName">The header value to return. </param>
		///<exception cref="T:System.ObjectDisposedException">The current instance has been disposed. </exception>
		public string GetResponseHeader(string headerName) {
			return response.GetResponseHeader(headerName);
		}

		///<summary>
		///Gets or sets the cookies that are associated with this response.
		///</summary>
		///
		///<returns>
		///A <see cref="T:System.Net.CookieCollection"></see> that contains the cookies that are associated with this response.
		///</returns>
		///
		///<exception cref="T:System.ObjectDisposedException">The current instance has been disposed. </exception>
		public CookieCollection Cookies {
			get { return response.Cookies; }
			set { response.Cookies = value; }
		}

		///<summary>
		///Gets the method that is used to encode the body of the response.
		///</summary>
		///
		///<returns>
		///A string that describes the method that is used to encode the body of the response.
		///</returns>
		///
		///<exception cref="T:System.ObjectDisposedException">The current instance has been disposed. </exception>
		public string ContentEncoding {
			get { return response.ContentEncoding; }
		}

		///<summary>
		///Gets the character set of the response.
		///</summary>
		///
		///<returns>
		///A string that contains the character set of the response.
		///</returns>
		///
		///<exception cref="T:System.ObjectDisposedException">The current instance has been disposed. </exception><PermissionSet><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode" /></PermissionSet>
		public string CharacterSet {
			get { return response.CharacterSet; }
		}

		///<summary>
		///Gets the name of the server that sent the response.
		///</summary>
		///
		///<returns>
		///A string that contains the name of the server that sent the response.
		///</returns>
		///
		///<exception cref="T:System.ObjectDisposedException">The current instance has been disposed. </exception>
		public string Server {
			get { return response.Server; }
		}

		///<summary>
		///Gets the last date and time that the contents of the response were modified.
		///</summary>
		///
		///<returns>
		///A <see cref="T:System.DateTime"></see> that contains the date and time that the contents of the response were modified.
		///</returns>
		///
		///<exception cref="T:System.ObjectDisposedException">The current instance has been disposed. </exception><PermissionSet><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" /></PermissionSet>
		public DateTime LastModified {
			get { return response.LastModified; }
		}

		///<summary>
		///Gets the status of the response.
		///</summary>
		///
		///<returns>
		///One of the <see cref="T:System.Net.HttpStatusCode"></see> values.
		///</returns>
		///
		///<exception cref="T:System.ObjectDisposedException">The current instance has been disposed. </exception>
		public HttpStatusCode StatusCode {
			get { return response.StatusCode; }
		}

		///<summary>
		///Gets the status description returned with the response.
		///</summary>
		///
		///<returns>
		///A string that describes the status of the response.
		///</returns>
		///
		///<exception cref="T:System.ObjectDisposedException">The current instance has been disposed. </exception>
		public string StatusDescription {
			get { return response.StatusDescription; }
		}

		///<summary>
		///Gets the version of the HTTP protocol that is used in the response.
		///</summary>
		///
		///<returns>
		///A <see cref="T:System.Version"></see> that contains the HTTP protocol version of the response.
		///</returns>
		///
		///<exception cref="T:System.ObjectDisposedException">The current instance has been disposed. </exception>
		public Version ProtocolVersion {
			get { return response.ProtocolVersion; }
		}

		///<summary>
		///Gets the method that is used to return the response.
		///</summary>
		///
		///<returns>
		///A string that contains the HTTP method that is used to return the response.
		///</returns>
		///
		///<exception cref="T:System.ObjectDisposedException">The current instance has been disposed. </exception>
		public string Method {
			get { return response.Method; }
		}

		///<summary>
		///When overridden by a descendant class, closes the response stream.
		///</summary>
		///
		///<exception cref="T:System.NotSupportedException">Any attempt is made to access the method, when the method is not overridden in a descendant class. </exception><PermissionSet><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" /></PermissionSet>
		public void Close() {
			response.Close();
		}

		///<summary>
		///When overridden in a descendant class, returns the data stream from the Internet resource.
		///</summary>
		///
		///<returns>
		///An instance of the <see cref="T:System.IO.Stream"></see> class for reading data from the Internet resource.
		///</returns>
		///
		///<exception cref="T:System.NotSupportedException">Any attempt is made to access the method, when the method is not overridden in a descendant class. </exception><PermissionSet><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" /></PermissionSet>
		public Stream GetResponseStream() {
			return response.GetResponseStream();
		}

		///<summary>
		///Gets a <see cref="T:System.Boolean"></see> value that indicates whether this response was obtained from the cache.
		///</summary>
		///
		///<returns>
		///true if the response was taken from the cache; otherwise, false.
		///</returns>
		///
		public bool IsFromCache {
			get { return response.IsFromCache; }
		}

		///<summary>
		///Gets a <see cref="T:System.Boolean"></see> value that indicates whether mutual authentication occurred.
		///</summary>
		///
		///<returns>
		///true if both client and server were authenticated; otherwise, false.
		///</returns>
		///
		public bool IsMutuallyAuthenticated {
			get { return response.IsMutuallyAuthenticated; }
		}

		///<summary>
		///When overridden in a descendant class, gets or sets the content length of data being received.
		///</summary>
		///
		///<returns>
		///The number of bytes returned from the Internet resource.
		///</returns>
		///
		///<exception cref="T:System.NotSupportedException">Any attempt is made to get or set the property, when the property is not overridden in a descendant class. </exception><PermissionSet><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" /></PermissionSet>
		public long ContentLength {
			get { return response.ContentLength; }
			set { response.ContentLength = value; }
		}

		///<summary>
		///When overridden in a derived class, gets or sets the content type of the data being received.
		///</summary>
		///
		///<returns>
		///A string that contains the content type of the response.
		///</returns>
		///
		///<exception cref="T:System.NotSupportedException">Any attempt is made to get or set the property, when the property is not overridden in a descendant class. </exception><PermissionSet><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" /></PermissionSet>
		public string ContentType {
			get { return response.ContentType; }
			set { response.ContentType = value; }
		}

		///<summary>
		///When overridden in a derived class, gets the URI of the Internet resource that actually responded to the request.
		///</summary>
		///
		///<returns>
		///An instance of the <see cref="T:System.Uri"></see> class that contains the URI of the Internet resource that actually responded to the request.
		///</returns>
		///
		///<exception cref="T:System.NotSupportedException">Any attempt is made to get or set the property, when the property is not overridden in a descendant class. </exception><PermissionSet><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" /></PermissionSet>
		public Uri ResponseUri {
			get { return response.ResponseUri; }
		}

		///<summary>
		///When overridden in a derived class, gets a collection of header name-value pairs associated with this request.
		///</summary>
		///
		///<returns>
		///An instance of the <see cref="T:System.Net.WebHeaderCollection"></see> class that contains header values associated with this response.
		///</returns>
		///
		///<exception cref="T:System.NotSupportedException">Any attempt is made to get or set the property, when the property is not overridden in a descendant class. </exception><PermissionSet><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" /></PermissionSet>
		public WebHeaderCollection Headers {
			get { return response.Headers; }
		}

		///<summary>
		///Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		///</summary>
		///<filterpriority>2</filterpriority>
		public void Dispose() {
			//response.GetType().GetMethod("Dispose", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(response, null);
		}
	}
}