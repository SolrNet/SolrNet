using System;

namespace HttpWebAdapters {
	public interface IHttpWebRequestFactory {
		IHttpWebRequest Create(string url);
		IHttpWebRequest Create(Uri url);
	}
}