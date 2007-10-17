namespace HttpWebAdapters {
	public class HttpWebRequestMethod {
		private string method;
		private static readonly string SGET = "GET";
		private static readonly string SPOST = "POST";
		public static readonly HttpWebRequestMethod GET = new HttpWebRequestMethod(SGET);
		public static readonly HttpWebRequestMethod POST = new HttpWebRequestMethod(SPOST);

		private HttpWebRequestMethod(string m) {
			method = m;
		}

		public override string ToString() {
			return method;
		}

		public static HttpWebRequestMethod Parse(string s) {
			if (s == SGET)
				return GET;
			return POST;
		}
	}
}